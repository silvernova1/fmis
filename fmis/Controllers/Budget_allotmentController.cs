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
    public class Budget_allotmentController : Controller
    {
        private readonly Budget_allotmentContext _context;

        public Budget_allotmentController(Budget_allotmentContext context)
        {
            _context = context;
        }

        // GET: Budget_allotment
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            return View(await _context.Budget_allotment.ToListAsync());
        }

        // GET: Budget_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budget_allotment == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            ViewBag.layout = "_Layout";
            return View(budget_allotment);
        }

        // GET: Budget_allotment/Create
        public IActionResult Create()
        {
            ViewBag.layout = "_Layout";
            return View();
        }

        // POST: Budget_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(budget_allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.layout = "_Layout";
            return View(budget_allotment);
        }

        // GET: Budget_allotment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotment.FindAsync(id);
            if (budget_allotment == null)
            {
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(budget_allotment);
        }

        // POST: Budget_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            if (id != budget_allotment.Id)
            {
                ViewBag.layout = "_Layout";
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
                    if (!Budget_allotmentExists(budget_allotment.Id))
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
            ViewBag.layout = "_Layout";
            return View(budget_allotment);
        }

        // GET: Budget_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            ViewBag.layout = "_Layout";
            return View(budget_allotment);
        }

        // POST: Budget_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget_allotment = await _context.Budget_allotment.FindAsync(id);
            _context.Budget_allotment.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Budget_allotmentExists(int id)
        {
            ViewBag.layout = "_Layout";
            return _context.Budget_allotment.Any(e => e.Id == id);
        }
    }
}
