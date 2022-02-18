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
using fmis.Models.John;
using System.Globalization;
using fmis.Models.silver;
using Microsoft.AspNetCore.Http;

namespace fmis.Controllers.Budget
{
    public class BudgetAllotmentTrustFundController : Controller
    {
        private readonly MyDbContext _context;

        public BudgetAllotmentTrustFundController(MyDbContext context)
        {
         
            _context = context;

        }

        public async Task<IActionResult> Index(int? id)
        {
            /*const string yearly_reference = "_yearly_reference";*/
            ViewBag.filter = new FilterSidebar("trust_fund", "BudgetAllotment_trust_fund", "");
            ViewBag.layout = "_Layout";

            var budget_allotment_trust_fund = await _context.BudgetAllotmentTrustFund
            .Include(c => c.Yearly_reference)
            .Include(x => x.FundSources)

            .AsNoTracking()
            .ToListAsync();

            ViewBag.AllotmentClass = await _context.AllotmentClass.AsNoTracking().ToListAsync();
            ViewBag.AppropriationSource = await _context.Appropriation.AsNoTracking().ToListAsync();
            return View(budget_allotment_trust_fund);
        }

        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            PopulateYrDropDownList();
   
            return View();
        }

        private void PopulateYrDropDownList(object selectedPrexc = null)
        {

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

            oh = (from c in _context.Yearly_reference select c).ToList();
            oh.Insert(0, new Yearly_reference { YearlyReferenceId = 0, YearlyReference = "--Select Year--" });

            ViewBag.message = oh;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BudgetAllotmentTrustFund budget_allotment_trust_fund)
        {
            _context.Add(budget_allotment_trust_fund);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
