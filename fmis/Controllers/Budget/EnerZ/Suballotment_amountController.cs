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
    public class Suballotment_amountController : Controller
    {
        /*private readonly Suballotment_amountContext _context;

        public Suballotment_amountController(Suballotment_amountContext context)
        {
            _context = context;
        }*/

        public class Suballotment_amountData
        {
            public int Id { get; set; }
            public int Expenses { get; set; }
            public float Amount { get; set; }
            public int Fund_source { get; set; }
        }

        private Suballotment_amountContext _context { get; }

        public Suballotment_amountController(Suballotment_amountContext context)
        {
            this._context = context;
        }

        // GET: Suballotment_amount
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Suballotment_amount.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult saveSuballotment_amount(Suballotment_amountData suballotment_amount_data)
        {
            var suballotment_amountes = new List<Suballotment_amount>();
            var suballotment_amount = new Suballotment_amount();



            suballotment_amount.Id = 12312312;
            suballotment_amount.Expenses = 12312312;
            suballotment_amount.Amount = 12312312;
            suballotment_amount.Fund_source = 12312312;
            suballotment_amountes.Add(suballotment_amount);

            this._context.Suballotment_amount.Add(suballotment_amount);
            this._context.SaveChanges();
            return Json(suballotment_amount_data);
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
            if (suballotment_amount == null)
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
        public string Create([Bind("Id,Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suballotment_amount);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
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
