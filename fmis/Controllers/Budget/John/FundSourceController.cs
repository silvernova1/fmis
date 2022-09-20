using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Models.John;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using fmis.Filters;
using fmis.Models.silver;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.John
{
    [Authorize(Policy = "BudgetAdmin")]
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _FundSourceContext;
        private readonly UacsContext _uContext;
        private readonly BudgetAllotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly RespoCenterContext _rContext;
        private readonly MyDbContext _MyDbContext;
        private readonly ILogger<FundSourceController> _logger;

        public FundSourceController(FundSourceContext FundSourceContext, UacsContext uContext, BudgetAllotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext, BudgetAllotmentContext BudgetAllotmentContext, RespoCenterContext rContext, ILogger<FundSourceController> logger)
        {
            _FundSourceContext = FundSourceContext;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
            _rContext = rContext;
            _logger = logger;
        }

        public class FundsourceamountData
        {
            public int FundSourceId { get; set; }
            public int UacsId { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal Amount { get; set; }
            public int Id { get; set; }
            public string fundsource_amount_token { get; set; }
            public string fundsource_token { get; set; }
            public int BudgetAllotmentId { get; set; }
        }

        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        #region API
        public ActionResult CheckFundSourceTitle(string title)
        {
            return Ok(_MyDbContext.FundSources.Any(x => x.FundSourceTitle == title && x.BudgetAllotment.YearlyReferenceId == YearlyRefId));
        }
        #endregion

        public async Task<IActionResult> Index(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId, string search)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var res = next_year.AddYears(-1);
            var result = res.Year.ToString();

            var budget_allotment = await _MyDbContext.Budget_allotments
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.RespoCenter)
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.Prexc)
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.Appropriation)
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.AllotmentClass)
            .Include(x => x.Yearly_reference)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BudgetAllotmentId == BudgetAllotmentId);

            var fundsourcesLastYr = await _MyDbContext.FundSources
                .Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == 2 && x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result)
                .Include(x => x.RespoCenter)
                .Include(x => x.Prexc)
                .Include(x => x.Appropriation)
                .Include(x => x.AllotmentClass)
                .Include(x=>x.BudgetAllotment).ThenInclude(x=>x.Yearly_reference)
                .ToListAsync();

            budget_allotment.FundSources = budget_allotment.FundSources.Concat(fundsourcesLastYr).ToList();

            ViewBag.CurrentYrAllotment_beginningbalance = _MyDbContext.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == year && x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId).Sum(x => x.Beginning_balance).ToString("C", new CultureInfo("en-PH"));
            ViewBag.CurrentYrAllotment_remainingbalance = _MyDbContext.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == year && x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId).Sum(x => x.Remaining_balance).ToString("C", new CultureInfo("en-PH"));
            ViewBag.CurrentYrAllotment_obligatedAmount = _MyDbContext.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == year && x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId).Sum(x => x.obligated_amount).ToString("C", new CultureInfo("en-PH"));
            ViewBag.LastYrAllotment_remainingbalance = _MyDbContext.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId && x.IsAddToNextAllotment == true).Sum(x => x.Remaining_balance).ToString("C", new CultureInfo("en-PH"));



            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                ViewBag.Search = search;
                budget_allotment.FundSources = budget_allotment.FundSources
                    .Where(x => x.FundSourceTitle.Contains(search, StringComparison.InvariantCultureIgnoreCase) || x.RespoCenter.Respo.Contains(search, StringComparison.InvariantCultureIgnoreCase) || x.Prexc.pap_title.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return View(budget_allotment);
        }

        [HttpPost]
        public async Task<ActionResult> CheckNextYear(int fundsourceId, bool addToNext)
        {
            var fundsources = await _MyDbContext.FundSources.FindAsync(fundsourceId);

            fundsources.IsAddToNextAllotment = addToNext;

            _MyDbContext.Update(fundsources);
            await _MyDbContext.SaveChangesAsync();


            return Ok(await _MyDbContext.SaveChangesAsync());
        }


        // GET: FundSource/Create
        public async Task<IActionResult> Create(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;

            string ac = AllotmentClassId.ToString();

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.Where(x => x.uacs_type == AllotmentClassId || x.uacs_type == 4).ToListAsync());
            ViewBag.uacs = uacs_data;


            PopulatePrexcsDropDownList();
            PopulateRespoDropDownList();
            PopulateSectionsDropDownList();
            PopulateFundDropDownList();

            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            return View(); //open create
        }
        public IActionResult GetSectionsList(int RespoId)
        {
            List<Sections> selectList = _MyDbContext.Sections.Where(x => x.RespoId == RespoId).ToList();
            ViewBag.Slist = new SelectList(selectList, "SectionId, SectionName");
            return PartialView("DisplaySections");
        }
        // POST: FundSource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FundSource fundSource, int PrexcId, int FundId)
        {
            fundSource.CreatedAt = DateTime.Now;
            fundSource.UpdatedAt = DateTime.Now;
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            var result = _MyDbContext.Prexc.Where(x => x.Id == PrexcId).First();
            var result2 = _MyDbContext.Fund.Where(x => x.FundId == FundId).First();

            /*if (_MyDbContext.FundSources.Where(x => x.FundSourceTitle == fundSource.FundSourceTitle).Count() > 0)
            {
                ModelState.AddModelError("FundSourceTitle", "Duplicate Fund Source Title");
                return View(fundSource);
            }*/

            var fundsource_amount = _MyDbContext.FundSourceAmount.Where(f => f.fundsource_token == fundSource.token).ToList();

            fundSource.Beginning_balance = fundsource_amount.Sum(x => x.beginning_balance);
            fundSource.Remaining_balance = fundsource_amount.Sum(x => x.beginning_balance);

            var item = _MyDbContext.FundSources.Where(x => x.FundSourceTitle.Equals(fundSource.FundSourceTitle)).ToList();
            if (ModelState.IsValid)
            {
                if(item.Count() > 0)
                {

                    ViewBag.Duplicate = fundSource.FundSourceTitle + " already exist in our database.";
                }
                else
                {
                    _FundSourceContext.Add(fundSource);
                    _FundSourceContext.SaveChanges();

                    fundsource_amount.ForEach(a => a.FundSourceId = fundSource.FundSourceId);
                    this._MyDbContext.SaveChanges();
                }
            }
            /*var item = _MyDbContext.FundSources.Where(x => x.FundSourceTitle.Equals(fundSource.FundSourceTitle)).FirstOrDefault();
            if (item!=null)
            {
                ModelState.AddModelError("keyName", "Message");
                return View();
            }*/

            return RedirectToAction("Index", "FundSource", new
            {
                AllotmentClassId = fundSource.AllotmentClassId,
                AppropriationId = fundSource.AppropriationId,
                BudgetAllotmentId = fundSource.BudgetAllotmentId
            });
        }


        //POST
        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.Prexc.ToList();
            return Json(branches.Where(x => x.Id == id).ToList());
        }

        // GET: FundSource/Edit/5
        public async Task<IActionResult> Edit(int fund_source_id, int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            var fundsourcess = _MyDbContext.FundSources.Where(x => x.FundSourceId == fund_source_id)
                .Include(x => x.FundSourceAmounts.Where(x => x.status == "activated").OrderBy( x=> x.UacsId))
                .FirstOrDefault();

            var fundsource = _MyDbContext.FundSources.Where(x => x.FundSourceId == fund_source_id).FirstOrDefault();

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcsDropDownList(fundsource);
            PopulateRespoDropDownList();
            PopulateFundDropDownList();

            return View(fundsource);
        }

        // POST: FundSource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FundSource fundSource)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var funsource_amount = await _MyDbContext.FundSourceAmount.Where(f => f.FundSourceId == fundSource.FundSourceId && f.status == "activated").AsNoTracking().ToListAsync();
            var beginning_balance = funsource_amount.Sum(x => x.beginning_balance);
            var remaining_balance = funsource_amount.Sum(x => x.remaining_balance);

            var fundsource_data = await _MyDbContext.FundSources.Where(s => s.FundSourceId == fundSource.FundSourceId).AsNoTracking().FirstOrDefaultAsync();
            fundsource_data.PrexcId = fundSource.PrexcId;
            fundsource_data.FundId = fundSource.FundId;
            fundsource_data.FundSourceTitle = fundSource.FundSourceTitle;
            fundsource_data.FundSourceTitleCode = fundSource.FundSourceTitleCode;
            fundsource_data.PapType = fundSource.PapType;
            fundsource_data.RespoId = fundSource.RespoId;
            fundsource_data.Beginning_balance = beginning_balance;
            fundsource_data.Remaining_balance = remaining_balance;
            fundsource_data.Original = fundSource.Original;
            fundsource_data.Breakdown = fundSource.Breakdown;

            _MyDbContext.Update(fundsource_data);
            await _MyDbContext.SaveChangesAsync();

            return RedirectToAction("Index", "FundSource", new
            {
                AllotmentClassId = fundSource.AllotmentClassId,
                AppropriationId = fundSource.AppropriationId,
                BudgetAllotmentId = fundSource.BudgetAllotmentId
            });

        }

        [HttpPost]
        public IActionResult SaveFundsourceamount(List<FundsourceamountData> data)
        {
            var data_holder = _MyDbContext.FundSourceAmount;

            foreach (var item in data)
            {
                var fundsource_amount = new FundSourceAmount(); //CLEAR OBJECT
                if (data_holder.Where(s => s.fundsource_amount_token == item.fundsource_amount_token).FirstOrDefault() != null) //CHECK IF EXIST
                    fundsource_amount = data_holder.Where(s => s.fundsource_amount_token == item.fundsource_amount_token).FirstOrDefault();

                fundsource_amount.FundSourceId = item.FundSourceId == 0 ? null : item.FundSourceId;
                fundsource_amount.BudgetAllotmentId = item.BudgetAllotmentId;
                fundsource_amount.UacsId = item.UacsId;
                fundsource_amount.beginning_balance = item.Amount;
                fundsource_amount.remaining_balance = item.Amount;
                fundsource_amount.status = "activated";
                fundsource_amount.fundsource_amount_token = item.fundsource_amount_token;
                fundsource_amount.fundsource_token = item.fundsource_token;
                _MyDbContext.FundSourceAmount.Update(fundsource_amount);
                this._MyDbContext.SaveChanges();
            }

            return Json(data);
        }



        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.PrexcId = new SelectList((from s in _pContext.Prexc.ToList()
                                              select new
                                              {
                                                  PrexcId = s.Id,
                                                  prexc = s.pap_title,
                                                  pap_type = s.pap_type,
                                              }),
                                       "PrexcId",
                                       "prexc",
                                       "pap_type",
                                       null);

        }

        private void PopulateRespoDropDownList()
        {
            ViewBag.RespoId = new SelectList((from s in _MyDbContext.RespoCenter.ToList()
                                              select new
                                              {
                                                  RespoId = s.RespoId,
                                                  respo = s.Respo
                                              }),
                                     "RespoId",
                                     "respo",
                                     null);

        }
        private void PopulateSectionsDropDownList()
        {
            ViewBag.SectionsId = new SelectList((from s in _MyDbContext.Sections.ToList()
                                              select new
                                              {
                                                  SectionId = s.SectionId,
                                                  SectionName = s.SectionName
                                              }),
                                     "SectionId",
                                     "SectionName",
                                     null);

        }

        private void PopulateFundDropDownList()
        {
            var departmentsQuery = from d in _MyDbContext.Fund
                                   orderby d.Fund_description
                                   select d;
            ViewBag.FundId = new SelectList((from s in _MyDbContext.Fund.ToList()
                                             select new
                                             {
                                                 FundId = s.FundId,
                                                 FundDescription = s.Fund_description
                                             }),
                                       "FundId",
                                       "FundDescription",
                                       null);
        }

        // POST: FundSource/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteFundsourceamount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                {
                    var fund_source_amount = _MyDbContext.FundSourceAmount.FirstOrDefault(s => s.fundsource_amount_token == many.many_token);
                    fund_source_amount.status = "deactivated";

                    _MyDbContext.FundSourceAmount.Update(fund_source_amount);
                    await _MyDbContext.SaveChangesAsync();

                    var fund_source_update = await _MyDbContext.FundSources.AsNoTracking().FirstOrDefaultAsync(s => s.token == fund_source_amount.fundsource_token);
                    fund_source_update.Remaining_balance -= fund_source_amount.beginning_balance;

                    //detach para ma calculate ang multiple delete
                    var local = _MyDbContext.Set<FundSource>()
                            .Local
                            .FirstOrDefault(entry => entry.token.Equals(fund_source_amount.fundsource_token));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _MyDbContext.Entry(local).State = EntityState.Detached;
                    }
                    // set Modified flag in your entry
                    _MyDbContext.Entry(fund_source_update).State = EntityState.Modified;
                    //end detach

                    _MyDbContext.FundSources.Update(fund_source_update);
                    _MyDbContext.SaveChanges();
                }
            }
            else
            {
                var fund_source_amount = _MyDbContext.FundSourceAmount.FirstOrDefault(s => s.fundsource_amount_token == data.single_token);
                fund_source_amount.status = "deactivated";

                _MyDbContext.FundSourceAmount.Update(fund_source_amount);
                await _MyDbContext.SaveChangesAsync();

                var fund_source_update = await _MyDbContext.FundSources.AsNoTracking().FirstOrDefaultAsync(s => s.token == fund_source_amount.fundsource_token);
                fund_source_update.Remaining_balance -= fund_source_amount.beginning_balance;
                _MyDbContext.FundSources.Update(fund_source_update);
                _MyDbContext.SaveChanges();
            }

            return Json(data);
        }

        // GET: FundSource/Delete/5
        public async Task<IActionResult> Delete(int? id, int? BudgetId, int budget_id)
        {
            ViewBag.BudgetId = BudgetId;
            ViewBag.budget_id = budget_id;

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _FundSourceContext.FundSource
                .FirstOrDefaultAsync(m => m.FundSourceId == id);
            if (fundSource == null)
            {
                return NotFound();
            }

            return View(fundSource);
        }

        // POST: FundSource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var fundSource = await _MyDbContext.FundSources.Include(x => x.FundSourceAmounts).FirstOrDefaultAsync(x=>x.FundSourceId == id);


            _MyDbContext.Remove(fundSource);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "FundSource", new { 
                AllotmentClassId = fundSource.AllotmentClassId,
                AppropriationId = fundSource.AppropriationId,
                BudgetAllotmentId = fundSource.BudgetAllotmentId
            });
        }



        private bool FundSourceExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            return _FundSourceContext.FundSource.Any(e => e.FundSourceId == id);
        }

        #region COOKIES

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        #endregion
    }
}