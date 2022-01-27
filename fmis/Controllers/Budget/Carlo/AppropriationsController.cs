using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Filters;

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
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
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

            return View(appropriation);
        }

        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Code")] Appropriation appropriation)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            if (ModelState.IsValid)
            {
                _context.Add(appropriation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appropriation);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            if (id == null)
            {
                return NotFound();
            }

            var appropriation = await _context.Appropriation.FindAsync(id);
            if (appropriation == null)
            {
                return NotFound();
            }
            return View(appropriation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Appropriation appropriation)
        {

            var appro = await _context.Appropriation.Where(x => x.Id == appropriation.Id).AsNoTracking().FirstOrDefaultAsync();
            appro.Description = appropriation.Description;
            appro.Code = appropriation.Code;

            _context.Update(appro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Appropriations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
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

            return View(appropriation);
        }

        // POST: Appropriations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
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
