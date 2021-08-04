using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers
{
    public class BudgetAllotmentController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            return View("~/Views/Budget/silver/BudgetAllotment.cshtml");
        }

    }
}