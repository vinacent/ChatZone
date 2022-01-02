using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Views.Shared.Components.SettingPanel;

public class SettingPanelViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}