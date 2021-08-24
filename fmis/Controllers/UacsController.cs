using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Rotativa.AspNetCore;

namespace fmis.Controllers
{

    public class UacsController : Controller
    {
        private readonly UacsContext _context;

        public UacsController(UacsContext context)
        {
            _context = context;
        }



        public class UacsData
        {

            public string Account_title { get; set; }
            public string Expense_code { get; set; }
        }

        // GET: Obligations
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Uacs.ToList());
            ViewBag.temp = json;
            return View("~/Views/Uacs/Index.cshtml");
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await _context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // GET: Obligations/Create
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult AddData(List<string[]> dataListFromTable)
        {
            var dataListTable = dataListFromTable;
            return Json("Response, Data Received Successfully");
        }

        [HttpPost]
        public IActionResult SaveUacs(List<UacsData> data)
        {
            var uacses = new List<Uacs>();
            var uacs = new Uacs();


            foreach (var item in data)
            {
                uacs.Account_title = item.Account_title;
                uacs.Expense_code = item.Expense_code;

                uacses.Add(uacs);
            }


            this._context.Uacs.Update(uacs);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Account_title,Expense_code")] Uacs uacs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uacs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uacs);
        }

        [HttpPost]

        public ActionResult AddUacs(IEnumerable<Uacs> UacsInput)

        {

            var p = UacsInput;
            return null;

        }

        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await _context.Uacs.FindAsync(id);
            if (uacs == null)
            {
                return NotFound();
            }
            return View(uacs);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Account_title,Expense_code")] Uacs uacs)
        {
            if (id != uacs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uacs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObligationExists(uacs.Id))
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
            return View(uacs);
        }

        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await _context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // POST: Obligations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obligation = await _context.Uacs.FindAsync(id);
            _context.Uacs.Remove(obligation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObligationExists(int id)
        {
            return _context.Uacs.Any(e => e.Id == id);
        }
    }
}

