using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Filters;

namespace fmis.Controllers
{
    public class Yearly_referenceController : Controller
    {
        private readonly Yearly_referenceContext _context;
        private readonly MyDbContext _MyDbcontext;

        public Yearly_referenceController(Yearly_referenceContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }

        // GET: Yearly_reference
        [Route("YearlyReference")]
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference", "");
            return View(await _context.Yearly_reference.ToListAsync());
            
        }

        // GET: Yearly_reference/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var yearly_reference = await _context.Yearly_reference
                .FirstOrDefaultAsync(m => m.YearlyReferenceId == id);
            if (yearly_reference == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            ViewBag.layout = "_Layout";
            return View(yearly_reference);
        }

        // GET: Yearly_reference/Create
        public IActionResult Create()
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference","");
            return View();
        }

        // POST: Yearly_reference/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,YearlyReference")] Yearly_reference yearly_reference)
        {
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference", "");
            if (ModelState.IsValid)
            {
                _MyDbcontext.Add(yearly_reference);
                await _MyDbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.layout = "_Layout";
            return View(yearly_reference);
        }

        // GET: Yearly_reference/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference", "");
            if (id == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            var yearly_reference = await _context.Yearly_reference.FindAsync(id);
            if (yearly_reference == null)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }
            ViewBag.layout = "_Layout";
            return View(yearly_reference);
        }

        // POST: Yearly_reference/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,YearlyReference")] Yearly_reference yearly_reference)
        {
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference", "");

            if (id != yearly_reference.YearlyReferenceId)
            {
                ViewBag.layout = "_Layout";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yearly_reference);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Yearly_referenceExists(yearly_reference.YearlyReferenceId))
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
            return View(yearly_reference);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var yearlyReference = await _context.Yearly_reference.Where(p => p.YearlyReferenceId == ID).FirstOrDefaultAsync();
            _context.Yearly_reference.Remove(yearlyReference);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private bool Yearly_referenceExists(int id)
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "yearlyreference", "");
            return _context.Yearly_reference.Any(e => e.YearlyReferenceId == id);
        }
    }
}
