
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using books.Data;
using books.Models;

namespace books.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookContext _context;

        public AccountController(BookContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (ModelState.IsValid == true)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == model.Email &&
                    u.Password == model.Password);

                if (user is not null)
                {
                    await Authenticate(model.Email);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid Email or Password");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm model)
        {
            if (ModelState.IsValid == true)
            {
                User? user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user is null)
                {
                    User newUser = new User
                    {
                        Email = model.Email,
                        Password = model.Password,
                    };

                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "This email is already exist");

            }

            return View(model);
        }




        private async Task Authenticate(string email)
        {
            List<Claim> claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims: claims,
                authenticationType: "ApplicationCookie",
                nameType: ClaimsIdentity.DefaultNameClaimType,
                roleType: ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: new ClaimsPrincipal(identity));

        }


    }
}
