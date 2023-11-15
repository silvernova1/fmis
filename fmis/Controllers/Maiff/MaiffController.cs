using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;

namespace fmis.Controllers.Maiff
{
    public class MaiffController : Controller
    {
        [Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("Maiff", "Dv", "");
            return View();
        }
    }
}
