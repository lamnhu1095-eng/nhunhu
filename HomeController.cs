using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DaNangSafeMap.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var username = User.Identity?.Name ?? "Khách";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "user";
            
            ViewData["Username"] = username;
            ViewData["Role"] = role;
            
            return View();
        }

        [AllowAnonymous]
        public IActionResult Public()
        {
            return View();
        }
    }
}