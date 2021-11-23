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
using fmis.Filters;
using fmis.Data.silver;
using System.Globalization;
using fmis.Models.John;
using fmis.ViewModel;

namespace fmis.Controllers
{
    public class Budget_allotmentsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly FundSourceContext _Context;
        private readonly Yearly_referenceContext _osContext;
        private readonly Ors_headContext _orssContext;
        private readonly PersonalInformationMysqlContext _pis_context;


        public Budget_allotmentsController(MyDbContext context, FundSourceContext Context, Yearly_referenceContext osContext, Ors_headContext orssContext, PersonalInformationMysqlContext pis_context)
        {
            _context = context;
            _Context = Context;
            _osContext = osContext;
            _orssContext = orssContext;
            _pis_context = pis_context;
        }

        // GET: Budget_allotments
        public async Task<IActionResult> Index(int? id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            ViewBag.layout = "_Layout";

            var sumfunds = _context.FundSourceAmount
                .Sum(x => x.Amount);

            ViewBag.sumfunds = sumfunds.ToString("C", new CultureInfo("en-Ph"));

            //sum of the amounts
            var query = _context.FundSources
                .Select(x => new FundSourceAmount
                {
                    Id = x.Id,
                    Amount = _context.FundSourceAmount.Where(i => i.FundSourceId == x.Id).Select(x => x.Amount).Sum()
                });


            ViewBag.Query = query.ToList();

            var ballots = _context.Budget_allotments
            .Include(c => c.Yearly_reference)
            .AsNoTracking();
            return View(await ballots.ToListAsync());
        }

        // GET: Budget_allotments/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            PopulateYrDropDownList();
            return View();
        }

        private void PopulatePsDropDownList()
        {
            ViewBag.pi_userid = new SelectList((from s in _pis_context.allPersonalInformation()
                                                where !_context.Requesting_office.Any(ro => ro.pi_userid == s.userid)
                                                select new
                                                {
                                                    pi_userid = s.userid,
                                                    ps = s.full_name
                                                }),
                                          "pi_userid",
                                          "ps",
                                           null);

        }

        private void PopulateYrDropDownList(object selectedPrexc = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var prexsQuery = from d in _context.Yearly_reference
                             orderby d.YearlyReference
                             select d;
            ViewBag.YearlyReferenceId = new SelectList((from s in _context.Yearly_reference.ToList()
                                                        where !_context.Budget_allotments.Any(ro => ro.YearlyReferenceId == s.YearlyReferenceId)
                                                        select new
                                                        {
                                                            YearlyReferenceId = s.YearlyReferenceId,
                                                            yr = s.YearlyReference
                                                        }),
                                         "YearlyReferenceId", "yr"
                                         );

            List<Yearly_reference> oh = new List<Yearly_reference>();

            oh = (from c in _osContext.Yearly_reference select c).ToList();
            oh.Insert(0, new Yearly_reference { YearlyReferenceId = 0, YearlyReference = "--Select Year--" });

            ViewBag.message = oh;
        }
        // POST: Budget_allotments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetAllotmentId,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at,YearlyReferenceId")] Budget_allotment budget_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
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
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateYrDropDownList(budget_allotment.YearlyReferenceId);
            return View(budget_allotment);
        }


        // GET: Budget_allotments/Details/5
        public async Task<IActionResult> Fundsource(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            /*PopulateHeadDropDownList();*/
            /*PopulatePsDropDownList();*/


            var sumfunds = _context.FundSourceAmount
                .Where(x => x.FundSourceId == id)
                .Sum(x => x.Amount);
                 ViewBag.sumfunds = sumfunds.ToString("C", new CultureInfo("en-Ph"));

            var subfunds = _context.FundSourceAmount;
            /*
                        List<FundSource> fundsource = _context.FundSources.ToList();
                        List<FundSourceAmount> fundsourceamounts = _context.FundSourceAmount.ToList();*/



            /*var query = from ri in _context.FundSourceAmount
                        join rr in _context.FundSources
                           on ri.FundSourceId equals rr.Id
                        where rr.FundSourceId == id
                        group ri by ri.Account_title into g
                        select new
                        {
                            Id = g.Key,
                            fundstotal = g.Sum(x=>x.Amount)
                        };*/

            var query = _context.FundSources
                .Select(x => new FundSourceAmount
                {
                    Id = x.Id,
                    Amount = _context.FundSourceAmount.Where(i => i.FundSourceId == x.Id).Select(x=>x.Amount).Sum()
                });


            ViewBag.Query = query.ToList();


            /*var result = from fundsourceamount in _context.FundSourceAmount
                         join fundsource in _context.FundSources on fundsourceamount.Id equals fundsource.FundSourceId into Fundsource
                         from m in Fundsource.DefaultIfEmpty()
                        select new
                         {
                             Id = fundsourceamount.Id,
                             Amount = fundsourceamount.Amount,
                             FundSourceId = m.FundSourceId
                         };

            ViewBag.result = result;*/


            /*  List<FundSource> fundsource = _context.FundSources.ToList();
              List<FundSourceAmount> fundsourceamounts = _context.FundSourceAmount.ToList();

              var fundsSubTotal = from e in fundsource
                                  join d in fundsourceamounts on e.Id equals d.FundSourceId into table1
                                   from d in table1.ToList()
                                   select new FundsViewModel
                                   {
                                       fundsource = e,
                                       fundsourceamounts = d,
                                   };*/








            List <Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Personalinfo_userid = "--Select ORS Head--" });

            ViewBag.message = oh;
            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .Include(s => s.Sub_allotments)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }

        public async Task<IActionResult> Suballotment(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            /*PopulateHeadDropDownList();*/
           /* PopulatePsDropDownList();*/

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Personalinfo_userid = "--Select ORS Head--" });

            ViewBag.message = oh;
            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .Include(s => s.FundSources)
                .Include(s => s.Sub_allotments)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (budget_allotment == null)
            {
                return NotFound();
            }

            return View(budget_allotment);
        }


        // GET: Budget_allotments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }
            var budget_allotment = await _context.Budget_allotments
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);

            PopulateYrDropDownList(budget_allotment.YearlyReferenceId);
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
        public async Task<IActionResult> Edit(int id, [Bind("YearlyReferenceId,BudgetAllotmentId,Allotment_series,Allotment_title,Allotment_code,Created_at,Updated_at")] Budget_allotment budget_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
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
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
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
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var budget_allotment = await _context.Budget_allotments.FindAsync(id);
            _context.Budget_allotments.Remove(budget_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool Budget_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return _context.Budget_allotments.Any(e => e.BudgetAllotmentId == id);
        }
    }
}