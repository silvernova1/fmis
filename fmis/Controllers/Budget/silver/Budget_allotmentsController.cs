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


namespace fmis.Controllers
{
    public class Budget_allotmentsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly FundSourceContext _Context;
        private readonly Yearly_referenceContext _osContext;
        private readonly Ors_headContext _orssContext;


        public Budget_allotmentsController(MyDbContext context, FundSourceContext Context, Yearly_referenceContext osContext, Ors_headContext orssContext)
        {
            _context = context;
            _Context = Context;
            _osContext = osContext;
            _orssContext = orssContext;
        }

        // GET: Budget_allotments
        public async Task<IActionResult> Index()
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            ViewBag.layout = "_Layout";
            var ballots = _context.Budget_allotments
            .Include(c => c.Yearly_reference)
            .AsNoTracking();
            return View(await ballots.ToListAsync());
            /* return View("~/Views/silver/Budget_allotments/Index.cshtml");*/
            /*return View("~/Views/silver/Budget_allotments/Index.cshtml", await ballots.ToListAsync());*/
        }

        // GET: Budget_allotments/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            PopulateYrDropDownList();
            return View();
        }

        private void PopulateYrDropDownList(object selectedPrexc = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
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
                                         "YearlyReferenceId","yr"
                                         );

            List<Yearly_reference> oh = new List<Yearly_reference>();

            oh = (from c in _osContext.Yearly_reference select c).ToList();
            oh.Insert(0, new Yearly_reference { YearlyReferenceId = 0, YearlyReference = "--Select Year--" });

            ViewBag.message = oh;
        }
        // POST: Budget_allotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetAllotmentId,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at,YearlyReferenceId")] Budget_allotment budget_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            try
            {
                if (ModelState.IsValid)
                {
                    List<Prexc> p = new List<Prexc>();
                    _context.Add(budget_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException )
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateYrDropDownList(budget_allotment.YearlyReferenceId);
            return View(budget_allotment);
        }


        // GET: Budget_allotments/Details/5
        public async Task<IActionResult> Fundsource(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            /*PopulateHeadDropDownList();*/

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Personalinfo_userid = "--Select ORS Head--" });

            ViewBag.message = oh;
            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .Include(s => s.Sub_allotments)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        public async Task<IActionResult> Suballotment(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            /*PopulateHeadDropDownList();*/

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Personalinfo_userid = "--Select ORS Head--" });

            ViewBag.message = oh;
            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .Include(s => s.Sub_allotments)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }


        // GET: Budget_allotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
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
        public async Task<IActionResult> Edit(int id, [Bind("YearlyReferenceId,BudgetAllotmentId,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id != budget_allotment.BudgetAllotmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budget_allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Budget_allotmentExists(budget_allotment.BudgetAllotmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(budget_allotment);
        }



        // GET: Budget_allotments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        // POST: Budget_allotments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
            _context.Budget_allotments.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool Budget_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return _context.Budget_allotments.Any(e => e.BudgetAllotmentId == id);
        }
    }
}
