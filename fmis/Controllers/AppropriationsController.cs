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
    public class AppropriationsController : Controller
    {
        private readonly AppropriationContext _context;

        public AppropriationsController(AppropriationContext context)
        {
            _context = context;
        }

        // GET: Appropriations
        public async Task<IActionResult> Index()
        {
            ViewBag.Layout = "_Layout";
            return View(await _context.Appropriation.ToListAsync());
        }

        // GET: Appropriations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appropriation = await _context.Appropriation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appropriation == null)
            {
                return NotFound();
            }

            ViewBag.Layout = "_Layout";
            return View(appropriation);
        }

        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.Layout = "_Layout";
            return View();
        }

        // POST: Appropriations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Code,Created_at,Updated_at")] Appropriation appropriation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appropriation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Layout = "_Layout";
            return View(appropriation);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appropriation = await _context.Appropriation.FindAsync(id);
            if (appropriation == null)
            {
                return NotFound();
            }
            ViewBag.Layout = "_Layout";
            return View(appropriation);
        }

        // POST: Appropriations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Code,Created_at,Updated_at")] Appropriation appropriation)
        {
            if (id != appropriation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appropriation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppropriationExists(appropriation.Id))
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
            ViewBag.Layout = "_Layout";
            return View(appropriation);
        }

        // GET: Appropriations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appropriation = await _context.Appropriation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appropriation == null)
            {
                return NotFound();
            }

            ViewBag.Layout = "_Layout";
            return View(appropriation);
        }

        // POST: Appropriations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appropriation = await _context.Appropriation.FindAsync(id);
            _context.Appropriation.Remove(appropriation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppropriationExists(int id)
        {
            return _context.Appropriation.Any(e => e.Id == id);
        }
    }
}
