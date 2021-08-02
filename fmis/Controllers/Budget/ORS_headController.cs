using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers
{
    public class ORS_headController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            return View("~/Views/Budget/ORS_head.cshtml");
        }
    }
}
