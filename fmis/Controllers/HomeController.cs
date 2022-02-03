using fmis.Data;
using fmis.Filters;
using fmis.Models;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers
{

 /*   [Authorize(Roles = "Budget")]*/

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

        public IActionResult Dashboard(int id)
        {
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.filter = new FilterSidebar("dashboard", "home", "");
            ViewBag.layout = "_Layout";

            var ObligationAmount = _MyDbCOntext.ObligationAmount;
            var FundSource = _MyDbCOntext.FundSources;


            DashboardVM dashboard = new DashboardVM();
            dashboard.BudgetAllotments = _MyDbCOntext.Budget_allotments.Where(x=>x.BudgetAllotmentId == id).ToList();
            dashboard.FundSources = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).ToList();
            dashboard.Obligations = _MyDbCOntext.Obligation.Where(x => x.source_id == id).ToList();
            dashboard.Sub_allotments = _MyDbCOntext.Sub_allotment.Where(x => x.BudgetAllotmentId == id).ToList();

            var balance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.Sub_allotment.Where(s=>s.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            ViewBag.Balance = balance;

            var allotment = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbCOntext.Sub_allotment.Where(x => x.BudgetAllotmentId == id).Sum(s => s.Beginning_balance);
            ViewBag.Allotment = allotment;

            var obligated = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.obligated_amount) + _MyDbCOntext.Sub_allotment.Where(x => x.BudgetAllotmentId == id).Sum(s => s.obligated_amount);
            ViewBag.Obligated = obligated;


            /*    var dashboard_report = *//*(from fundsource in _MyDbCOntext.FundSources
                                       join budget_allotment in _MyDbCOntext.Budget_allotments
                                       on fundsource.BudgetAllotmentId equals budget_allotment.BudgetAllotmentId
                                       join yearly in _MyDbCOntext.Yearly_reference
                                       on budget_allotment.YearlyReferenceId equals id*//*
                                       (from budget_allotment in _MyDbCOntext.Budget_allotments
                                        join yearly in _MyDbCOntext.Yearly_reference
                                        on budget_allotment.BudgetAllotmentId equals id
                                        select new DashboardVM
                                       {
                                          // Fundsource = fundsource,
                                           Budgetallotments = budget_allotment
                                       }).ToList();
    */
            // return Json(new { id = id, dash = dashboard_report } );
            /*
                        var obligation = _MyDbCOntext.ObligationAmount.ToList();*/




            return View("~/Views/Home/Index.cshtml", dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private void PopulateYearDropDownList()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _MyDbCOntext.Yearly_reference
                                   orderby d.YearlyReference
                                   select d;
            ViewBag.Year = new SelectList((from s in _MyDbCOntext.Yearly_reference.ToList()
                                              select new
                                              {
                                                  Id = s.YearlyReferenceId,
                                                  year = s.YearlyReferenceId
                                              }),
                                       "Id",
                                       "year",
                                       null);

        }
/*
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
        }*/

    }
}
