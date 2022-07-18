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
    public class FundTransferedToController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private FundSource FundSource;

        private decimal REMAINING_BALANCE = 0;
        private decimal REALIGN_AMOUNT = 0;

        public FundTransferedToController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }


 


        public async Task <IActionResult> Index(int fundsource_id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            FundSource = await _MyDbContext.FundSources
                            .Include(x => x.FundSourceAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.BudgetAllotment)
                            .Include(x => x.FundTransferedTo.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.FundSourceAmount)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceId == fundsource_id);
            var from_uacs = await _MyDbContext.FundSourceAmount
                            .Where(x => x.FundSourceId == fundsource_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            FundSource.Uacs = await _MyDbContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId)).AsNoTracking().ToListAsync();

            return View("~/Views/FundTransferedTo/Index.cshtml", FundSource);
        }



   



    }
}
