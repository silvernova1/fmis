using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Models.John;

namespace fmis.Controllers.Budget.John
{
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _context;

        public FundSourceController(FundSourceContext context)
        {
            _context = context;
        }

        // GET: FundSource
        public async Task<IActionResult> Index()
        {
            return View(await _context.FundSource.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: FundSource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundSourceId,PrexcCode,FundSourceTitle,Description,FundSourceTitleCode,Respo")] FundSource fundSource)
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
