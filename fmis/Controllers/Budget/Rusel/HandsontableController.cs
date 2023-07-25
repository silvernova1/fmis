using fmis.Data;
using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace fmis.Controllers.Budget.Rusel
{
    public class HandsontableController : Controller
    {
        public string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string Username => User.FindFirstValue(ClaimTypes.Name);
        public string UserRole => User.FindFirstValue(ClaimTypes.Role);
        public string FName => User.FindFirstValue(ClaimTypes.GivenName);
        public string LName => User.FindFirstValue(ClaimTypes.Surname);

        private readonly ObligationContext _context;
        private readonly ObligationAmountContext _Ucontext;
        private readonly UacsContext _UacsContext;
        private readonly MyDbContext _MyDbContext;

        public HandsontableController(MyDbContext MyDbContext)
        {

            _MyDbContext = MyDbContext;
        }


        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Handsontable", "");
            ViewBag.layout = "_Layout";

            ViewBag.fullname = FName;

            return View();
        }

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        public async Task<IActionResult> Obligation()
        {
            string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var yearAdded = int.Parse(year);
            var res = next_year.AddYears(-1);
            var lastYr = res.Year.ToString();

            var obligation = await _MyDbContext
                                    .Obligation
                                    .Where(x => x.status == "activated" && x.yearAdded.Year == yearAdded).OrderBy(x => x.Ors_no)
                                    .Include(x => x.ObligationAmounts.Where(x => x.status == "activated"))
                                    .Include(x => x.FundSource)
                                    .Include(x => x.SubAllotment)
                                    .AsNoTracking()
                                    .ToListAsync();

            return Json(obligation);
        }

        public IActionResult fundSub()
        {
            string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var yearAdded = int.Parse(year);
            var res = next_year.AddYears(-1);
            var lastYr = res.Year.ToString();

            var fund_sub_data = (from x in _MyDbContext.FundSources.Where(x => 
            x.BudgetAllotment.YearlyReferenceId == YearlyRefId &&
            x.Original != true || 
            x.FundSourceTitle == "CANCELLED" || 
            x.IsAddToNextAllotment == true && 
            x.FundSourceTitle.Contains("CONAP")).ToList() 
            select new { 
                source_id = x.FundSourceId, 
                source_title = x.FundSourceTitle, 
                remaining_balance = x.Remaining_balance, 
                source_type = "fund_source", 
                obligated_amount = x.obligated_amount })
            .Concat(from y in _MyDbContext.SubAllotment.Where(x => 
            x.Budget_allotment.YearlyReferenceId == YearlyRefId || 
            x.IsAddToNextAllotment == true || 
            x.Suballotment_title == "CANCELLED").ToList() 
            select new { 
                source_id = y.SubAllotmentId, 
                source_title = y.Suballotment_title, 
                remaining_balance = y.Remaining_balance, 
                source_type = "sub_allotment", 
                obligated_amount = y.obligated_amount });

            return Json(fund_sub_data);
        }

        public IActionResult Details()
        {
            Dictionary<string, string> color = new Dictionary<string, string>();
            color["hr_admin"] = "highlight_yellow"; //HR_ADMIN
            color["1889"] = "highlight_pink"; //SILVER ARNELL (201500252 TJ ID #) (0881)
           //color["2652"] = "highlight_blue"; // MARICAR
           //color["2147"] = "highlight_orange"; // MINIE
           //color["201400182"] = "highlight_green"; // JONAH
           //color["2579"] = "highlight_gray"; //ROZELYN
           // color["0664"] = "highlight_salmon"; // ANNIE
            color["2543"] = "highlight_seagreen"; //LESLIE
            color["0848"] = "highlight_lightyellow"; //JEFF
            color["1729"] = "highlight_blue"; // Alexis
            color["2761"] = "highlight_orange"; // JHONDY
            color["1887"] = "highlight_green"; // CARLO
            color["1895"] = "highlight_gray"; // JUDE
            color["2760"] = "highlight_salmon"; // ANGELICA

            var json = new
            {
                fname = User.FindFirstValue(ClaimTypes.GivenName),
                lname = User.FindFirstValue(ClaimTypes.Surname),
                userid = UserId,
                username = Username,
                color = color[Username]
            };
            return Json(json);
        }

    }
}
