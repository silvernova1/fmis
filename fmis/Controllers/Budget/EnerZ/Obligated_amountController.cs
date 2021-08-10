using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using System.Text.Json;
using System.IO;
using System.Text;

namespace fmis.Controllers
{
    public class Obligated_amountController : Controller
    {
        /*private readonly Obligated_amountContext _context;

        public Obligated_amountController(Obligated_amountContext context)
        {
            _context = context;
        }*/

        public class Obligated_amountData
        {
            public int Obligation_id { get; set; }
            public int Expenses_title { get; set; }
            public int Code { get; set; }
            public float Amount { get; set; }
            public DateTime Created_at { get; set; }
            public DateTime Updated_at { get; set; }
        }

        private Obligated_amountContext _context { get; }

        public  Obligated_amountController(Obligated_amountContext context)
        {
            this._context = context;
        }

        // GET: Obligated_amount
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Obligated_amount.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult saveObligated_amount(Obligated_amountData obligated_amount_data)
        {
            var obligated_amountes = new List<Obligated_amount>();
            var obligated_amount = new Obligated_amount();



            obligated_amount.Id = 12312312;
            obligated_amount.Obligation_id = 12312312;
            obligated_amount.Expenses_title = 12312312;
            obligated_amount.Code = 12312312;
            obligated_amount.Amount = 12312312;
            obligated_amount.Created_at = DateTime.Now;
            obligated_amount.Updated_at = DateTime.Now;
            obligated_amountes.Add(obligated_amount);
            this._context.Obligated_amount.Add(obligated_amount);
            this._context.SaveChanges();
            return Json(obligated_amount_data);
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

        // POST: Obligated_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create([Bind("Id,Obligation_id,Expense_title,Code,Created_at,Updated_at")] Obligated_amount obligated_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligated_amount);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Obligation_id,Expense_title,Code,Created_at,Updated_at,Date_recieved")] Obligated_amount obligated_amount
            )
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

        // GET: Uacs/Delete/5
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