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
using Microsoft.EntityFrameworkCore;

namespace fmis.Controllers.Budget.Carlo
{
    public class FundsRealignmentController : Controller
    {
        private readonly FundsRealignmentContext _context;
        private readonly UacsContext _UacsContext;
        private readonly FundSourceAmountContext _FAContext;
        private readonly FundSourceContext _FContext;
        private readonly MyDbContext _allContext;
        private FundSource FundSource;
        private decimal REMAINING_BALANCE = 0;
        private decimal REALIGN_AMOUNT = 0;

        public FundsRealignmentController(FundsRealignmentContext context, UacsContext UacsContext, FundSourceAmountContext FAContext, FundSourceContext FContext, MyDbContext allContext)
        {
            _context = context;
            _UacsContext = UacsContext;
            _FAContext = FAContext;
            _FContext = FContext;
            _allContext = allContext;
        }

        public class FundsRealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public decimal Realignment_amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int FundSourceId { get; set; }
            public string token { get; set; }
        }

        public class FundRealingmentSaveAmount
        {
            public int fundsource_id { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal realignment_amount { get; set; }
            public decimal fundsource_amount_remaining_balance { get; set; }
            public decimal fundsource_amount_realignment { get; set; }
            public decimal amount { get; set; }
            public string realignment_token { get; set; }
            public string fundsource_amount_token { get; set; }
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

        public class GetRemainingAndRealignment
        {
            public decimal remaining_balance { get; set; }
            public decimal realignment_amount { get; set; }
        }

        public async Task<IActionResult> Index(int fundsource_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            FundSource = await _FContext.FundSource
                            .Include(x => x.FundSourceAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.BudgetAllotment)
                            .Include(x => x.FundsRealignment.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.FundSourceAmount)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceId == fundsource_id);
            var from_uacs = await _FAContext.FundSourceAmount
                            .Where(x => x.FundSourceId == fundsource_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            FundSource.Uacs = await _UacsContext.Uacs/*.Where(p => !from_uacs.Contains(p.UacsId))*/.AsNoTracking().ToListAsync();

            //return Json(FundSource);
            return View("~/Views/Carlo/FundsRealignment/Index.cshtml", FundSource);
        }

        public async Task<IActionResult> realignmentRemaining(int fundsource_id,string fundsource_amount_token) {
            var fund_source = await _FContext.FundSource
                                .Include(x => x.FundSourceAmounts.Where(x => x.fundsource_amount_token == fundsource_amount_token))
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.FundSourceId == fundsource_id);
            return Json(fund_source);
        }

        public async Task<IActionResult> fundSourceAmountRemainingBalance(string fundsource_amount_token)
        {
            var fundsource_amount_remaining_balance = await _FAContext.FundSourceAmount.AsNoTracking().FirstOrDefaultAsync(x => x.fundsource_amount_token == fundsource_amount_token);
            return Json(fundsource_amount_remaining_balance.remaining_balance);
        }

        public async Task<IActionResult> realignmentAmountSave(FundRealingmentSaveAmount calculation)
        {
            FundSource = await _FContext.FundSource
                            .Include(x => x.FundSourceAmounts.Where(s => s.fundsource_amount_token == calculation.fundsource_amount_token))
                            .Include(x => x.FundsRealignment.Where(s => s.token == calculation.realignment_token))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceId == calculation.fundsource_id);
            //funsource calculation
            FundSource.realignment_amount = calculation.realignment_amount;
            FundSource.Remaining_balance = calculation.remaining_balance;
            //fund realignment calculation
            FundSource.FundsRealignment.FirstOrDefault().Realignment_amount = calculation.amount;
            //fundsource amount calculation
            FundSource.FundSourceAmounts.FirstOrDefault().remaining_balance = calculation.fundsource_amount_remaining_balance;
            FundSource.FundSourceAmounts.FirstOrDefault().realignment_amount = calculation.fundsource_amount_realignment;
                
            _FContext.Update(FundSource);
            await _FContext.SaveChangesAsync();

            return Json(FundSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> SaveFundsRealignment(List<FundsRealignmentData> data)
        {
            var data_holder = _context.FundsRealignment;
            var funds_realignment = new FundsRealignment(); //CLEAR OBJECT

            foreach (var item in data)
            {

                funds_realignment = new FundsRealignment(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    funds_realignment = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                funds_realignment.FundSourceId = item.FundSourceId;
                funds_realignment.FundSourceAmountId = item.Realignment_from;
                funds_realignment.Realignment_to = item.Realignment_to;
                funds_realignment.Realignment_amount = item.Realignment_amount;
                funds_realignment.status = "activated";
                funds_realignment.token = item.token;

                _context.FundsRealignment.Update(funds_realignment);
                await _context.SaveChangesAsync();
            }
            return Json(funds_realignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFundsRealignment(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    SetUpDeleteDataCalculation(many.many_token, data.fundsource_id);
            }
            else
                SetUpDeleteDataCalculation(data.single_token, data.fundsource_id);

            GetRemainingAndRealignment getRemainingAndRealignment = new();
            getRemainingAndRealignment.remaining_balance = REMAINING_BALANCE;
            getRemainingAndRealignment.realignment_amount = REALIGN_AMOUNT;
            return Json(getRemainingAndRealignment);
        }

        public void SetUpDeleteDataCalculation(string funds_realignment_token, int fundsource_id)
        {
            var funds_realignment = new FundsRealignment(); //CLEAR OBJECT
            funds_realignment = _context.FundsRealignment
                                .Include(x => x.FundSource)
                                .FirstOrDefault(x => x.token == funds_realignment_token);
            funds_realignment.status = "deactivated";
            //funds_realignment.FundSource = _FContext.FundSource.FirstOrDefault(x => x.FundSourceId == fundsource_id);
            funds_realignment.FundSource.Remaining_balance += funds_realignment.Realignment_amount;
            funds_realignment.FundSource.realignment_amount -= funds_realignment.Realignment_amount;

            _context.Update(funds_realignment);
            _context.SaveChanges();

            REMAINING_BALANCE = funds_realignment.FundSource.Remaining_balance;
            REALIGN_AMOUNT = funds_realignment.FundSource.realignment_amount;
        }

    }
}
