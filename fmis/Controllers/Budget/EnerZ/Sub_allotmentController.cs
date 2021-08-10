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
    public class Sub_allotmentController : Controller
    {
        /*private readonly Sub_allotmentContext _context;

        public Sub_allotmentController(Sub_allotmentContext context)
        {
            _context = context;
        }*/

        public class Sub_allotmentData
        {
            public int Id { get; set; }
            public int Prexe_code { get; set; }
            public string Suballotment_code { get; set; }
            public string Suballotmenent_title { get; set; }
            public int Orc_head { get; set; }
            public string Responsibility_number { get; set; }
            public string Description { get; set; }
        }

        private Sub_allotmentContext _context { get; }

        public Sub_allotmentController(Sub_allotmentContext context)
        {
            this._context = context;
        }

        // GET: Sub_allotment
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Sub_allotment.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult saveSub_allotment(Sub_allotmentData Sub_allotment_data)
        {
            var sub_allotmentes = new List<Sub_allotment>();
            var sub_allotment = new Sub_allotment();

            sub_allotment.Id = 12312312;
            sub_allotment.Prexe_code = 12312312;
            sub_allotment.Suballotment_title = "hahahaha";
            sub_allotment.Suballotment_title = "hahahaha";
            sub_allotment.Orc_head = 12312312;
            sub_allotment.Responsibility_number = "hahahaha";
            sub_allotment.Description = "hahahaha";
            sub_allotmentes.Add(sub_allotment);

            this._context.Sub_allotment.Add(sub_allotment);
            this._context.SaveChanges();
            return Json(Sub_allotment_data);
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

        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create([Bind("Id,Prexc_code,Suballotment_code,Suballotment_title,Orc_head,Responsibility_number,Description")] Sub_allotment sub_Allotment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sub_Allotment);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
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

        // POST: Uacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prexc_code,Suballotment_code,Suballotment_title,Orc_head,Responsibility_number,Description")] Sub_allotment sub_Allotment)
        {
            if (id != sub_Allotment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_Allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_allotmentExists(sub_Allotment.Id))
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
            return View(sub_Allotment);
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
