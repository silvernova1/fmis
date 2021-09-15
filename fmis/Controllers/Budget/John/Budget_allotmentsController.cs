using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Data;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace fmis.Controllers
{
    public class Budget_allotmentsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly FundSourceContext _Context;
        private readonly Yearly_referenceContext _osContext;
        private readonly Ors_headContext _orssContext;


        public Budget_allotmentsController(MyDbContext context, FundSourceContext Context, Yearly_referenceContext osContext, Ors_headContext orssContext)
        {
            _context = context;
            _Context = Context;
            _osContext = osContext;
            _orssContext = orssContext;
        }

        // GET: Budget_allotments
        public async Task<IActionResult> Index()
        {

            return View(await _context.Budget_allotments.ToListAsync());
        }

        // GET: Budget_allotments/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            PopulateHeadDropDownList();

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Head_name = "--Select ORS Head--" });

            ViewBag.message = oh;



            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }


        // Populate Ord Head
        private void PopulateHeadDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _context.Personal_information
                             orderby d.userid
                             select d;

            /*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*/

            ViewBag.Pid = new SelectList((from s in _context.Personal_information.ToList()
                                          select new
                                          {
                                              Pid = s.Pid,
                                              ps = s.fname + " " + s.mname + " " + s.lname
                                          }),
       "Pid",
       "ps",
       null);

        }




        // GET: Budget_allotments/Create
        public IActionResult Create()
        {
            PopulateYrDropDownList();
            List<Yearly_reference> oh = new List<Yearly_reference>();

            oh = (from c in _osContext.Yearly_reference select c).ToList();
            oh.Insert(0, new Yearly_reference { Id = 0, YearlyReference = "--Select Year--" });

            ViewBag.message = oh;
            return View();
        }


        private void PopulateYrDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _osContext.Yearly_reference
                             orderby d.YearlyReference
                             select d;


            ViewBag.Id = new SelectList((from s in _osContext.Yearly_reference.ToList()
                                         select new
                                         {
                                             Id = s.Id,
                                             yr = s.YearlyReference
                                         }),
       "Id",
       "yr",
       null);

        }

        // POST: Budget_allotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at,Id")] Budget_allotment budget_allotment)
        {



            try
            {
                if (ModelState.IsValid)
                {
                    List<Prexc> p = new List<Prexc>();



                    _context.Add(budget_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateYrDropDownList(budget_allotment.Id);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(budget_allotment);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }

        // GET: Budget_allotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
            if (budget_allotment == null)
            {
                return NotFound();
            }
            return View(budget_allotment);
        }

        // POST: Budget_allotments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetAllotmentId,Year,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            if (id != budget_allotment.BudgetAllotmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budget_allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Budget_allotmentExists(budget_allotment.BudgetAllotmentId))
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
            return View(budget_allotment);
        }

        // GET: Budget_allotments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget_allotment = await _context.Budget_allotments
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        // POST: Budget_allotments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
            _context.Budget_allotments.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Budget_allotmentExists(int id)
        {
            return _context.Budget_allotments.Any(e => e.BudgetAllotmentId == id);
        }
    }
}
