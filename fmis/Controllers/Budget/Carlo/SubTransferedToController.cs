using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data.Carlo;
using System.Text.Json;
using fmis.Models.John;
using fmis.Data;
using fmis.Data.John;
using fmis.Models;
using fmis.Models.Carlo;
using Microsoft.EntityFrameworkCore;


namespace fmis.Controllers.Budget.Carlo
{
    public class SubTransferedToController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private SubAllotment SubAllotment;

        public SubTransferedToController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public async Task<IActionResult> Index(int sub_allotment_id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");


            SubAllotment = await _MyDbContext.SubAllotment
                            .Include(x => x.SubAllotmentAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.Budget_allotment)
                            .Include(x => x.SubTransferedTo.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.Suballotment_amount)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);
            var from_uacs = await _MyDbContext.Suballotment_amount
                            .Where(x => x.SubAllotmentId == sub_allotment_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            SubAllotment.Uacs = await _MyDbContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId)).AsNoTracking().ToListAsync();

            return View("~/Views/SubTransferedTo/Index.cshtml", SubAllotment);
        }
    }
}
