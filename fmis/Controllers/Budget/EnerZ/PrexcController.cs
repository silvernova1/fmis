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
    public class PrexcController : Controller
    {
        /*private readonly PrexcContext _context;

        public PrexcController(PrexcContext context)
        {
            _context = context;
        }*/

        public class PrexcData
        {
            public int Id { get; set; }
            public string pap_title { get; set; }
            public string pap_code1 { get; set; }
            public string pap_code2 { get; set; }
            public DateTime Created_at { get; set; }
            public DateTime Updated_at { get; set; }
        }

        private PrexcContext _context { get; }

        public PrexcController(PrexcContext context)
        {
            this._context = context;
        }

        // GET: Prexc
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Prexc.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult savePrexc(PrexcData prexc_data)
        {
            var prexces = new List<Prexc>();
            var prexc = new Prexc();



            prexc.Id = 12312312;
            prexc.pap_title = "hahahaha";
            prexc.pap_code1 = "hahahaha";
            prexc.pap_code2 = "hahahaha";
            prexc.Created_at = DateTime.Now;
            prexc.Updated_at = DateTime.Now;
            prexces.Add(prexc);

            this._context.Prexc.Add(prexc);
            this._context.SaveChanges();
            return Json(prexc_data);
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

        // GET: Prexc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prexc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create([Bind("Id,pap_title,pap_code1,pap_code2,Created_at,Updated_at")] Prexc prexc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prexc);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,pap_title,pap_code1,pap_code2,Created_at,Updated_at,Date_recieved")] Prexc prexc)
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
