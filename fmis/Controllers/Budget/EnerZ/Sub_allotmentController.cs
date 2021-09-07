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
    public class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;
        private readonly Ors_headContext _Context;

        public Sub_allotmentController(Sub_allotmentContext context, Ors_headContext Context)
        {
            _context = context;
            _Context = Context;
        }

        public class Sub_allotmentData
        {
            public int Id { get; set; }
            public int Prexe_code { get; set; }
            public string Suballotment_code { get; set; }
            public string Suballotment_title { get; set; }
            public int Ors_head { get; set; }
            public string Responsibility_number { get; set; }
            public string Description { get; set; }
        }
        // GET: 
        public async Task<IActionResult> Index(int? id)
        {

            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Sub_allotment.ToList());
            ViewBag.temp = json;


            /*List<Budget_allotment> item = _context.Budget_allotment.Include(f => f.FundSources).ToList();*/
            /*IList<FundSource> item = _Context.FundSource.Include(f => f.Budget_allotment).ToList();
            return View(item);*/
            /*var item = _context.Budget_allotment.FromSqlRaw("Select * from Budget_Allotment")
                  .ToList();*/
            /*var item = _context.Budget_allotment.Include(f => f.FundSources);*/





            return View(await _context.Sub_allotment.ToListAsync());

        }

        // GET: Sub_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var Sub = await _context.Sub_allotment
                .Include(s => s.Ors_head)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Sub == null)
            {

                return NotFound();
            }

            return View(Sub);
        }

        // GET: Sub_allotment/Create
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
        public IActionResult saveObligation(List<Sub_allotmentData> data)
        {
            var Sub = new List<Sub_allotment>();
            var Allotment = new Sub_allotment();


            foreach (var item in data)
            {
                Allotment.Prexe_code = item.Prexe_code;
                Allotment.Suballotment_code = item.Suballotment_code;
                Allotment.Suballotment_title = item.Suballotment_title;
                Allotment.Ors_head = item.Ors_head;
                Allotment.Responsibility_number = item. Responsibility_number;
                Allotment.Description = item.Description;

                Sub.Add(Allotment);
            }

            ViewBag.layout = "_Layout";
            this._context.Sub_allotment.Add(Allotment);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Sub_allotmnent/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Prexe_code,Suballotment_code,Suballotment_title,Ors_head,Responsibility_number,Description")] Sub_allotment Allotment)
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

        public ActionResult AddSub_allotment(IEnumerable<Budget_allotment> SubInput)

        {

            var p = SubInput;
            return null;

        }

        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Allotment = await _context.Sub_allotment.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prexe_code,Suballotment_code,Suballotment_title,Ors_head,Responsibility_number,Description")] Sub_allotment Allotment)
        {
            if (id != Allotment.Id)
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
                    if (!Sub_allotmentExists(Allotment.Id))
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

            var Allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var Allotment = await _context.Sub_allotment.FindAsync(id);
            _context.Sub_allotment.Remove(Allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sub_allotmentExists(int id)
        {
            ViewBag.layout = "_Layout";
            return _context.Sub_allotment.Any(e => e.Id == id);
        }
    }
}

