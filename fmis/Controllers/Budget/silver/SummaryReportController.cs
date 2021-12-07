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
     /*   public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass");
            ViewBag.layout = "_Layout";
            ViewBag.FundSource = "_Layout";

            return View(await _context.Uacs
                .Include(i => i.FundsRealignments)
                .ToListAsync());
            .ThenInclude(x => x.FirstOrDefault()?.fundsource_id)

        }*/

    }
}
