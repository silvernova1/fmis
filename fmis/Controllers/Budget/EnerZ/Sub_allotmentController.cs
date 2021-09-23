using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace fmis.Controllers.Budget.EnerZ
{
    public class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;
        private readonly PrexcContext _pContext;

        public Sub_allotmentController(Sub_allotmentContext context, PrexcContext pContext)
        {
            _context = context;
            _pContext = pContext;
        }


        // GET: Sub_allotment
        public async Task<IActionResult> Index(int? id)
        {



            /*List<FundSource> item = _context.FundSource.Include(f => f.Budget_allotment).ToList();*/
            /* var item = _context.FundSource.FromSqlRaw("Select * from FundSource")
                   .ToList();
             return View(item);*/

            /* return View(await _context.FundSource
                 .Include(s => s.Budget_allotment)
                 .Where(m => m.Budget_allotmentBudgetAllotmentId == id)
                 .ToListAsync());*/

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
        public IActionResult Create(int? id)
        {

            PopulatePrexcsDropDownList();

            ViewBag.BudgetId = id;

            List<Prexc> p = new List<Prexc>();

            p = (from c in _pContext.Prexc select c).ToList();
            p.Insert(0, new Prexc { Id = 0, pap_title = "--Select PREXC--" });

            ViewBag.message = p;
            //TempData["Uacs"] = await _dbContext.Uacs.ToListAsync();
            //var item = await _bContext.Budget_allotment.Where(b => b.BudgetAllotmentId == 1).Select(b => b.BudgetAllotmentId).SingleOrDefaultAsync();
            //TempData["FundSource"] = await _context.FundSource.ToListAsync();

            /*
                        Budget_allotment budget_allotment = new Budget_allotment()
                        {
                            BudgetAllotmentId = 7
                        };*/


            //var FundSource = await _bContext.Budget_allotment.(u => u.BudgetAllotmentId).Select(u => u.BudgetAllotmentId).FirstOrDefaultAsync();

            //FundSource FundSource = _context.FundSource.Include(p => p.Budget_allotment).Where(p => p.Budget_allotment.BudgetAllotmentId == id).FirstOrDefault();


            return View();
        }

        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Prexc_code,Suballotment_code,Suballotment_title,Ors_head,Responsibility_number,Description,Id")] Sub_allotment sub_allotment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Prexc> p = new List<Prexc>();



                    _context.Add(sub_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(sub_allotment.Id);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(sub_allotment);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
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
            PopulatePrexcsDropDownList(sub_allotment.Id);
            return View(sub_allotment);
        }

        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.Id = new SelectList(departmentsQuery.AsNoTracking(), "Id", "pap_title", selectedDepartment);
        }

        /*private void PopulatePrexcsDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _pContext.Prexc
                             orderby d.pap_title
                             select d;

            *//*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*//*

            ViewBag.Id = new SelectList((from s in _pContext.Prexc.ToList()
                                         select new
                                         {
                                             Id = s.Id,
                                             prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                         }),
       "Id",
       "prexc",
       null);

        }*/

        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sub_allotment sub_allotment)
        {


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

