﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models.John;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;

namespace fmis.Controllers.Budget.John
{

    public class AllotmentClassesController : Controller
    {
        private readonly AllotmentClassContext _context;

        public AllotmentClassesController(AllotmentClassContext context)
        {
            _context = context;
        }

        // GET: AllotmentClasses
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass");
            ViewBag.layout = "_Layout";
            return View(await _context.AllotmentClass.ToListAsync());
        }

        // GET: AllotmentClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allotmentClass == null)
            {
                return NotFound();
            }

            return View(allotmentClass);
        }

        // GET: AllotmentClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllotmentClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Allotment_Class,Account_Code,Created_At,Updated_At")] AllotmentClass allotmentClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allotmentClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allotmentClass);
        }

        // GET: AllotmentClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass.FindAsync(id);
            if (allotmentClass == null)
            {
                return NotFound();
            }
            return View(allotmentClass);
        }

        // POST: AllotmentClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Allotment_Class,Account_Code")] AllotmentClass allotmentClass)
        {
            if (id != allotmentClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(allotmentClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllotmentClassExists(allotmentClass.Id))
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
            return View(allotmentClass);
        }

        
        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var allotmentClass = await _context.AllotmentClass.Where(p => p.Id == ID).FirstOrDefaultAsync();
            _context.AllotmentClass.Remove(allotmentClass);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /*// GET: AllotmentClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allotmentClass == null)
            {
                return NotFound();
            }

            return View(allotmentClass);
        }

        // POST: AllotmentClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allotmentClass = await _context.AllotmentClass.FindAsync(id);
            _context.AllotmentClass.Remove(allotmentClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool AllotmentClassExists(int id)
        {
            return _context.AllotmentClass.Any(e => e.Id == id);
        }
    }
}
