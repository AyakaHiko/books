using System.Security.Claims;
using books.Authorization;
using books.Models;
using books.Models.ViewModels.SuperAdminViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace books.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public SuperAdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy = Policies.SuperAdminAccessOnly)]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> ManageUsersClaims()
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();
            IQueryable<User> users = _userManager.Users
                .Where(u => u.EmailConfirmed == true && u.Id != currentUser.Id);
            IList<UserAccessVm> accessVm = new List<UserAccessVm>();
            foreach (var user in users)
            {
                var vm = new UserAccessVm();
                IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
                if (userClaims.Any(c => c.Type == Claims.Admin))
                {
                    vm.Access = Access.Admin;
                }
                else if (userClaims.Any(c => c.Type == Claims.Member))
                {
                    vm.Access = Access.Member;
                }
                else
                {
                    vm.Access = Access.None;
                }

                vm.Email = user.Email;
                accessVm.Add(vm);

            }
            return View(accessVm);
        }

        [HttpPost]
        public async Task<ActionResult> ManageUsersClaims(List<UserAccessVm> userAccessVms)
        {
            ViewBag.Message = null;
            foreach (var userAccessVm in userAccessVms)
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == userAccessVm.Email);
                if (user == null)
                    continue;
                IList<Claim> useClaims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, useClaims);
                switch (userAccessVm.Access)
                {
                    case Access.Admin:
                        await _userManager.AddClaimAsync(user,Claims.AdminClaim);
                        break;
                    case Access.Member:
                        await _userManager.AddClaimAsync(user, Claims.MemberClaim);
                        break;
                }
            }

            ViewBag.Message = "Success";
            return View(userAccessVms);
        }
    }
}
