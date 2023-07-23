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
    [Authorize(Policy = "BudgetAdmin")]


    
    public class AllotmentClassesController : Controller
    {
        private readonly MyDbContext _context;

        public AllotmentClassesController(MyDbContext context)
        {
            _context = context;
        }

        public class AllotmentClassData {
            public string Allotment_Class { get; set; }
            public string Account_Code { get; set; }
            public string Fund_Code { get; set; }
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

        public class TestClass {
            public string data { get; set; }
        }

        // POST: AllotmentClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AllotmentClass allotmentClass)
        {
            if (ModelState.IsValid)
            {
                if (allotmentClass.Allotment_Class == "PS" || allotmentClass.Allotment_Class == "ps")
                {
                    allotmentClass.Allotment_Class = "PS";
                    allotmentClass.Account_Code = "100";
                    allotmentClass.Fund_Code = "01";
                    allotmentClass.Desc = "Personnel Services";
                    _context.Add(allotmentClass);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                else if (allotmentClass.Allotment_Class == "MOOE" || allotmentClass.Allotment_Class == "mooe")
                {
                    allotmentClass.Allotment_Class = "MOOE";
                    allotmentClass.Account_Code = "200";
                    allotmentClass.Fund_Code = "02";
                    allotmentClass.Desc = "Maintenance and Other Operating Expenses";
                    _context.Add(allotmentClass);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                else if (allotmentClass.Allotment_Class == "CO" || allotmentClass.Allotment_Class == "co")
                {
                    allotmentClass.Allotment_Class = "CO";
                    allotmentClass.Account_Code = "300";
                    allotmentClass.Fund_Code = "06";
                    allotmentClass.Desc = "Capital Outlay";
                    _context.Add(allotmentClass);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
            }
            //return View(allotmentClass);
            return Json(allotmentClass);
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
        /*[ValidateAntiForgeryToken]*/
        public async Task<IActionResult> Edit([FromBody] AllotmentClass allotmentClass)
        {
            /*var allotment_class = await _context.AllotmentClass.Where(x => x.Id == allotmentClass.Id).AsNoTracking().FirstOrDefaultAsync();
            allotment_class.Allotment_Class = allotmentClass.Allotment_Class;
            allotment_class.Account_Code = allotmentClass.Account_Code;
            return RedirectToAction("Index");*/

            _context.Update(allotmentClass); 
            await _context.SaveChangesAsync();

            return Json(allotmentClass);
            
        }

        public async Task<ActionResult> Delete(int id)
        {
            var allotmentClass = await _context.AllotmentClass.Where(p => p.Id == id).FirstOrDefaultAsync();
            _context.AllotmentClass.Remove(allotmentClass);
            await _context.SaveChangesAsync();
            //return RedirectToAction("Index");
            return Json(id);
        }

        private bool AllotmentClassExists(int id)
        {
            return _context.AllotmentClass.Any(e => e.Id == id);
        }
    }
}
