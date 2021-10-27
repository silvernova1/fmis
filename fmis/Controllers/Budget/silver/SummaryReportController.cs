using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Filters;

namespace fmis.Controllers.Budget.silver
{
    public class SummaryReportController: Controller
    {
        public IActionResult Index()
        {
            ViewBag.Layout = "_Layout";
            ViewBag.filter = new FilterSidebar("budget_report", "SummaryReport");
            return View("~/Views/SummaryReport/Index.cshtml");
        }
    }
}
