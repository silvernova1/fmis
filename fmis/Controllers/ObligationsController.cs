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
    public class ObligationsController : Controller
    {
        private readonly ObligationContext _context;

        public ObligationsController(ObligationContext context)
        {
            _context = context;
        }

        // GET: Obligations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Obligation.ToListAsync());
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View(obligation);
        }

        // GET: Obligations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obligation);
        }

        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation.FindAsync(id);
            if (obligation == null)
            {
                return NotFound();
            }
            return View(obligation);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
        {
            if (id != obligation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obligation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObligationExists(obligation.Id))
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
            return View(obligation);
        }

        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View(obligation);
        }

        // POST: Obligations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obligation = await _context.Obligation.FindAsync(id);
            _context.Obligation.Remove(obligation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObligationExists(int id)
        {
            return _context.Obligation.Any(e => e.Id == id);
        }
    }
}
