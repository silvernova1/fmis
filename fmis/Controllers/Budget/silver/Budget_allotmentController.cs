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
    public class Budget_allotmentController : Controller
    {
        private readonly Budget_allotmentContext _context;

        public Budget_allotmentController(Budget_allotmentContext context)
        {
            _context = context;
        }

        public class Budget_allotmentData
        {
            [DataType(DataType.Date)]
            public int Id { get; set; }
            public string Year { get; set; }
            public string Allotment_series { get; set; }
            public string Allotment_title { get; set; }
            public string Allotment_code { get; set; }
            [DataType(DataType.Date)]
            public DateTime Created_at { get; set; }
            [DataType(DataType.Date)]
            public DateTime Updated_at { get; set; }
        }
        // GET: 
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Budget_allotment.ToList());
            ViewBag.temp = json;
            return View(await _context.Budget_allotment.ToListAsync());
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }

            var Budget = await _context.Budget_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Budget == null)
            {
                return NotFound();
            }
            return View(Budget);
        }

        // GET: Obligations/Create
        public IActionResult Create()
        {
            ViewBag.layout = "_Layout";
            return View();
        }
        public ActionResult AddData(List<string[]> dataListFromTable)
        {
            var dataListTable = dataListFromTable;
            return Json("Response, Data Received Successfully");
        }

        [HttpPost]
        public IActionResult saveObligation(List<Budget_allotmentData> data)
        {
            var Budget = new List<Budget_allotment>();
            var Allotment = new Budget_allotment();


            foreach (var item in data)
            {
                Allotment.Id = item.Id;
                Allotment.Year = item.Year;
                Allotment.Allotment_series = item.Allotment_series;
                Allotment.Allotment_title = item.Allotment_title;
                Allotment.Allotment_code = item.Allotment_code;
                Allotment.Created_at = item.Created_at;
                Allotment.Updated_at = item.Updated_at;

                Budget.Add(Allotment);
            }
            this._context.Budget_allotment.Add(Allotment);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment Allotment)
        {
            ViewBag.layout = "_Layout";
            if (ModelState.IsValid)
            {

                _context.Add(Allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Allotment);
        }

        [HttpPost]

        public ActionResult AddObligation(IEnumerable<Budget_allotment> BudgetInput)
        {
            var p = BudgetInput;
            return null;
        }
        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }
            var Allotment = await _context.Budget_allotment.FindAsync(id);
            if (Allotment == null)
            {
                return NotFound();
            }
            return View(Allotment);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment Allotment)
        {
            ViewBag.layout = "_Layout";
            if (id != Allotment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Budget_allotmentExists(Allotment.Id))
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
            return View(Allotment);
        }
        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }
            var Allotment = await _context.Budget_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Allotment == null)
            {
                return NotFound();
            }
            return View(Allotment);
        }
        // POST: Obligations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.layout = "_Layout";
            var Allotment = await _context.Budget_allotment.FindAsync(id);
            _context.Budget_allotment.Remove(Allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Budget_allotmentExists(int id)
        {
            ViewBag.layout = "_Layout";
            return _context.Budget_allotment.Any(e => e.Id == id);
        }
    }
}
