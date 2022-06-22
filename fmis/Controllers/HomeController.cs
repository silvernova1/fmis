using fmis.Data;
using fmis.Filters;
using fmis.Models;
using fmis.Models.John;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers
{

    [Authorize(Roles = "Budget")]

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

        public IActionResult Dashboard(int? post_yearly_reference)
        {
            const string yearly_reference = "_yearly_reference";
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.filter = new FilterSidebar("dashboard", "home", "");
            ViewBag.layout = "_Layout";

            int id = 0;
            if (post_yearly_reference != null)
            {
                HttpContext.Session.SetInt32(yearly_reference, (int)post_yearly_reference);
                id = (int)post_yearly_reference;
            }
            else
            {
                id = (int)HttpContext.Session.GetInt32(yearly_reference);
            }

            var ObligationAmount = _MyDbCOntext.ObligationAmount;
            var FundSource = _MyDbCOntext.FundSources;

            DashboardVM dashboard = new DashboardVM();
            dashboard.BudgetAllotments = _MyDbCOntext.Budget_allotments.Where(x=>x.BudgetAllotmentId == id).ToList();
            dashboard.FundSources = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).ToList();
            dashboard.Sub_allotments = _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id).ToList();
            dashboard.AllotmentClasses = _MyDbCOntext.AllotmentClass.Where(x => x.Id == id).ToList();

            var balance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(s=>s.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            ViewBag.Balance = balance;

            var allotmentPS = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 1).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 1).Sum(s => s.Remaining_balance);
            var allotmentPSObligation = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 1).Sum(x => x.obligated_amount) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 1).Sum(s => s.obligated_amount);

            var allotmentMOOE = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 2).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 2).Sum(s => s.Remaining_balance);
            var allotmentCO = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 3).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id && x.AllotmentClassId == 3).Sum(s => s.Remaining_balance);

            var allotmentPS_res = allotmentPS / balance * 100;
            ViewBag.PS = String.Format("{0:0.##}", allotmentPS_res);

            var allotmentPSObligation_res = allotmentPSObligation / balance * 100;
            ViewBag.PSObligation = String.Format("{0:0.##}", allotmentPSObligation_res);

            var allotmentMOOE_res = allotmentMOOE / balance * 100;
            ViewBag.MOOE = String.Format("{0:0.##}", allotmentMOOE_res);

            var allotmentCO_res = allotmentCO / balance * 100;
            ViewBag.CO = String.Format("{0:0.##}", allotmentCO_res);

            var msd = _MyDbCOntext.FundSources.Where(x => x.RespoId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 1 && x.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            var lhsd = _MyDbCOntext.FundSources.Where(x => x.RespoId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 2 && x.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            var rdard = _MyDbCOntext.FundSources.Where(x => x.RespoId == 3 && x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 3 && x.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            var rled = _MyDbCOntext.FundSources.Where(x => x.RespoId == 4 && x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 4 && x.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            ViewBag.Div = msd.ToString("{0:0.##}");
            var msd_res = msd / balance * 100;
            ViewBag.MSD = String.Format("{0:0.##}", msd_res);

            ViewBag.Div = lhsd.ToString("{0:0.##}");
            var lhsd_res = lhsd / balance * 100;
            ViewBag.LHSD = String.Format("{0:0.##}", lhsd_res);

            ViewBag.Div = rdard.ToString("{0:0.##}");
            var rdard_res = rdard / balance * 100;
            ViewBag.RDARD = String.Format("{0:0.##}", rdard_res);

            ViewBag.Div = rled.ToString("{0:0.##}");
            var rled_res = rled / balance * 100;
            ViewBag.RLED = String.Format("{0:0.##}", rled_res);

            var allotment = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == id).Sum(s => s.Beginning_balance);
            ViewBag.Allotment = allotment.ToString("C", new CultureInfo("en-PH"));

            var allotmentbalance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(s => s.BudgetAllotmentId == id).Sum(s => s.Remaining_balance);
            var obligated = _MyDbCOntext.ObligationAmount.Sum(x => x.Amount);


            ViewBag.Obligated = obligated;


            List<AllotmentClass> allotmentClasses = (from allotmentclass in _MyDbCOntext.AllotmentClass
                                                     select allotmentclass).ToList();
            ViewBag.progress = 0.36 * 100;




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
