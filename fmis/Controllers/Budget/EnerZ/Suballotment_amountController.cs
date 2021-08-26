using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers
{

    public class Suballotment_amountController : Controller
    {
        private readonly Suballotment_amountContext _context;

        public Suballotment_amountController(Suballotment_amountContext context)
        {
            _context = context;
        }

        // GET: Suallotment_amouont
        public async Task<IActionResult> Index()
        {
            return View(await _context.Suballotment_amount.ToListAsync());
        }

        // GET: Suballotment_amount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suballotment_amount== null)
            {
                return NotFound();
            }

            return View(suballotment_amount);
        }

        // GET: Suballotment_amount/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suballotment_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Epenses,Amount,Fund_source")] Suballotment_amount suballotment_Amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suballotment_Amount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suballotment_Amount);
        }

        // GET: Suballotment_amount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount.FindAsync(id);
            if (suballotment_amount == null)
            {
                return NotFound();
            }
            return View(suballotment_amount);
        }

        // POST: Suballotment_amount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Epenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
        {
            if (id != suballotment_amount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suballotment_amount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Suballotment_amountExists(suballotment_amount.Id))
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
            return View(suballotment_amount);
        }

        // GET: Suballotment_amount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suballotment_amount == null)
            {
                return NotFound();
            }

            return View(suballotment_amount);
        }

        // POST: Suballotment_amount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suballotment_amount = await _context.Suballotment_amount.FindAsync(id);
            _context.Suballotment_amount.Remove(suballotment_amount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Suballotment_amountExists(int id)
        {
            return _context.Suballotment_amount.Any(e => e.Id == id);
        }
    }
}

