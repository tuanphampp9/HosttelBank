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
    private readonly IRoleRepositories _roleRepository;
    public AccountController(IUserRepositories userRepository, IRoleRepositories roleRepositories)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepositories;

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
    [HttpPost]
public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
{
    if (ModelState.IsValid)
    {
        if (string.IsNullOrEmpty(model.UserName))
        {
            // Xử lý khi UserName là null hoặc rỗng
            ModelState.AddModelError("", "Tên đăng nhập không được để trống.");
            return View(model);
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            // Xử lý khi Password là null hoặc rỗng
            ModelState.AddModelError("", "Mật khẩu không được để trống.");
            return View(model);
        }

        var user = await _userRepository.AuthenticateUserAsync(model.UserName, model.Password);
        
        if (user == null)
        {
            // Xử lý khi không tìm thấy người dùng
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        var role = await _roleRepository.GetRoleByIdAsync(user.roleId);

        if (role == null)
        {
            // Xử lý khi không tìm thấy vai trò của người dùng
            ModelState.AddModelError("", "Vai trò của người dùng không hợp lệ.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // Đăng nhập bằng cookie
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.username!),
            new Claim(ClaimTypes.Role, role.roleName!),
            new Claim("UserId", user.userId.ToString()),
            new Claim("RoleName", role.roleName!)
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


    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View(model);
            }

            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = _userRepository.GetUserById(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
                return View(model);;
            }
            else
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return View(model);
            }
        }

        return View(model);
    }
    public IActionResult AccessDenied()
    {
        return View();
    }
}