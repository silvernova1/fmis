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
    public class PrexcController : Controller
    {
        private readonly PrexcContext _context;

        public PrexcController(PrexcContext context)
        {
            _context = context;
        }

        public class PrexcData
        {
            public string pap_title { get; set; }
            public string pap_code1 { get; set; }
            public string pap_code2 { get; set; }
        }

        // GET: PrexcData
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Prexc.ToList());
            ViewBag.temp = json;
            return View(await _context.Prexc.ToListAsync());
        }

        // GET: Prexc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prexc == null)
            {
                return NotFound();
            }
            return View(prexc);
        }

        // GET: Prexc
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
        public IActionResult savePrexc(List<PrexcData> data)
        {
            var prexcs = new List<Prexc>();
            var prexc = new Prexc();

            foreach (var item in data)
            {
                prexc.pap_title = item.pap_title;
                prexc.pap_code1 = item.pap_code1;
                prexc.pap_code2 = item.pap_code2;
                prexcs.Add(prexc);
            }

            this._context.Prexc.Add(prexc);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST: Prexc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pap_title,Pap_code1,Pap_code2")] Prexc prexc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prexc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prexc);
        }

        [HttpPost]

        public ActionResult AddPrexc(IEnumerable<Prexc> PrexcInput)

        {

            var p = PrexcInput;
            return null;

        }

        // GET: Prexc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc.FindAsync(id);
            if (prexc == null)
            {
                return NotFound();
            }
            return View(prexc);
        }

        // POST: Prexc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Pap_title,Pap_code1,Pap_code2")] Prexc prexc)
        {
            if (id != prexc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prexc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrexcExists(prexc.Id))
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
            return View(prexc);
        }

        // GET: Prexc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var prexc = await _context.Prexc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prexc == null)
            {
                return NotFound();
            }
            return View(prexc);
        }

        // POST: Prexc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prexc = await _context.Prexc.FindAsync(id);
            _context.Prexc.Remove(prexc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool PrexcExists(int id)
        {
            return _context.Prexc.Any(e => e.Id == id);
        }
    }
}