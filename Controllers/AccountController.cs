using ChatZone.Entities;
using ChatZone.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatZone.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return NotFound();
    }

    [HttpGet("login")]
    public IActionResult Login(string loginName, bool isRegisterSuccessful = false)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new LoginDto();
        if (!string.IsNullOrWhiteSpace(loginName))
        {
            model.UserName = loginName;
        }

        if (isRegisterSuccessful)
        {
            ViewBag.Notify = "Đăng ký tài khoản thành công!";
        }

        return View(model);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto input)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, true, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            if (result.IsLockedOut)
            {
                ViewBag.Error = "Tài khoản đã bị tạm khóa";
            }
            else if (result.IsNotAllowed)
            {
                ViewBag.Error = "Tài khoản không được phép đăng nhập";
            }
            else if (result.RequiresTwoFactor)
            {
                ViewBag.Error = "Tài khoản yêu cầu xác minh 2 bước";
            }
            else
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            }

            return View();
        }
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterDto());
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto input)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var appUser = new AppUser
        {
            FullName = input.FullName.Trim(),
            UserName = input.UserName,
            IsIgnoreURLQuerystring = false,
            AllowUserPostImage = true
        };
        var result = await _userManager.CreateAsync(appUser, input.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("Login", new { loginName = input.UserName, isRegisterSuccessful = true });
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View();
        }
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}