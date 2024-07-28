using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly IUserRepositories _userRepository;
    public AccountController(IUserRepositories userRepository)
    {
        _userRepository = userRepository;

    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null!)
    {
        // Nếu đã đăng nhập, chuyển hướng đến trang chủ
        if (HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        // Lưu `returnUrl` vào ViewData để sử dụng trong view
        ViewData["ReturnUrl"] = returnUrl;

        // Hiển thị trang đăng nhập
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
    {
        if (ModelState.IsValid)
        {
            if (model.UserName == null)
            {
                // Xử lý khi UserName là null
                return BadRequest("Tên đăng nhập không được để trống.");
            }
            if (model.Password == null)
            {
                // Xử lý khi Password là null
                return BadRequest("Mật khẩu không được để trống.");
            }
            var user = await _userRepository.AuthenticateUserAsync(model.UserName, model.Password);


            if (user != null)
            {
                // Đăng nhập bằng cookie
                var claims = new List<Claim>
                {   
                    new Claim(ClaimTypes.Name, user.username!),
                    new Claim(ClaimTypes.Role, user.roleId.ToString()), // Thay thế bằng loại Role của bạn,
                    new Claim("UserId", user.userId.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    //IsPersistent = model.RememberMe // Lưu cookie trong thời gian dài hạn nếu cần
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);


                return RedirectToLocal(returnUrl);
            }

            // Thông báo lỗi
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        // Clear returnUrl để tránh XSS attack
        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return Redirect(returnUrl);
        }
    }
    public IActionResult AccessDenied()
    {
        return View();
    }
}