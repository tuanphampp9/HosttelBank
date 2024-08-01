using Btl_web_nc.Models;
using Btl_web_nc.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Btl_web_nc.ViewModels;
using System.Text.RegularExpressions;

public class AccountController : Controller
{
    private readonly IUserRepositories _userRepository;
    private readonly IRoleRepositories _roleRepository;
    public AccountController(IUserRepositories userRepository, IRoleRepositories roleRepositories, IPasswordHasher<User> passwordHasher)
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
                var role = await _roleRepository.GetRoleByIdAsync(user.roleId);

                if(role != null)
                {
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
            }

            // Thông báo lỗi
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            ViewData["ReturnUrl"] = returnUrl;
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


    //Dang ky

    [HttpGet]
    public IActionResult SignUp()
    {
        if (HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Username))
                {
                    return BadRequest("Tên đăng ký không được để trống.");
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Mật khẩu không được để trống.");
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu và xác nhận mật khẩu không khớp.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    return BadRequest("Số điện thoại không được để trống.");
                }
                // Kiểm tra định dạng số điện thoại
                if (!IsValidPhoneNumber(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ.");
                    return View(model);
                }

                // Kiểm tra xem tên đăng ký đã tồn tại chưa
                var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng ký đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra xem số điện thoại đã được sử dụng chưa
                var existingPhoneNumber = await _userRepository.GetUserByPhoneNumberAsync(model.PhoneNumber);
                if (existingPhoneNumber != null)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại đã được sử dụng.");
                    return View(model);
                }

                // Mã hóa mật khẩu
                //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Tạo người dùng mới
                var newUser = new User
                {
                    username = model.Username,
                    password = model.Password,
                    phoneNumber = model.PhoneNumber,
                    roleId = 2
                };

                // Lưu người dùng mới vào cơ sở dữ liệu
                await _userRepository.CreateUserAsync(newUser);

                // Đăng nhập người dùng mới
                await SignInUserAsync(newUser);

                //_logger.LogInformation($"User {newUser.Username} registered successfully.");
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "An error occurred while registering a new user.");
            ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.");
        }

        // Nếu có lỗi, hiển thị lại form với thông báo lỗi
        return View(model);
    }

    private async Task SignInUserAsync(User user)
    {
        var role = await _roleRepository.GetRoleByIdAsync(user.roleId);
        if (role != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, role.roleName),
                new Claim("UserId", user.userId.ToString()),
                new Claim("RoleName", role.roleName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Kiểm tra định dạng số điện thoại Việt Nam
        var regex = new Regex(@"^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$");
        return regex.IsMatch(phoneNumber);
    }

}