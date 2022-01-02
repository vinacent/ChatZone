using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ChatZone.Models;
using ChatZone.Models.Home;
using ChatZone.Views.Shared.Components.CommentTimelineWidget;
using Microsoft.AspNetCore.Authorization;

namespace ChatZone.Controllers;

[Authorize]
[Route("")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult EmptySplash()
    {
        return Redirect("/index");
    }

    [HttpGet("index")]
    public IActionResult Index(CommentTimelineFilter input)
    {
        var model = new HomeViewModel
        {
            CommentTimelineFilter = input
        };
        return View(model);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}