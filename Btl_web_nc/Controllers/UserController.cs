using Btl_web_nc.Models;
using Btl_web_nc.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Btl_web_nc.RepositoryInterfaces;

namespace Btl_web_nc.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepositories _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserController(IUserRepositories userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Profile()
        {
            var user = GetCurrentUser();
            var viewModel = new ProfileViewModel
            {
                Username = user.username,
                PhoneNumber = user.phoneNumber
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();

                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    if (!_passwordHasher.VerifyPassword(user.password, model.CurrentPassword))
                    {
                        ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                        return View("Profile", model);
                    }

                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        user.password = _passwordHasher.HashPassword(model.NewPassword);
                    }
                }

                user.username = model.Username;
                user.phoneNumber = model.PhoneNumber;

                _userRepository.UpdateUser(user);
                return RedirectToAction("Profile");
            }

            return View("Profile", model);
        }

        private User GetCurrentUser()
        {
            var username = User.Identity.Name;
            return _userRepository.GetUserByUsername(username);
        }
    }

    
}
