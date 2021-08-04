using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;

namespace fmis.Controllers
{
    public class BudgetAllotmentsController : Controller
    {
        private readonly Obligated_amountContext _context;

        public BudgetAllotmentsController(Obligated_amountContext context)
        {
            _context = context;
        }

        // GET: BudgetAllotments
        public async Task<IActionResult> Index()
        {
            return View(await _context.BudgetAllotment.ToListAsync());
        }

        // GET: BudgetAllotments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetAllotment = await _context.BudgetAllotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetAllotment == null)
            {
                return NotFound();
            }

            return View(budgetAllotment);
        }

        // GET: BudgetAllotments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BudgetAllotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,Allotment_Series,Allotment_Tittle,Allotment_Code,Created_At,Updated_At")] BudgetAllotment budgetAllotment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(budgetAllotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budgetAllotment);
        }

        // GET: BudgetAllotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetAllotment = await _context.BudgetAllotment.FindAsync(id);
            if (budgetAllotment == null)
            {
                return NotFound();
            }
            return View(budgetAllotment);
        }

        // POST: BudgetAllotments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Allotment_Series,Allotment_Tittle,Allotment_Code,Created_At,Updated_At")] BudgetAllotment budgetAllotment)
        {
            if (id != budgetAllotment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budgetAllotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetAllotmentExists(budgetAllotment.Id))
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
            return View(budgetAllotment);
        }

        // GET: BudgetAllotments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetAllotment = await _context.BudgetAllotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetAllotment == null)
            {
                return NotFound();
            }

            return View(budgetAllotment);
        }

        // POST: BudgetAllotments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budgetAllotment = await _context.BudgetAllotment.FindAsync(id);
            _context.BudgetAllotment.Remove(budgetAllotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetAllotmentExists(int id)
        {
            return _context.BudgetAllotment.Any(e => e.Id == id);
        }
    }
}
