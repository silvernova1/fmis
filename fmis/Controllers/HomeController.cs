using fmis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.layout = null;   
            return View("~/Views/Shared/_LoginPartial.cshtml");
        }

        public IActionResult Dashboard()
        {
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.layout = "_Layout";
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Gallery() 
        {
            ViewBag.filter_sidebar = "gallery";
            ViewBag.layout = "_Layout";
            return View("~/Views/Home/Gallery.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
