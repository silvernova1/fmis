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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace fmis.Controllers
{

    //[Authorize]
    //[Authorize(Policy = "Job Order")]
    [Authorize(Policy = "Administrator")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _MyDbCOntext;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _MyDbCOntext = context;
        }

        public IEnumerable<Obligation> obligations {get; set;}

        public IActionResult Index()
        {
            ViewBag.layout = null;
            return View("~/Views/Shared/_LoginPartial.cshtml");
        }

        #region COOKIES

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        #endregion

        public IActionResult Dashboard(string date_from, string date_to, FundSource fundSource, int name, int id)
        {
            Console.WriteLine("user " + User.FindFirstValue(ClaimTypes.Name));

            ViewBag.filter_sidebar = "dashboard";
            ViewBag.filter = new FilterSidebar("dashboard", "home", "");
            ViewBag.layout = "_Layout";
            ViewBag.date_from = date_from;
            ViewBag.date_to = date_to;
            ViewBag.id = id;

            DateTime date1 = Convert.ToDateTime(date_from);
            DateTime date2 = Convert.ToDateTime(date_to);
            DateTime datefilter = Convert.ToDateTime(date_to);
            String date3 = datefilter.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);

            date1.ToString("yyyy-MM-dd 00:00:00");
            date2.ToString("yyyy-MM-dd 23:59:59");
            ViewBag.date1 = date1;
            DateTime dateTimeNow = date2;
            DateTime dateTomorrow = dateTimeNow.Date.AddDays(1);
            /*var lastDayOfMonth = DateTime.DaysInMonth(date1.Year, date1.Month);*/


            //LASTDAY OF THE MONTH
            var firstDayOfMonth = new DateTime(date2.Year, date2.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime lastday = Convert.ToDateTime(lastDayOfMonth);
            lastday.ToString("yyyy-MM-dd 23:59:59");
            ViewBag.lastday = lastday;
            ViewBag.firstDayOfMonth = firstDayOfMonth;


            string year = _MyDbCOntext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var res = next_year.AddYears(-1);
            var result = res.Year.ToString();
            ViewBag.date2 = date2/*.ToString("MMMM dd, yyyy")*/;
            var obligationsFilter = (from oa in _MyDbCOntext.ObligationAmount
                                                  join o in _MyDbCOntext.Obligation
                                                  on oa.ObligationId equals o.Id
                                                  join f in _MyDbCOntext.FundSources
                                                  on o.FundSourceId equals f.FundSourceId
                                                  where o.Date >= date1 && o.Date <= date2
                                                  select new
                                                  {
                                                      amount = oa.Amount,
                                                      uacsId = oa.UacsId,
                                                      sourceId = o.FundSourceId,
                                                      sourceType = o.source_type,
                                                      date = o.Date,
                                                      status = o.status,
                                                      allotmentClassID = f.AllotmentClassId,
                                                      appropriationID = f.AppropriationId
                                                  }).ToList();

            var allotmentClassId = fundSource.AllotmentClassId;
            var appropriationSourceId = fundSource.AppropriationId;
            ViewBag.allotmentClassId = allotmentClassId;
            ViewBag.appropriationSourceId = appropriationSourceId;
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
            dashboard.RespoCenters = _MyDbCOntext.RespoCenter.ToList();

            var balance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.Remaining_balance)
                        + _MyDbCOntext.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance)
                        + _MyDbCOntext.SubAllotment.Where(s => s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.Remaining_balance)
                        + _MyDbCOntext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance);

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

            var allotmentbalance = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.Remaining_balance) + _MyDbCOntext.SubAllotment.Where(s => s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.Remaining_balance);

            var obligated = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.obligated_amount)
                          /*+  _MyDbCOntext.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.obligated_amount)*/
                          +  _MyDbCOntext.SubAllotment.Where(s => s.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(s => s.obligated_amount)
                          +  _MyDbCOntext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.obligated_amount);

            var obligatedAmount = (from oa in _MyDbCOntext.ObligationAmount
                                   join o in _MyDbCOntext.Obligation
                                   on oa.ObligationId equals o.Id
                                   join f in _MyDbCOntext.FundSources
                                   on o.FundSourceId equals f.FundSourceId
                                   join b in _MyDbCOntext.Budget_allotments
                                   on f.BudgetAllotmentId equals b.BudgetAllotmentId
                                   join s in _MyDbCOntext.SubAllotment
                                   on b.BudgetAllotmentId equals s.BudgetAllotmentId
                                   join y in _MyDbCOntext.Yearly_reference
                                   on b.YearlyReferenceId equals y.YearlyReferenceId
                                   //where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday && o.status == "activated" && oa.status == "activated"
                                   where o.Date >= date1 && o.Date <= date2 && o.status == "activated" && oa.status == "activated"
                                   select new
                                   {
                                       obligationamount = oa.Amount,
                                       filteredFs = f.Beginning_balance,
                                       filteredSa = s.Beginning_balance,
                                       filteredFsRb = f.Remaining_balance,
                                       filteredSaRb = s.Remaining_balance,
                                       YearlyRefId = y.YearlyReferenceId,
                                       allotmentClassID = f.AllotmentClassId,
                                       appropriationID = f.AppropriationId

                                   }).ToList();

            var filterObligationsFs = (from oa in _MyDbCOntext.ObligationAmount
                                             join o in _MyDbCOntext.Obligation
                                             on oa.ObligationId equals o.Id
                                             join f in _MyDbCOntext.FundSources
                                             on o.FundSourceId equals f.FundSourceId
                                             where o.Date >= date1 && o.Date <= date2 && o.status == "activated" && oa.status == "activated"
                                            select new
                                             {
                                                 amount = oa.Amount,
                                                 uacsId = oa.UacsId,
                                                 sourceId = o.FundSourceId,
                                                 sourceType = o.source_type,
                                                 date = o.Date,
                                                 status = o.status,
                                                 allotmentClassID = f.AllotmentClassId,
                                                 appropriationID = f.AppropriationId,
                                                 fundSourceTitle = f.FundSourceTitle,
                                                 budgetallotmentId = f.BudgetAllotmentId
                                             }).ToList();
            var filterObligationsSa = (from oa in _MyDbCOntext.ObligationAmount
                                       join o in _MyDbCOntext.Obligation
                                       on oa.ObligationId equals o.Id
                                       join s in _MyDbCOntext.SubAllotment
                                       on o.SubAllotmentId equals s.SubAllotmentId
                                       where o.Date >= date1 && o.Date <= date2 && o.status == "activated" && oa.status == "activated"
                                       select new
                                       {
                                           amount = oa.Amount,
                                           uacsId = oa.UacsId,
                                           sourceId = o.FundSourceId,
                                           sourceType = o.source_type,
                                           date = o.Date,
                                           status = o.status,
                                           allotmentClassID = s.AllotmentClassId,
                                           appropriationID = s.AppropriationId,
                                           fundSourceTitle = s.Suballotment_title,
                                           budgetallotmentId = s.BudgetAllotmentId
                                       }).ToList();

            var allotment = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId).Sum(x => x.Beginning_balance)
                            + _MyDbCOntext.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId).Sum(x => x.Remaining_balance)
                            +_MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId && x.Date >= date1 && x.Date <= date2).Sum(s => s.Beginning_balance)
                            + _MyDbCOntext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId && x.Date >= date1 && x.Date <= date2).Sum(x => x.Remaining_balance);

            ViewBag.Allotment = allotment;

            var filerobligationsTotal = filterObligationsFs.Where(x => x.budgetallotmentId == YearlyRefId && x.appropriationID == appropriationSourceId && x.allotmentClassID == allotmentClassId).Sum(x => x.amount) 
                                        + filterObligationsSa.Where(x => x.budgetallotmentId == YearlyRefId && x.appropriationID == appropriationSourceId && x.allotmentClassID == allotmentClassId).Sum(x => x.amount);

            ViewBag.Obligated = filerobligationsTotal;

            

            var filteredBalances = _MyDbCOntext.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId).Sum(x => x.Remaining_balance)
                            + _MyDbCOntext.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId).Sum(x => x.Remaining_balance)
                            + _MyDbCOntext.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId && x.Date >= date1 && x.Date <= date2).Sum(s => s.Remaining_balance)
                            + _MyDbCOntext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationSourceId && x.Date >= date1 && x.Date <= date2).Sum(x => x.Remaining_balance);

            ViewBag.filteredBalances = allotment - filerobligationsTotal;

            List<AllotmentClass> allotmentClasses = (from allotmentclass in _MyDbCOntext.AllotmentClass
                                                     select allotmentclass).ToList();
            ViewBag.progress = 0.36 * 100;

            var respo = _MyDbCOntext.Budget_allotments.Include(x=>x.FundSources).ThenInclude(x=>x.RespoCenter).ToList();

            ViewBag.respo = respo;

            List<Appropriation> appro = new List<Appropriation>();
            appro = (from a in _MyDbCOntext.Appropriation select a).ToList();
            appro.Insert(0, new Appropriation { AppropriationId = 0, AppropriationSource = "-- Select Appropriation Source --" } );
            ViewBag.appro = appro;

            List<AllotmentClass> allot = new List<AllotmentClass>();
            allot = (from a in _MyDbCOntext.AllotmentClass select a).ToList();
            allot.Insert(0, new AllotmentClass { Id = 0, Allotment_Class = "-- Select Allotment Class --" });


            ViewBag.allot = allot;

            ViewBag.AllotId = new SelectList((from s in _MyDbCOntext.AllotmentClass.ToList()
                                              select new
                                              {
                                                  Id = s.Id,
                                                  allotmentClass = s.Allotment_Class,
                                              }),
                                       "Id",
                                       "allotmentClass",
                                       null);


            //CHART JS BEGIn
            List<DataPoint> dataPoints1 = new List<DataPoint>();
            List<DataPoint> dataPoints2 = new List<DataPoint>();
            List<DataPoint> dataPoints3 = new List<DataPoint>();

            dataPoints1.Add(new DataPoint(1451586600000, 50000, null));
            dataPoints1.Add(new DataPoint(1454265000000, 40000, null));
            dataPoints1.Add(new DataPoint(1456770600000, 30000, null));
            dataPoints1.Add(new DataPoint(1459449000000, 35000, null));
            dataPoints1.Add(new DataPoint(1462041000000, 43000, null));
            dataPoints1.Add(new DataPoint(1464719400000, 60000, null));
            dataPoints1.Add(new DataPoint(1467311400000, 35000, null));
            dataPoints1.Add(new DataPoint(1469989800000, 50000, null));
            dataPoints1.Add(new DataPoint(1472668200000, 70000, "High Renewals"));
            dataPoints1.Add(new DataPoint(1475260200000, 35000, null));
            dataPoints1.Add(new DataPoint(1477938600000, 30000, null));
            dataPoints1.Add(new DataPoint(1480530600000, 37000, null));

            dataPoints2.Add(new DataPoint(1451586600000, 45000, null));
            dataPoints2.Add(new DataPoint(1454265000000, 48000, null));
            dataPoints2.Add(new DataPoint(1456770600000, 40000, null));
            dataPoints2.Add(new DataPoint(1459449000000, 41000, null));
            dataPoints2.Add(new DataPoint(1462041000000, 49000, null));
            dataPoints2.Add(new DataPoint(1464719400000, 46000, null));
            dataPoints2.Add(new DataPoint(1467311400000, 42000, null));
            dataPoints2.Add(new DataPoint(1469989800000, 43000, null));
            dataPoints2.Add(new DataPoint(1472668200000, 50000, null));
            dataPoints2.Add(new DataPoint(1475260200000, 43000, null));
            dataPoints2.Add(new DataPoint(1477938600000, 42000, null));
            dataPoints2.Add(new DataPoint(1480530600000, 50000, null));

            /*dataPoints3.Add(new DataPoint(1451586600000, 27000, null));
            dataPoints3.Add(new DataPoint(1454265000000, 21000, null));
            dataPoints3.Add(new DataPoint(1456770600000, 12000, null));
            dataPoints3.Add(new DataPoint(1459449000000, 18000, null));
            dataPoints3.Add(new DataPoint(1462041000000, 24000, null));
            dataPoints3.Add(new DataPoint(1464719400000, 33000, null));
            dataPoints3.Add(new DataPoint(1467311400000, 16000, null));
            dataPoints3.Add(new DataPoint(1469989800000, 29000, null));
            dataPoints3.Add(new DataPoint(1472668200000, 38000, null));
            dataPoints3.Add(new DataPoint(1475260200000, 24000, null));
            dataPoints3.Add(new DataPoint(1477938600000, 12000, null));
            dataPoints3.Add(new DataPoint(1480530600000, 19000, null));*/


            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);
            ViewBag.DataPoints3 = JsonConvert.SerializeObject(dataPoints3);
            //CHART JS END


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
