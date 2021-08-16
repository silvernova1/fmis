using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;

namespace fmis.Controllers.Budget.silver
{
    public class Requesting_officeController : Controller
    {
        private readonly Requesting_officeContext _context;

        public Requesting_officeController(Requesting_officeContext context)
        {
            _context = context;
        }

        // GET: Requesting_office
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            return View(await _context.Requesting_office.ToListAsync());
        }

        // GET: Requesting_office/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var requesting_office = await _context.Requesting_office
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requesting_office == null)
            {
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(requesting_office);
        }

        // GET: Requesting_office/Create
        public IActionResult Create()
        {
            ViewBag.layout = "_Layout";
            return View();
        }

        // POST: Requesting_office/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Head_name,Position,Created_at,Updated_at")] Requesting_office requesting_office)
        {
            if (ModelState.IsValid)
            {
                ViewBag.layout = "_Layout";
                _context.Add(requesting_office);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.layout = "_Layout";
            return View(requesting_office);
        }

        // GET: Requesting_office/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var requesting_office = await _context.Requesting_office.FindAsync(id);
            if (requesting_office == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(requesting_office);
        }

        // POST: Requesting_office/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Head_name,Position,Created_at,Updated_at")] Requesting_office requesting_office)
        {
            if (id != requesting_office.Id)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requesting_office);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Requesting_officeExists(requesting_office.Id))
                    {
                        ViewBag.layout = "_Layout";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.layout = "_Layout";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.layout = "_Layout";
            return View(requesting_office);
        }

        // GET: Requesting_office/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var requesting_office = await _context.Requesting_office
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requesting_office == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(requesting_office);
        }

        // POST: Requesting_office/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.layout = "_Layout";
            var requesting_office = await _context.Requesting_office.FindAsync(id);
            _context.Requesting_office.Remove(requesting_office);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Requesting_officeExists(int id)
        {
            ViewBag.layout = "_Layout";
            return _context.Requesting_office.Any(e => e.Id == id);
        }
    }
}
