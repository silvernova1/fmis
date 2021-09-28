using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers.Budget.Carlo
{
    public class SaobController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Layout = "_Layout";
            ViewBag.filter = new FilterSidebar("budget_report", "saob");
            return View("~/Views/Carlo/Saob/Index.cshtml");
        }
    }
}
