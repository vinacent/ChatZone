using ChatZone.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Views.Shared.Components.Navbar;

public class NavbarViewComponent : ViewComponent
{
    private readonly UserManager<AppUser> _userManager;

    public NavbarViewComponent(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");
        return View(user);
    }
}