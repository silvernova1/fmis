using fmis.Data;
using fmis.Filters;
using fmis.Models;
using fmis.Models.John;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers
{

    [Authorize]
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

        #region COOKIES

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        #endregion

        public IActionResult Dashboard(int? post_yearly_reference)
        {
            //const string yearly_reference = "YearlyRefId";
            ViewBag.filter_sidebar = "dashboard";
            ViewBag.filter = new FilterSidebar("dashboard", "home", "");
            ViewBag.layout = "_Layout";

            string year = _MyDbCOntext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var res = next_year.AddYears(-1);
            var result = res.Year.ToString();

            /*int id = YearlyRefId;
            if (post_yearly_reference != null)
            {
                id = YearlyRefId;
            }
            else
            {
                id = YearlyRefId;
            }*/

            var suballotmentsLastYr = _MyDbCOntext.SubAllotment
            .Where(x => x.AppropriationId == 1 && x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result)
            .Include(x => x.AllotmentClass).Sum(x => x.Beginning_balance);

            var ObligationAmount = _MyDbCOntext.ObligationAmount;
            var FundSource = _MyDbCOntext.FundSources;

            DashboardVM dashboard = new DashboardVM();
            dashboard.BudgetAllotments = _MyDbCOntext.Budget_allotments.Where(x=>x.BudgetAllotmentId == YearlyRefId).ToList();
            dashboard.FundSources = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId).ToList();
            dashboard.Sub_allotments = _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId).ToList();
            dashboard.AllotmentClasses = _MyDbCOntext.AllotmentClass.Where(x => x.Id == YearlyRefId).ToList();

            var balance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(s=>s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.Remaining_balance) + suballotmentsLastYr;
            ViewBag.Balance = balance;

            var allotmentPS = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId && x.AllotmentClassId == 1).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.Budget_allotment.YearlyReferenceId == YearlyRefId && x.AllotmentClassId == 1).Sum(s => s.Remaining_balance);
            var allotmentPSObligation = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId && x.AllotmentClassId == 1).Sum(x => x.obligated_amount) + _MyDbCOntext.SubAllotment.Where(x => x.Budget_allotment.YearlyReferenceId == YearlyRefId && x.AllotmentClassId == 1).Sum(s => s.obligated_amount);

            var allotmentMOOE = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == 2).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == 2).Sum(s => s.Remaining_balance);
            var allotmentCO = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == 3).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == 3).Sum(s => s.Remaining_balance);

            var allotmentPS_res = balance == 0? 0 : allotmentPS / balance * 100;
            ViewBag.PS = String.Format("{0:0.##}", allotmentPS_res);

            var allotmentPSObligation_res = balance == 0 ? 0 :  allotmentPSObligation / balance * 100;
            ViewBag.PSObligation = String.Format("{0:0.##}", allotmentPSObligation_res);

            var allotmentMOOE_res = balance == 0 ? 0 : allotmentMOOE / balance * 100;
            ViewBag.MOOE = String.Format("{0:0.##}", allotmentMOOE_res);

            var allotmentCO_res = balance == 0 ? 0 : allotmentCO / balance * 100;
            ViewBag.CO = String.Format("{0:0.##}", allotmentCO_res);

            var msd = _MyDbCOntext.FundSources.Where(x => x.RespoId == 1 && x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 1 && x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Remaining_balance);
            var lhsd = _MyDbCOntext.FundSources.Where(x => x.RespoId == 2 && x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 2 && x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Remaining_balance);
            var rdard = _MyDbCOntext.FundSources.Where(x => x.RespoId == 3 && x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 3 && x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Remaining_balance);
            var rled = _MyDbCOntext.FundSources.Where(x => x.RespoId == 4 && x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(x => x.RespoId == 4 && x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Remaining_balance);
            ViewBag.Div = msd.ToString("{0:0.##}");
            var msd_res = balance == 0 ? 0 : msd / balance * 100;
            ViewBag.MSD = String.Format("{0:0.##}", msd_res);

            ViewBag.Div = lhsd.ToString("{0:0.##}");
            var lhsd_res = balance == 0 ? 0 : lhsd / balance * 100;
            ViewBag.LHSD = String.Format("{0:0.##}", lhsd_res);

            ViewBag.Div = rdard.ToString("{0:0.##}");
            var rdard_res = balance == 0 ? 0 : rdard / balance * 100;
            ViewBag.RDARD = String.Format("{0:0.##}", rdard_res);

            ViewBag.Div = rled.ToString("{0:0.##}");
            var rled_res = balance == 0 ? 0 : rled / balance * 100;
            ViewBag.RLED = String.Format("{0:0.##}", rled_res);


            var allotment = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Beginning_balance) + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Beginning_balance) + suballotmentsLastYr;
            ViewBag.Allotment = allotment.ToString("C", new CultureInfo("en-PH"));

            var allotmentbalance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(s => s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.Remaining_balance);
            //var obligated = _MyDbCOntext.ObligationAmount.Sum(x => x.Amount);
            var obligated = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.obligated_amount) + _MyDbCOntext.SubAllotment.Where(s => s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.obligated_amount);
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
