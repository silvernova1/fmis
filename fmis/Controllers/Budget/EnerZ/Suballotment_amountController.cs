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

namespace fmis.Controllers
{
    public class Suballotment_amountController : Controller
    {
        private readonly Suballotment_amountContext _context;

        public Suballotment_amountController(Suballotment_amountContext context)
        {
            _context = context;
        }

        public class Suballotment_amountData
        {

            public int Expenses { get; set; }
            public float Amount { get; set; }
            public int Fund_source { get; set; }
        }

        // GET:  Suballotment_amount
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Suballotment_amount.ToList());
            ViewBag.temp = json;
            return View(await _context.Suballotment_amount.ToListAsync());
        }

        // GET:  Suballotment_amount/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET:  Suballotment_amount/Create
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
        public IActionResult saveSuballotment_amount(List<Suballotment_amountData> data)
        {
            var suballotment_amounts = new List<Suballotment_amount>();
            var suballotment_amount = new Suballotment_amount();


            foreach (var item in data)
            {

                suballotment_amount.Expenses = item.Expenses;
                suballotment_amount.Amount = item.Amount;
                suballotment_amount.Fund_source = item.Fund_source;

                suballotment_amounts.Add(suballotment_amount);
            }


            this._context.Suballotment_amount.Add(suballotment_amount);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Suballotment_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suballotment_amount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suballotment_amount);
        }

        [HttpPost]

        public ActionResult AddSuballotment_amount(IEnumerable<Suballotment_amount> Suballotment_amountInput)

        {

            var p = Suballotment_amountInput;
            return null;

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

        // POST:  Suballotment_amount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
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

        // GET:  Suballotment_amount/Delete/5
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

        // POST:  Suballotment_amount/Delete/5
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