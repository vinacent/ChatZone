using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Views.Shared.Components.Footer;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}