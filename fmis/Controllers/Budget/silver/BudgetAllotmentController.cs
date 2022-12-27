using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Data;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Storage;
using fmis.Filters;
using fmis.Data.silver;
using fmis.Models.John;
using System.Globalization;
using fmis.Models.silver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace fmis.Controllers
{
    [Authorize(Policy = "BudgetAdmin")]
    public class BudgetAllotmentController : Controller
    {
        private readonly MyDbContext _context;
        private readonly BudgetAllotmentContext _BudgetAllotmentContext;
        private readonly Yearly_referenceContext _osContext;
        private readonly Ors_headContext _orssContext;
        private readonly PersonalInformationMysqlContext _pis_context;
        private readonly SubAllotmentContext _saContext;

        public BudgetAllotmentController(BudgetAllotmentContext BudgetAllotmentContext, MyDbContext context, Yearly_referenceContext osContext, Ors_headContext orssContext, PersonalInformationMysqlContext pis_context, SubAllotmentContext sa_Context)
        {
            _BudgetAllotmentContext = BudgetAllotmentContext;
            _context = context;
            _osContext = osContext;
            _orssContext = orssContext;
            _pis_context = pis_context;
            _saContext = sa_Context;
        }

        //POST
        public IActionResult selectAT(int? id)
        {
            var branches = _context.AllotmentClass.ToList();
            return Json(branches.Where(x => x.Id == id).ToList());
        }

        // GET: Budget_allotments
        public async Task<IActionResult> Index(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "BudgetAllotment" ,"");
            ViewBag.layout = "_Layout";

            string year = _context.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var res = next_year.AddYears(-1);
            var result = res.Year.ToString();
            ViewBag.Result = result;


            /*var budget_allotment = await _context.Budget_allotments
            .Include(c => c.Yearly_reference)
            .Include(x => x.FundSources)
            .Include(x => x.SubAllotment)
            .ThenInclude(x => x.Appropriation)
            .FirstOrDefaultAsync(x => x.YearlyReferenceId == YearlyRefId);*/

            var budget_allotment = await _context.Budget_allotments
            .Include(c => c.Yearly_reference)
            .Include(x => x.FundSources)
            .Include(x => x.SubAllotment)
                .ThenInclude(x => x.Budget_allotment)
                .ThenInclude(x => x.Yearly_reference)
            .Include(x => x.SubAllotment)
                .ThenInclude(x => x.Appropriation)
            .Include(x => x.SubAllotment)
                .ThenInclude(x => x.SubNegative)
            .FirstOrDefaultAsync(x => x.YearlyReferenceId == YearlyRefId);


            budget_allotment.FundSources.ToList().ForEach(x =>
            {
                x.obligated_amount = _context.Obligation.Include(x => x.ObligationAmounts).Where(y => y.FundSourceId == x.FundSourceId).AsNoTracking().ToList().Sum(x => x.ObligationAmounts.Sum(x => x.Amount));

                x.obligated_amount =+ x.obligated_amount;
                x.Remaining_balance = x.Beginning_balance - x.obligated_amount;
            });

            _context.FundSources.UpdateRange(budget_allotment.FundSources);
            _context.SaveChanges();



            budget_allotment.SubAllotment.ToList().ForEach(x =>
            {
                x.obligated_amount = _context.Obligation.Include(x => x.ObligationAmounts).Where(y => y.SubAllotmentId == x.SubAllotmentId).AsNoTracking().ToList().Sum(x => x.ObligationAmounts.Sum(x => x.Amount));

                x.obligated_amount = +x.obligated_amount;
                x.Remaining_balance = x.Beginning_balance - x.obligated_amount;
                x.Beginning_balance = x.Beginning_balance - x.SubNegative.Where(x => x.SubAllotmentId == x.SubAllotmentId).Sum(x => x.Amount);
            });

            ViewBag.AllotmentClass = await _context.AllotmentClass.Include(x=>x.BudgetAllotments).AsNoTracking().ToListAsync();
            ViewBag.AppropriationSource = await _context.Appropriation.Include(x => x.BudgetAllotments).AsNoTracking().ToListAsync();

            var allotmentClass_Id = _context.AllotmentClass.FirstOrDefault(x => x.Id == 3).Id;

            var suballotmentsLastYr = await _context.SubAllotment
                .Include(x => x.AllotmentClass)
                .Include(x => x.Budget_allotment).ThenInclude(x => x.Yearly_reference)
                .Where(x => x.AppropriationId == 2 && x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result)
                .ToListAsync();


            budget_allotment.SubAllotment = budget_allotment.SubAllotment.Concat(suballotmentsLastYr).ToList();

            var fundsourcesLastYr = await _context.FundSources
                .Where(x => x.AppropriationId == 2 && x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result)
                .Include(x => x.AllotmentClass)
                .ToListAsync();

            suballotmentsLastYr.ForEach(x => x.AppropriationId = 2);
            fundsourcesLastYr.ForEach(x => x.AppropriationId = 2);

            budget_allotment.FundSources = budget_allotment.FundSources.Concat(fundsourcesLastYr).ToList();
            //budget_allotment.FirstOrDefault().SubAllotment = budget_allotment.FirstOrDefault().SubAllotment.Concat(suballotmentsLastYr).ToList();

            var CurrentYrAllotment_beginningBalance = _context.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Beginning_balance) +
                                                      _context.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.Beginning_balance) +
                                                      _context.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Beginning_balance) +
                                                      _context.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Beginning_balance);
            ViewBag.CurrentYrAllotment_beginningBalance = CurrentYrAllotment_beginningBalance.ToString("C", new CultureInfo("en-PH"));

            var CurrentYrAllotment_remainingBalance = _context.FundSources.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(x => x.Remaining_balance) +
                                                      _context.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance) +
                                                      _context.SubAllotment.Where(x => x.BudgetAllotmentId == YearlyRefId).Sum(s => s.Remaining_balance) +
                                                      _context.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance);
            ViewBag.CurrentYrAllotment_remainingBalance = CurrentYrAllotment_remainingBalance.ToString("C", new CultureInfo("en-PH"));

            var CurrentYrAllotment_realignmentAmount = _context.SubAllotment.Where(x => x.Budget_allotment.Yearly_reference.YearlyReference == year).Sum(x => x.realignment_amount) + _context.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == year).Sum(x => x.realignment_amount);
            ViewBag.CurrentYrAllotment_realignmentAmount = CurrentYrAllotment_realignmentAmount.ToString("C", new CultureInfo("en-PH"));

            var CurrentYrAllotment_obligatedAmount = _context.FundSources.Where(x => x.BudgetAllotment.Yearly_reference.YearlyReference == year).Sum(x => x.obligated_amount)
                                                   + _context.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.obligated_amount)
                                                   + _context.SubAllotment.Where(x => x.Budget_allotment.Yearly_reference.YearlyReference == year).Sum(x => x.obligated_amount)
                                                   + _context.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.obligated_amount);


            ViewBag.CurrentYrAllotment_obligatedAmount = CurrentYrAllotment_obligatedAmount.ToString("C", new CultureInfo("en-PH"));

            var previousAllot = _context.FundSources.Where(x => x.IsAddToNextAllotment == true && x.BudgetAllotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance) + _context.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Remaining_balance);
            ViewBag.PreviousAllot = previousAllot.ToString("C", new CultureInfo("en-PH"));

            var previousAllotConapSaa =  _context.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId).Sum(x => x.Remaining_balance);
            ViewBag.previousAllotConap = previousAllotConapSaa.ToString("C", new CultureInfo("en-PH"));

            return View(budget_allotment);
        }

        // GET: Budget_allotments/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            PopulatePrexcsDropDownList();
            PopulateYrDropDownList();
            PopulateAllotDropDownList();
            PopulateAllotmentClassDropDownList();

            return View();
        }

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _context.AllotmentClass
                                   orderby d.Allotment_Class
                                   select d;
            ViewBag.PrexcId = new SelectList((from s in _context.AllotmentClass.ToList()
                                              select new
                                              {
                                                  PrexcId = s.Id,
                                                  prexc = s.Allotment_Class
                                              }),
                                       "PrexcId",
                                       "prexc",
                                       null);                    
        }

        private void PopulatePsDropDownList()
        {
            ViewBag.pi_userid = new SelectList((from s in _pis_context.allPersonalInformation()
                                                where !_context.Requesting_office.Any(ro => ro.pi_userid == s.userid)
                                                select new
                                                {
                                                    pi_userid = s.userid,
                                                    ps = s.full_name
                                                }),
                                          "pi_userid",
                                          "ps",
                                           null);

        }

        private void PopulateYrDropDownList(object selectedPrexc = null)
        {
          
            var prexsQuery = from d in _context.Yearly_reference
                             orderby d.YearlyReference
                             select d;

            ViewBag.YearlyReferenceId = new SelectList((from s in _context.Yearly_reference.ToList()
                                                        where !_context.Budget_allotments.Any(ro => ro.YearlyReferenceId == s.YearlyReferenceId)
                                                        select new
                                                        {
                                                            YearlyReferenceId = s.YearlyReferenceId,
                                                            yr = s.YearlyReference
                                                        }),
                                         "YearlyReferenceId", "yr"
                                         );

            List<Yearly_reference> oh = new List<Yearly_reference>();

            oh = (from c in _osContext.Yearly_reference select c).ToList();
            oh.Insert(0, new Yearly_reference { YearlyReferenceId = 0, YearlyReference = "--Select Year--" });

            ViewBag.message = oh;


        }

        private void PopulateAllotDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _context.AllotmentClass
                                   orderby d.Allotment_Class
                                   select d;
            ViewBag.AllotmentClassId = new SelectList((from s in _context.AllotmentClass.ToList()
                                              select new
                                              {
                                                  Id = s.Id,
                                                  Allotment_Class = s.Allotment_Class
                                              }),
                                       "Id",
                                       "Allotment_Class",
                                       null);

        }

        private void PopulateAllotmentClassDropDownList()
        {
            ViewBag.AllotmentClassId = new SelectList((from s in _context.AllotmentClass.ToList()
                                              select new
                                              {
                                                  AllotmentClassId = s.Id,
                                                  AllotmentClass = s.Allotment_Class
                                              }),
                                    "AllotmentClassId",
                                    "AllotmentClass",
                                    null);
        }



        // POST: Budget_allotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BudgetAllotment budget_allotment)
        {
            _BudgetAllotmentContext.Add(budget_allotment);
            _BudgetAllotmentContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Budget_allotments/Details/5
        public async Task<IActionResult> Suballotment(int budget_id, float SubsTotal)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            var sub_Allotment = _context.SubAllotment.Where(s => s.BudgetAllotmentId == budget_id);
            ViewBag.beginning_balance = sub_Allotment.Sum(x => x.Beginning_balance).ToString("C", new CultureInfo("en-PH"));
            ViewBag.obligated_amount = sub_Allotment.Sum(x => x.obligated_amount).ToString("C", new CultureInfo("en-PH"));
            ViewBag.remaining_balance = sub_Allotment.Sum(x => x.Remaining_balance).ToString("C", new CultureInfo("en-PH"));

            //START Query of the amounts
            var subquery = _context.SubAllotment
                .Select(x => new Suballotment_amount
                {
                    SubAllotmentAmountId = x.prexcId,
                    beginning_balance = _context.Suballotment_amount.Where(i => i.SubAllotmentId == x.SubAllotmentId).Select(x => x.beginning_balance).Sum()
                });

            ViewBag.Query = subquery.ToList();

            //END Sum of the amounts

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Personalinfo_userid = "--Select ORS Head--" });

            ViewBag.message = oh;
            ViewBag.budget_id = budget_id;

            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.SubAllotment)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.BudgetAllotmentId == budget_id);

            return View(budget_allotment);
        }

        // GET: Budget_allotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);

            PopulateYrDropDownList(budget_allotment.YearlyReferenceId);
            if (budget_allotment == null)
            {
                return NotFound();
            }
            return View(budget_allotment);
        }

        // POST: Budget_allotments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( BudgetAllotment budget_allotment)
        {
            var Budget = await _BudgetAllotmentContext.BudgetAllotment.Where(x => x.BudgetAllotmentId == budget_allotment.BudgetAllotmentId).AsNoTracking().FirstOrDefaultAsync();
            Budget.Allotment_series = budget_allotment.Allotment_series;
            Budget.Allotment_title = budget_allotment.Allotment_title;
            Budget.Allotment_code = budget_allotment.Allotment_code;

            _BudgetAllotmentContext.Update(Budget); 
            await _BudgetAllotmentContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Budget_allotments/Delete/5
        public async Task<IActionResult> Delete(int? BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            if (BudgetAllotmentId == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == BudgetAllotmentId);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        // POST: Budget_allotments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var budget_allotment = await _context.Budget_allotments.FindAsync(BudgetAllotmentId);
            _context.Budget_allotments.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool Budget_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            return _context.Budget_allotments.Any(e => e.BudgetAllotmentId == id);
        }

        #region APIs
        [HttpPost]
        public async Task<ActionResult> CheckNextYear(int allotmentClassId, int appropriationId, int budgetAllotmentId, bool addToNext)
        {
            var subAllotments = await _context.SubAllotment.Where(x => x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationId && x.BudgetAllotmentId == budgetAllotmentId).ToListAsync();

            subAllotments.ForEach(x=>x.IsAddToNextAllotment = addToNext);

            _context.UpdateRange(subAllotments);
            await _context.SaveChangesAsync();


            return Ok(_context.SaveChangesAsync());
        }

        [HttpPost]
        public async Task<ActionResult> FundCheckNextYear(int allotmentClassId, int appropriationId, int budgetAllotmentId, bool addToNext)
        {
            var fundSources = await _context.FundSources.Where(x => x.AllotmentClassId == allotmentClassId && x.AppropriationId == appropriationId && x.BudgetAllotmentId == budgetAllotmentId).ToListAsync();

            fundSources.ForEach(x => x.IsAddToNextAllotment = addToNext);

            _context.UpdateRange(fundSources);
            await _context.SaveChangesAsync();


            return Ok(_context.SaveChangesAsync());
        }

        /*[HttpPost]
        public async Task<ActionResult> CheckNextYear(int subAllotmentId, bool addToNext)
        {
            var subAllotments = await _context.SubAllotment.FindAsync(subAllotmentId);

            subAllotments.IsAddToNextAllotment = addToNext;

            _context.Update(subAllotments);
            await _context.SaveChangesAsync();


            return Ok(await _context.SaveChangesAsync());
        }*/
        #endregion

        #region COOKIES

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        #endregion
    }
}