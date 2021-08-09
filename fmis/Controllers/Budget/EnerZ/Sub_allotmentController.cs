using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;

namespace fmis.Controllers.Budget.EnerZ
{
    public class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;

        public Sub_allotmentController(Sub_allotmentContext context)
        {
            _context = context;
        }

        // GET: Sub_allotment
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sub_allotment.ToListAsync());
        }

        // GET: Sub_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // GET: Sub_allotment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Prexe_code,Suballotment_code,Suballotmenent_title,Orc_head,Responsibility_number,Description")] Sub_allotment sub_allotment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sub_allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sub_allotment);
        }

        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            return View(sub_allotment);
        }

        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prexe_code,Suballotment_code,Suballotmenent_title,Orc_head,Responsibility_number,Description")] Sub_allotment sub_allotment)
        {
            if (id != sub_allotment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_allotmentExists(sub_allotment.Id))
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
            return View(sub_allotment);
        }

        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            _context.Sub_allotment.Remove(sub_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sub_allotmentExists(int id)
        {
            return _context.Sub_allotment.Any(e => e.Id == id);
        }
    }
}
