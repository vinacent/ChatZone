using ChatZone.DbContext;
using ChatZone.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.Views.Shared.Components.CommentMostestPageWidget;

public class CommentMostestPageWidgetViewComponent : ViewComponent
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;

    public CommentMostestPageWidgetViewComponent(AppDbContext appDbContext,
        UserManager<AppUser> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var crrUser = await _userManager.FindByNameAsync(User.Identity?.Name);
        var counter = _appDbContext.Comments.Where(x => x.GroupOwnerId == crrUser.Id).ToList().GroupBy(x => x.Location);
        var commentPages = counter
            .Select(x => new CommentPageCounter { Location = x.Key, CommentCount = x.LongCount() })
            .OrderByDescending(x => x.CommentCount).ToList();

        return View(commentPages);
    }
}