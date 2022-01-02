using ChatZone.Entities;
using ChatZone.Models.Personal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Controllers;

[Route("personal")]
[Authorize]
public class PersonalController : Controller
{
    private readonly UserManager<AppUser> _appUser;

    public PersonalController(UserManager<AppUser> appUser)
    {
        _appUser = appUser;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var user = await _appUser.FindByNameAsync(User.Identity?.Name);
        if (user == null) return NotFound();
        var model = new PersonalViewModel
        {
            User = user
        };
        return View(model);
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(PersonalViewModel input)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Nội dung nhập không hợp lệ";
            return View();
        }

        var user = await _appUser.FindByNameAsync(User.Identity?.Name);
        if (user == null) return NotFound();

        user.FullName = input.User.FullName;
        user.FilterBadWords = input.User.FilterBadWords;
        user.AllowUserPostImage = input.User.AllowUserPostImage;

        await _appUser.UpdateAsync(user);

        ViewBag.Notify = "Đã cập nhật thành công";
        return View(input);
    }
}