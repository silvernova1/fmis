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
    public class Obligated_amountController : Controller
    {
        private readonly Obligated_amountContext _context;
        private readonly Obligated_amountContext _dbContext;

        public Obligated_amountController(Obligated_amountContext context, Obligated_amountContext dbContext)
        {
            _context = context;
            _dbContext = dbContext;
        }

        // GET: Obligated_amount
        public async Task<IActionResult> Index()
        {

            return View(await _context.Obligated_amount.ToListAsync());
        }

        // GET: Obligated_amount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obligated_amount = await _context.Obligated_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligated_amount == null)
            {
                return NotFound();
            }
            return View(obligated_amount);
        }

        // GET: Obligated_amount/Create
        public async Task<IActionResult> CreateAsync()
        {
            TempData["Obligated_amount"] = await _dbContext.Obligated_amount.ToListAsync();
            // TempData["FundSource"] = await _context.FundSource.ToListAsync();
            return View();
        }

        // POST: Obligated_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Obligation_id,Expense_title,Code,Amount,Created_at,Updated_at")] Obligated_amount obligated_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligated_amount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obligated_amount);
        }

        // GET: Obligated_amount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligated_amount = await _context.Obligated_amount.FindAsync(id);
            if (obligated_amount == null)
            {
                return NotFound();
            }
            return View(obligated_amount);
        }

        // POST: Obligated_amount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Obligation_id,Expense_title,Code,Amount,Created_at,Updated_at")] Obligated_amount obligated_amount)
        {
            if (id != obligated_amount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obligated_amount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Obligated_amountExists(obligated_amount.Id))
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
            return View(obligated_amount);
        }

        // GET: Obligated_amount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligated_amount = await _context.Obligated_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligated_amount == null)
            {
                return NotFound();
            }

            return View(obligated_amount);
        }

        // POST: Obligated-amount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obligated_amount = await _context.Obligated_amount.FindAsync(id);
            _context.Obligated_amount.Remove(obligated_amount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Obligated_amountExists(int id)
        {
            return _context.Obligated_amount.Any(e => e.Id == id);
        }
    }
}
