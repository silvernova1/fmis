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
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.Carlo
{
    [Authorize(Policy = "BudgetAdmin")]
    public class FundTransferedToController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private FundSource FundSource;

        public FundTransferedToController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        } 

        public class FundTransferedToData
        {
            public int Realignment_to { get; set; }
            public string Particulars { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal Amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int FundSourceId { get; set; }
            public string token { get; set; }
        }

        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public int fundsource_id { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }
        public async Task <IActionResult> Index(int fundsource_id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            FundSource = await _MyDbContext.FundSources
                            .Include(x => x.FundSourceAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.BudgetAllotment)
                            .Include(x => x.FundTransferedTo.Where(w => w.status == "activated"))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceId == fundsource_id);

            var from_uacs = await _MyDbContext.FundSourceAmount
                .Where(x => x.FundSourceId == fundsource_id)
                .Select(x => x.UacsId)
                .ToArrayAsync();

            //FundSource.Uacs = await _MyDbContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId) && p.uacs_type == FundSource.AllotmentClassId).AsNoTracking().ToListAsync();

            FundSource.Uacs = await _MyDbContext.Uacs.Where(x => x.uacs_type == FundSource.AllotmentClassId).ToListAsync();



            return View("~/Views/FundTransferedTo/Index.cshtml", FundSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFundTransferedTo(List<FundTransferedToData> data)
        {
            var data_holder = _MyDbContext.FundTransferedTo;
            var fund_transfered_to = new FundTransferedTo(); //CLEAR OBJECT

            foreach (var item in data)
            {

                fund_transfered_to = new FundTransferedTo(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    fund_transfered_to = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                fund_transfered_to.FundSourceId = item.FundSourceId;
                fund_transfered_to.FundSourceAmountId = item.Realignment_to;
                fund_transfered_to.Particulars = item.Particulars;
                fund_transfered_to.Amount = item.Amount;
                fund_transfered_to.status = "activated";
                fund_transfered_to.token = item.token;

                _MyDbContext.FundTransferedTo.Update(fund_transfered_to);
                await _MyDbContext.SaveChangesAsync();
            }
            return Json(fund_transfered_to);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFundTransferedTo(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._MyDbContext.FundTransferedTo;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _MyDbContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._MyDbContext.FundTransferedTo;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}
