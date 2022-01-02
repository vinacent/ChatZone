using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Views.Shared.Components.TestComment;

public class TestCommentViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}