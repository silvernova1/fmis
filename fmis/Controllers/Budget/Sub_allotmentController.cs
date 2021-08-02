using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers
{
    public class Sub_allotmentController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            return View("~/Views/Budget/Sub_allotment.cshtml");
        }
    }
}
