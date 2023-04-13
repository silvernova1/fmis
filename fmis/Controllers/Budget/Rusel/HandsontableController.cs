using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Security.Claims;
using System.Text.Json;

namespace fmis.Controllers.Budget.Rusel
{
    public class HandsontableController : Controller
    {
        public string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string Username => User.FindFirstValue(ClaimTypes.Name);
        public string UserRole => User.FindFirstValue(ClaimTypes.Role);
        public string FName => User.FindFirstValue(ClaimTypes.GivenName);
        public string LName => User.FindFirstValue(ClaimTypes.Surname);

        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Handsontable", "");
            ViewBag.layout = "_Layout";

            ViewBag.fullname = FName;

            return View();
        }

        public IActionResult Details()
        {
            var json = new
            {
                fname = User.FindFirstValue(ClaimTypes.GivenName),
                lname = User.FindFirstValue(ClaimTypes.Surname),
                userid = UserId,
            };
            return Json(json);
        }
    }
}
