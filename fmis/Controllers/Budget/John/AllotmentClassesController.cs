using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models.John;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;

namespace fmis.Controllers.Budget.John
{

    public class AllotmentClassesController : Controller
    {
        private readonly AllotmentClassContext _context;

        public AllotmentClassesController(AllotmentClassContext context)
        {
            _context = context;
        }

        // GET: AllotmentClasses
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass","");
            ViewBag.layout = "_Layout";
            return View(await _context.AllotmentClass.OrderBy(x=>x.Account_Code).ToListAsync());
        }

        // GET: AllotmentClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allotmentClass == null)
            {
                return NotFound();
            }

            return View(allotmentClass);
        }

        // GET: AllotmentClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllotmentClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Allotment_Class,Account_Code,Fund_Code,Desc")] AllotmentClass allotmentClass)
        {
            if (ModelState.IsValid)
            {
                if (allotmentClass.Allotment_Class == "PS")
                {
                    allotmentClass.Allotment_Class = "PS";
                    allotmentClass.Account_Code = "100";
                    allotmentClass.Fund_Code = "01";
                    allotmentClass.Desc = "Personnel Services";
                    _context.Add(allotmentClass);       
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }
                else if (allotmentClass.Allotment_Class == "MOOE")
                {
                    allotmentClass.Allotment_Class = "MOOE";
                    allotmentClass.Account_Code = "200";
                    allotmentClass.Fund_Code = "02";
                    allotmentClass.Desc = "Maintenance and Other Operating Expenses";
                    _context.Add(allotmentClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else if (allotmentClass.Allotment_Class == "CO")
                {
                    allotmentClass.Allotment_Class = "CO";
                    allotmentClass.Account_Code = "300";
                    allotmentClass.Fund_Code = "06";
                    allotmentClass.Desc = "Capital Outlay";
                    _context.Add(allotmentClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(allotmentClass);
        }

        // GET: AllotmentClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass.FindAsync(id);
            if (allotmentClass == null)
            {
                return NotFound();
            }
            return View(allotmentClass);
        }

        // POST: AllotmentClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AllotmentClass allotmentClass)
        {
            var allotment_class = await _context.AllotmentClass.Where(x => x.Id == allotmentClass.Id).AsNoTracking().FirstOrDefaultAsync();
            allotment_class.Allotment_Class = allotmentClass.Allotment_Class;
            allotment_class.Account_Code = allotmentClass.Account_Code;

            _context.Update(allotment_class); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var allotmentClass = await _context.AllotmentClass.Where(p => p.Id == ID).FirstOrDefaultAsync();
            _context.AllotmentClass.Remove(allotmentClass);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /*// GET: AllotmentClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allotmentClass = await _context.AllotmentClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allotmentClass == null)
            {
                return NotFound();
            }

            return View(allotmentClass);
        }

        // POST: AllotmentClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allotmentClass = await _context.AllotmentClass.FindAsync(id);
            _context.AllotmentClass.Remove(allotmentClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool AllotmentClassExists(int id)
        {
            return _context.AllotmentClass.Any(e => e.Id == id);
        }
    }
}
