using ChatZone.DbContext;
using ChatZone.Entities;
using ChatZone.Helpers;
using ChatZone.Models.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.Controllers;

[Route("comments")]
public class CommentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CommentsController(AppDbContext context,
        UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("remove")]
    [Authorize]
    public async Task<IActionResult> RemoveComment(Guid commentId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment == null) return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        if (user == null) return RedirectToAction("Index", "Home");
        ;

        if (comment.GroupOwnerId == user.Id)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("like")]
    public async Task<IActionResult> LikeComment(Guid commentId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment == null) return NotFound();

        comment.Likes++;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return Content(comment.Likes.ToString());
    }

    [HttpGet("load-comments")]
    public async Task<IActionResult> LoadComments(CommentFilter input)
    {
        var model = new LoadCommnetViewModel
        {
            Filter = input
        };
        if (string.IsNullOrWhiteSpace(input.Location))
        {
            ViewBag.Error = "Kh??ng th??? x??c ?????nh ???????c v??? tr?? trang ??ang th???c hi???n";
            return PartialView(model);
        }

        if (!string.IsNullOrWhiteSpace(input.Owner))
        {
            var user = await _userManager.FindByNameAsync(input.Owner);
            if (user != null)
            {
                var queryable = _context.Comments.Where(x => x.GroupOwnerId == user.Id &&
                                                             x.Location.ToLower() == input.Location.ToLower());

                queryable = queryable.OrderByDescending(x => x.CreationTime);
                var total = await queryable.LongCountAsync();
                model.Data = await queryable.ProcessAsync(input.SkipCount, input.MaxResultCount);
                model.TotalCount = total;

                return PartialView(model);
            }
        }

        ViewBag.Error = "Vui l??ng ????ng nh???p h??? th???ng v?? sao ch??p ????ng ??o???n m?? ???????c cung c???p";
        return PartialView(model);
    }

    [HttpGet("load-box")]
    public async Task<IActionResult> LoadCommentBox(string location, string owner)
    {
        var model = new CommentDto
        {
            Location = location
        };

        if (string.IsNullOrWhiteSpace(location))
        {
            ViewBag.Error = "Kh??ng th??? x??c ?????nh ???????c v??? tr?? trang ??ang th???c hi???n";
            return PartialView(model);
        }

        if (!string.IsNullOrWhiteSpace(owner))
        {
            var user = await _userManager.FindByNameAsync(owner);
            if (user != null)
            {
                model.GroupOwnerId = user.Id;
                ViewBag.AllowUserPostImage = user.AllowUserPostImage is true;
                // Get logged user
                if (User.Identity?.IsAuthenticated == true)
                {
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (currentUser != null)
                    {
                        model.FullName = currentUser.FullName;
                    }
                }

                return PartialView(model);
            }
        }

        ViewBag.Error = "Vui l??ng ????ng nh???p h??? th???ng v?? sao ch??p ????ng ??o???n m?? ???????c cung c???p";
        return PartialView(model);
    }

    [HttpPost("add")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AddComment(CommentDto input, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return Content("<div class=\"vc-alert vc-alert-danger\" role=\"alert\">D??? li???u g???i l??n kh??ng h???p l???</div>");
        }

        if (input.GroupOwnerId <= 0)
        {
            return Content(
                "<div class=\"vc-alert vc-alert-danger\" role=\"alert\">Vui l??ng ????ng nh???p h??? th???ng v?? sao ch??p ????ng ??o???n m?? ???????c cung c???p.</div>");
        }

        var groupOwner = await _userManager.FindByIdAsync(input.GroupOwnerId.ToString());
        if (groupOwner == null)
        {
            return Content(
                "<div class=\"vc-alert vc-alert-danger\" role=\"alert\">Vui l??ng ????ng nh???p h??? th???ng v?? sao ch??p ????ng ??o???n m?? ???????c cung c???p.</div>");
        }

        if (string.IsNullOrWhiteSpace(input.Location))
        {
            return Content(
                "<div class=\"vc-alert vc-alert-danger\" role=\"alert\">Kh??ng th??? x??c ?????nh ???????c v??? tr?? trang ??ang th???c hi???n.</div>");
        }

        if (string.IsNullOrWhiteSpace(input.FullName))
        {
            return Content(
                "<div class=\"vc-alert vc-alert-danger\" role=\"alert\">Vui l??ng nh???p h??? v?? t??n c???a b???n.</div>");
        }

        if (string.IsNullOrWhiteSpace(input.Content))
        {
            return Content(
                "<div class=\"vc-alert vc-alert-danger\" role=\"alert\">Vui l??ng nh???p n???i dung b??nh lu???n.</div>");
        }

        input.ImagePath = await UploadFileAsync(groupOwner, file);

        var badWords = groupOwner.FilterBadWords?.Replace("\r", "\n").Replace("\n", ",")
            .Split(",").Where(x => x.Length > 0)?.ToList() ?? new List<string>();

        foreach (var word in badWords)
        {
            input.Content = input.Content.Replace(word, "***");
        }

        var id = Guid.NewGuid();
        await _context.Comments.AddAsync(new Comment
        {
            Id = id,
            GroupOwnerId = input.GroupOwnerId,
            Location = input.Location ?? "",
            FullName = input.FullName ?? "",
            Content = input.Content ?? "",
            IsApproved = true,
            Likes = 0,
            ImagePath = input.ImagePath ?? "",
            CreationTime = DateTime.Now,
            ParentId = input.ParentId
        });
        await _context.SaveChangesAsync();

        // var result = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

        return Content("<div class=\"vc-alert vc-alert-success\" role=\"alert\">???? g???i b??nh lu???n th??nh c??ng!</div>");
    }

    private async Task<string> UploadFileAsync(AppUser user, IFormFile? input)
    {
        if (input is not { Length: > 0 }) return "";
        var folder = Path.Combine(_webHostEnvironment.WebRootPath, "contents", user.UserName);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var fileName = $"{Guid.NewGuid()}.{Path.GetExtension(input.FileName).TrimStart('.')}";

        var uploaded = Path.Combine(folder,
            fileName);

        await using Stream fileStream = new FileStream(uploaded, FileMode.Create);
        await input.CopyToAsync(fileStream);
        return Path.Combine("contents", user.UserName, fileName);
    }
}