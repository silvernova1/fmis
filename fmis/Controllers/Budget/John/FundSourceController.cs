using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Models.John;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace fmis.Controllers.Budget.John
{
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _context;
        private readonly UacsContext _uContext;
        private readonly Budget_allotmentContext _bContext;
        private readonly PrexcContext _pContext;

        public FundSourceController(FundSourceContext context, UacsContext uContext, Budget_allotmentContext bContext, PrexcContext pContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
        }


        // GET: FundSource
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

            return View(await _context.FundSource.ToListAsync());



        }

        // GET: FundSource/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource
                .FirstOrDefaultAsync(m => m.FundSourceId == id);
            if (fundSource == null)
            {
                return NotFound();
            }

            return View(fundSource);
        }

        // GET: FundSource/Create
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

        // POST: FundSource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundSourceId,PrexcCode,FundSourceTitle,Description,FundSourceTitleCode,Respo,Budget_allotmentBudgetAllotmentId,Id")] FundSource fundSource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Prexc> p = new List<Prexc>();



                    _context.Add(fundSource);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(fundSource.Id);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(fundSource);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }

        // GET: FundSource/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource.FindAsync(id);
            if (fundSource == null)
            {
                return NotFound();
            }
            PopulatePrexcsDropDownList(fundSource.Id);
            return View(fundSource);
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

        // POST: FundSource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexcToUpdate = await _context.FundSource
                .FirstOrDefaultAsync(c => c.FundSourceId == id);

            if (await TryUpdateModelAsync<FundSource>(prexcToUpdate,
        "",
        c => c.FundSourceTitle, c => c.Id, c => c.Description, c => c.FundSourceTitleCode, c => c.Respo))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            PopulatePrexcsDropDownList(prexcToUpdate.Id);
            return View(prexcToUpdate);
        }


        // GET: FundSource/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _context.FundSource
                .FirstOrDefaultAsync(m => m.FundSourceId == id);
            if (fundSource == null)
            {
                return NotFound();
            }

            return View(fundSource);
        }

        // POST: FundSource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fundSource = await _context.FundSource.FindAsync(id);
            _context.FundSource.Remove(fundSource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FundSourceExists(int id)
        {
            return _context.FundSource.Any(e => e.FundSourceId == id);
        }
    }
}
