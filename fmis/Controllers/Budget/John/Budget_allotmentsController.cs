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

namespace fmis.Controllers
{
    public class Budget_allotmentsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly FundSourceContext _Context;
        private readonly Ors_headContext _osContext;

        public Budget_allotmentsController(MyDbContext context, FundSourceContext Context, Ors_headContext osContext)
        {
            _context = context;
            _Context = Context;
            _osContext = osContext;
        }

        // GET: Budget_allotments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Budget_allotments.ToListAsync());
        }

        // GET: Budget_allotments/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _osContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Head_name = "--Select ORS Head--" });
            ViewBag.message = oh;

            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        // GET: Budget_allotments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budget_allotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            try
            {
                if (ModelState.IsValid)
            {
                _context.Add(budget_allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(budget_allotment);
        }

        // GET: Budget_allotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
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
            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
            _context.Budget_allotments.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Budget_allotmentExists(int id)
        {
            return _context.Budget_allotments.Any(e => e.BudgetAllotmentId == id);
        }
    }
}
