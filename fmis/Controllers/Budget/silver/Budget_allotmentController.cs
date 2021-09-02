using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Data.John;
using fmis.Models;
using fmis.Models.John;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Sitecore.FakeDb;
using System.Dynamic;
using fmis.ViewModel;

namespace fmis.Controllers
{
    public class Budget_allotmentController : Controller
    {
        private readonly Budget_allotmentContext _context;
        private readonly FundSourceContext _Context;

        public Budget_allotmentController(Budget_allotmentContext context, FundSourceContext Context)
        {
            _context = context;
            _Context = Context;
        }

        public class Budget_allotmentData
        {
            [DataType(DataType.Date)]
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
        public async Task<IActionResult> Index(int? id)
        {

            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Budget_allotment.ToList());
            ViewBag.temp = json;


            /*List<Budget_allotment> item = _context.Budget_allotment.Include(f => f.FundSources).ToList();*/
            /*IList<FundSource> item = _Context.FundSource.Include(f => f.Budget_allotment).ToList();
            return View(item);*/
            /*var item = _context.Budget_allotment.FromSqlRaw("Select * from Budget_Allotment")
                  .ToList();*/
            /*var item = _context.Budget_allotment.Include(f => f.FundSources);*/


            


            return View(await _context.Budget_allotment.ToListAsync());

        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
  


            if (id == null)
            {
                
                return NotFound();
            }

            var Budget = await _context.Budget_allotment
                .Include(s => s.FundSources)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);

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
            ViewBag.layout = "_Layout";
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
                Allotment.Year = item.Year;
                Allotment.Allotment_series = item.Allotment_series;
                Allotment.Allotment_title = item.Allotment_title;
                Allotment.Allotment_code = item.Allotment_code;
                Allotment.Created_at = item.Created_at;
                Allotment.Updated_at = item.Updated_at;

                Budget.Add(Allotment);
            }

            ViewBag.layout = "_Layout";
            this._context.Budget_allotment.Add(Allotment);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment Allotment)
        {
            if (ModelState.IsValid)
            {

                _context.Add(Allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.layout = "_Layout";
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
            if (id == null)
            {
                return NotFound();
            }

            var Allotment = await _context.Budget_allotment.FindAsync(id);
            if (Allotment == null)
            {
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(Allotment);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment Allotment)
        {
            if (id != Allotment.BudgetAllotmentId)
            {
                ViewBag.layout = "_Layout";
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
                    if (!Budget_allotmentExists(Allotment.BudgetAllotmentId))
                    {
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
            return View(Allotment);
        }

        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var Allotment = await _context.Budget_allotment
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (Allotment == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            ViewBag.layout = "_Layout";
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
            return _context.Budget_allotment.Any(e => e.BudgetAllotmentId == id);
        }
    }
}
