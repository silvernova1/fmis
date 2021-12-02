using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Filters;
using fmis.Data;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Controllers.Budget.silver
{
    public class SummaryReportController : Controller
    {
        private readonly MyDbContext _context;

        public SummaryReportController(MyDbContext context)
        {
            _context = context;
        }

        // GET: AllotmentClasses
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass");

            var sumfunds = _context.FundSourceAmount.Sum(x => x.Amount);
            ViewBag.sumfunds = sumfunds;

            return View(await _context.Uacs
                .Include(i => i.FundsRealignments)
                .ToListAsync());
        
        }

    }
}
