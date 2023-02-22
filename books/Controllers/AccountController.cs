
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using books.Authorization;
using books.Data;
using books.Models;
using books.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace books.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailService _emailService;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            IEmailService emailService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new
                    {
                        userId = user.Id, code
                    },
                    HttpContext.Request.Scheme)!;

                await _emailService.SendEmailAsync(
                    from: "Library",
                    to: model.Email,
                    "Confirm your account",
                    html: $"To confirm registration <a href='{callbackUrl}'>follow the link</a>");

                return View("ConfirmedRegistration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded
                ? RedirectToAction("ConfirmedAccount", new { userName = user.Email })
                : View("Error");
        }


        [HttpGet]
        public IActionResult ConfirmedAccount(string userName)
        {
            return View("ConfirmedAccount", userName);
        }



        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginVm { ReturnUrl = returnUrl });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null)
            {
                if (await _userManager.IsEmailConfirmedAsync(user) == false)
                {
                    ModelState.AddModelError("", "Your email was not confirmed");
                    return View(model);
                }
            }

            if (user is null)
            {
                return View("Error");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                    Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login or password");
            }

            return View(model);
        }

        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Cabinet()
        {
            return Content(User.Identity!.Name!);
        }
    }
}
