using books.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace books.Controllers
{
    public class TestClaimsController : Controller
    {
        [Authorize(Policy = Policies.SuperAdminAccessOnly)]
        public IActionResult SuperAdmin()
        {
            return Content(User.Identity!.Name!);
        }

        [Authorize(Policy = Policies.AdminAndAboveAccess)]
        public IActionResult Admin()
        {
            return Content(User.Identity!.Name!);
        }

        [Authorize(Policy = Policies.MemberAndAboveAccess)]
        public IActionResult Member()
        {
            return Content(User.Identity!.Name!);
        }
    }
}
