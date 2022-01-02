using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Views.Shared.Components.Sidebar;

public class SidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}