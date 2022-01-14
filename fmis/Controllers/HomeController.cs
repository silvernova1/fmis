using fmis.Data;
using fmis.Filters;
using fmis.Models;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers
{
/*    [Authorize]*/
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _MyDbCOntext;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _MyDbCOntext = context;
        }


        public IActionResult Index()
        {
            ViewBag.layout = null;   
            return View("~/Views/Shared/_LoginPartial.cshtml");
        }

        public IActionResult Dashboard()
        {
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.filter = new FilterSidebar("dashboard", "home");
            ViewBag.layout = "_Layout";

            var ObligationAmount = _MyDbCOntext.ObligationAmount;
            var FundSource = _MyDbCOntext.FundSources;



            var CustomerViewModel = new DashboardVM
            {
                ObligationAmounts = ObligationAmount,
                FundSources = FundSource
            };



            var obligation = _MyDbCOntext.ObligationAmount.ToList();




            return View("~/Views/Home/Index.cshtml", CustomerViewModel);
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
