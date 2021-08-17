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
    public class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;

        public Sub_allotmentController(Sub_allotmentContext context)
        {
            _context = context;
        }

        public class Sub_allotmentData
        {

            public int Prexe_code { get; set; }
            public string Suballotment_code { get; set; }
            public string Suballotment_title { get; set; }
            public int Orc_head { get; set; }
            public string Responsibility_number { get; set; }
            public string Description { get; set; }
        }

        // GET: Sub_allotment
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Sub_allotment.ToList());
            ViewBag.temp = json;
            return View(await _context.Sub_allotment.ToListAsync());
        }

        // GET: Sub_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // GET: Sub_allotment/Create
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
        public IActionResult saveSub_allotment(List<Sub_allotmentData> data)
        {
            var sub_allotments = new List<Sub_allotment>();
            var sub_allotment = new Sub_allotment();


            foreach (var item in data)
            {

                sub_allotment.Prexe_code = item.Prexe_code;
                sub_allotment.Suballotment_code = item.Suballotment_code;
                sub_allotment.Suballotment_title = item.Suballotment_title;
                sub_allotment.Orc_head = item.Orc_head;
                sub_allotment.Responsibility_number = item.Responsibility_number;
                sub_allotment.Description = item.Description;
                sub_allotments.Add(sub_allotment);
            }


            this._context.Sub_allotment.Add(sub_allotment);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST:Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Prexe_code,Suballotment_code,Suballotment_title,Ors_head,Responsibility_number,Description")] Sub_allotment sub_allotment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sub_allotment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sub_allotment);
        }

        [HttpPost]

        public ActionResult AddSub_allotment(IEnumerable<Sub_allotment> Sub_allotmentInput)

        {

            var p = Sub_allotmentInput;
            return null;

        }

        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            return View(sub_allotment);
        }

        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Prexe_code,Suballotment_code,Suballotment_title,Ors_head,Responsibility_number,Description")] Sub_allotment sub_allotment)
        {
            if (id != sub_allotment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_allotmentExists(sub_allotment.Id))
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
            return View(sub_allotment);
        }

        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            _context.Sub_allotment.Remove(sub_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Sub_allotmentExists(int id)
        {
            return _context.Sub_allotment.Any(e => e.Id == id);
        }
    }
}