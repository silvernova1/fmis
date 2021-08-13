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
    public class Obligated_amountController : Controller
    {
        private readonly Obligated_amountContext _context;

        public Obligated_amountController(Obligated_amountContext context)
        {
            _context = context;
        }

        public class Obligated_amountData
        {

            public int Obligation_id { get; set; }
            public int Expense_Title { get; set; }
            public int Code { get; set; }
            public float Amount { get; set; }
          
        }

        // GET: Obligated_amount
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Obligated_amount.ToList());
            ViewBag.temp = json;
            return View(await _context.Obligated_amount.ToListAsync());
        }

        // GET: Obligated_amount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligated_amount = await _context.Obligated_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligated_amount == null)
            {
                return NotFound();
            }

            return View(obligated_amount);
        }

        // GET: Obligated_amount/Create
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
        public IActionResult saveObligated_amount(List<Obligated_amountData> data)
        {
            var obligated_amounts = new List<Obligated_amount>();
            var obligated_amount = new Obligated_amount();


            foreach (var item in data)
            {
                obligated_amount.Obligation_id = item.Obligation_id;
                obligated_amount.Expense_Title = item.Expense_Title;
                obligated_amount.Code = item.Code;
                obligated_amount.Amount = item.Amount;
                obligated_amounts.Add(obligated_amount);
            }


            this._context.Obligated_amount.Add(obligated_amount);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Obligated_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Obligation_id,Expense_Title,Code,Amount")] Obligated_amount obligated_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligated_amount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obligated_amount);
        }

        [HttpPost]

        public ActionResult AddObligated_amount(IEnumerable<Obligated_amount> Obligated_amountInput)

        {

            var p = Obligated_amountInput;
            return null;

        }

        // GET: Obligated_amount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligated_amount = await _context.Obligated_amount.FindAsync(id);
            if (obligated_amount == null)
            {
                return NotFound();
            }
            return View(obligated_amount);
        }

        // POST: Obligated_amount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Obligation_id, Expense_Title, Code, Amount")] Obligated_amount obligated_amount)
        {
            if (id != obligated_amount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obligated_amount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Obligated_amountExists(obligated_amount.Id))
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
            return View(obligated_amount);
        }

        // GET: Obligated_amount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligated_amount = await _context.Obligated_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligated_amount == null)
            {
                return NotFound();
            }

            return View(obligated_amount);
        }

        // POST: Obligated_amount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obligated_amount = await _context.Obligated_amount.FindAsync(id);
            _context.Obligated_amount.Remove(obligated_amount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Obligated_amountExists(int id)
        {
            return _context.Obligated_amount.Any(e => e.Id == id);
        }
    }
}