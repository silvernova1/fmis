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


namespace fmis.Controllers.Budget.John
{
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _context;
        private readonly UacsContext _uContext;
        private readonly Budget_allotmentContext _bContext;
        private readonly PrexcContext _pContext;

        public FundSourceController(FundSourceContext context, UacsContext uContext, Budget_allotmentContext bContext, PrexcContext pContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
        }


        // GET: FundSource
        public async Task<IActionResult> Index(int? id)
        {



            /*List<FundSource> item = _context.FundSource.Include(f => f.Budget_allotment).ToList();*/
            /* var item = _context.FundSource.FromSqlRaw("Select * from FundSource")
                   .ToList();
             return View(item);*/

            /* return View(await _context.FundSource
                 .Include(s => s.Budget_allotment)
                 .Where(m => m.Budget_allotmentBudgetAllotmentId == id)
                 .ToListAsync());*/

            return View( await _context.FundSource.ToListAsync());


            
        }

        // GET: FundSource/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource
                .FirstOrDefaultAsync(m => m.FundSourceId == id);
            if (fundSource == null)
            {
                return NotFound();
            }

            return View(fundSource);
        }

        // GET: FundSource/Create
        public IActionResult Create(int? id)
        {
            ViewBag.BudgetId = id;

            List<Prexc> p = new List<Prexc>();

            p = (from c in _pContext.Prexc select c).ToList();
            p.Insert(0, new Prexc { Id = 0, pap_title  = "--Select PREXC--" });

            ViewBag.message = p;
            //TempData["Uacs"] = await _dbContext.Uacs.ToListAsync();
            //var item = await _bContext.Budget_allotment.Where(b => b.BudgetAllotmentId == 1).Select(b => b.BudgetAllotmentId).SingleOrDefaultAsync();
            //TempData["FundSource"] = await _context.FundSource.ToListAsync();

            /*
                        Budget_allotment budget_allotment = new Budget_allotment()
                        {
                            BudgetAllotmentId = 7
                        };*/


            //var FundSource = await _bContext.Budget_allotment.(u => u.BudgetAllotmentId).Select(u => u.BudgetAllotmentId).FirstOrDefaultAsync();

            //FundSource FundSource = _context.FundSource.Include(p => p.Budget_allotment).Where(p => p.Budget_allotment.BudgetAllotmentId == id).FirstOrDefault();


            return View();
        }

        // POST: FundSource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundSourceId,PrexcCode,FundSourceTitle,Description,FundSourceTitleCode,Respo,Budget_allotmentBudgetAllotmentId")] FundSource fundSource)
        {


            if (ModelState.IsValid)
            {

                _context.Add(fundSource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fundSource);
        }

        // GET: FundSource/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource.FindAsync(id);
            if (fundSource == null)
            {
                return NotFound();
            }
            return View(fundSource);
        }

        // POST: FundSource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FundSourceId,PrexcCode,FundSourceTitle,Description,FundSourceTitleCode,Respo")] FundSource fundSource)
        {
            if (id != fundSource.FundSourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fundSource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundSourceExists(fundSource.FundSourceId))
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
            return View(fundSource);
        }

        // GET: FundSource/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fundSource = await _context.FundSource.FindAsync(id);
            _context.FundSource.Remove(fundSource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FundSourceExists(int id)
        {
            return _context.FundSource.Any(e => e.FundSourceId == id);
        }
    }
}
