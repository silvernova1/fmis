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
using System.Text.Json;

namespace fmis.Controllers.Budget.John
{
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _context;
        private readonly UacsContext _uContext;
        private readonly Budget_allotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly MyDbContext _MyDbContext;

        public FundSourceController(FundSourceContext context, UacsContext uContext, Budget_allotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
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

            return View( await _context.FundSource.ToListAsync());


            
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


            var json = JsonSerializer.Serialize(_MyDbContext.FundSourceAmount.Where(s => s.FundsId == id).ToList());
            ViewBag.temp = json;
            var uacs_data = JsonSerializer.Serialize(_MyDbContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;



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


            /*if (Ors_head.Id == 0)
            {

                ModelState.AddModelError("", "Select ORS Head");

            }

            int SelectValue = Ors_head.Id;
            ViewBag.SelectedValue = Ors_head.Id;*/

            /*List<Prexc> p = new List<Prexc>();

            p = (from c in _pContext.Prexc select c).ToList();
            p.Insert(0, new Prexc { Id = 0, pap_title = "--Select PREXC--" });

            ViewBag.message = p;*/

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
            return View(fundSource);
        }

        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;

            /*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*/

            ViewBag.Id = new SelectList((from s in _pContext.Prexc.ToList()
                                                 select new
                                                 {
                                                     Id = s.Id,
                                                     prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                                 }),
       "Id",
       "prexc",
       null);

        }

        // POST: FundSource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FundSourceId,PrexcCode,FundSourceTitle,Description,FundSourceTitleCode,Respo")] FundSource fundSource)
        {
            if (id != fundSource.FundSourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fundSource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundSourceExists(fundSource.FundSourceId))
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
            return View(fundSource);
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
