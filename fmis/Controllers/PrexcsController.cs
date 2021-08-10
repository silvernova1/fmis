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
    public class PrexcsController : Controller
    {
        private readonly fmisContext _context;

        public PrexcsController(fmisContext context)
        {
            _context = context;
        }

        // GET: Prexcs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prexc.ToListAsync());
        }

        // GET: Prexcs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prexc == null)
            {
                return NotFound();
            }

            return View(prexc);
        }

        // GET: Prexcs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prexcs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,pap_code1,pap_code2")] Prexc prexc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prexc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prexc);
        }

        // GET: Prexcs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc.FindAsync(id);
            if (prexc == null)
            {
                return NotFound();
            }
            return View(prexc);
        }

        // POST: Prexcs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,pap_code1,pap_code2")] Prexc prexc)
        {
            if (id != prexc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prexc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrexcExists(prexc.Id))
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
            return View(prexc);
        }

        // GET: Prexcs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prexc == null)
            {
                return NotFound();
            }

            return View(prexc);
        }

        // POST: Prexcs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prexc = await _context.Prexc.FindAsync(id);
            _context.Prexc.Remove(prexc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrexcExists(int id)
        {
            return _context.Prexc.Any(e => e.Id == id);
        }
    }
}
