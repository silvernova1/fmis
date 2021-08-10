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
    public class UtilizationController : Controller
    {
        /*private readonly UtilizationContext _context;

        public UtilizationController(UtilizationContext context)
        {
            _context = context;
        }*/

        public class UtilizationData
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Dv { get; set; }
            public string Pr_no { get; set; }
            public string Po_no { get; set; }
            public string Payer { get; set; }
            public string Address { get; set; }
            public string Particulars { get; set; }
            public int Ors_no { get; set; }
            public string Fund_source { get; set; }
            public float Gross { get; set; }
            public int Created_by { get; set; }
            public DateTime Date_recieved { get; set; }
            public DateTime Time_recieved { get; set; }
            public DateTime Date_released { get; set; }
            public DateTime Time_released { get; set; }
        }

        private UtilizationContext _context { get; }

        public UtilizationController(UtilizationContext context)
        {
            this._context = context;
        }

        // GET: Utilization
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Utilization.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult saveUtilization(UtilizationData utilization_data)
        {
            var utilizationes = new List<Utilization>();
            var utilization = new Utilization();

            utilization.Id = 12312312;
            utilization.Date = DateTime.Now;
            utilization.Dv = "hahahaha";
            utilization.Pr_no = "hahahaha";
            utilization.Po_no = "hahahaha";
            utilization.Payer = "hahahaha";
            utilization.Address = "hahahaha";
            utilization.Particulars = "hahahaha";
            utilization.Ors_no = 12312312;
            utilization.Fund_source = "hahahaha";
            utilization.Gross = 12312312;
            utilization.Created_by = 12312312;
            utilization.Date_recieved = DateTime.Now;
            utilization.Time_recieved = DateTime.Now;
            utilization.Date_released = DateTime.Now;
            utilization.Time_released = DateTime.Now;
            utilizationes.Add(utilization);

            this._context.Utilization.Add(utilization);
            this._context.SaveChanges();
            return Json(utilization_data);
        }

        // GET: Utilization/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilization == null)
            {
                return NotFound();
            }

            return View(utilization);
        }

        // GET: Utilization/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilization/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payer,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Utilization utilization)
        {
        
            if (ModelState.IsValid)
            {
                _context.Add(utilization);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
        }

        // GET: Utilization/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization.FindAsync(id);
            if (utilization == null)
            {
                return NotFound();
            }
            return View(utilization);
        }

        // POST: Utilization/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Dv,Pr_no,Po_no,Payer,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Utilization utilization)
        {
            if (id != utilization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizationExists(utilization.Id))
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
            return View(utilization);
        }

        // GET: Utilization/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilization == null)
            {
                return NotFound();
            }

            return View(utilization);
        }

        // POST: Utilization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilization = await _context.Utilization.FindAsync(id);
            _context.Utilization.Remove(utilization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizationExists(int id)
        {
            return _context.Utilization.Any(e => e.Id == id);
        }
    }
}
