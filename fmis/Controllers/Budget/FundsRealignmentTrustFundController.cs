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
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.Carlo
{
    [Authorize(Policy = "BudgetAdmin")]
    public class FundsRealignmentTrustFundController : Controller
    {

        private readonly MyDbContext _MyDbContext;
        private FundSourceTrustFund FundSourceTrustFund;
        private decimal REMAINING_BALANCE = 0;
        private decimal REALIGN_AMOUNT = 0;

        public FundsRealignmentTrustFundController( MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public class FundsRealignmentTrustFundData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal Realignment_amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int FundSourceTrustFundId { get; set; }
            public string token { get; set; }
        }

        public class FundsRealignmentTrustFundSaveAmount
        {
            public int fundsource_trustfund_id { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal realignment_amount { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal fundsource_amount_remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal fundsource_amount_realignment { get; set; }
            [Column(TypeName = "decimal(18,4)")]
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
            public int fundsource_trustfund_id { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        public class GetRemainingAndRealignment
        {
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal realignment_amount { get; set; }
        }

        public async Task<IActionResult> Index(int fundsource_trustfund_id)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "budgetallotment_trustfund", "");

            FundSourceTrustFund = await _MyDbContext.FundSourceTrustFund
                            .Include(x => x.FundSourceAmountTrustFund)
                                .ThenInclude(x => x.UacsTrustFund)
                            .Include(x => x.BudgetAllotmentTrustFund)
                            .Include(x => x.FundsRealignmentTrustFund.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.FundSourceAmountTrustFund)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceTrustFundId == fundsource_trustfund_id);
            var from_uacs = await _MyDbContext.FundSourceAmountTrustFund
                            .Where(x => x.FundSourceTrustFundId == fundsource_trustfund_id)
                            .Select(x => x.UacsTrustFundId)
                            .ToArrayAsync();

            FundSourceTrustFund.UacsTrustFund = await _MyDbContext.UacsTrustFund.Where(p => !from_uacs.Contains(p.UacsTrustFundId)).AsNoTracking().ToListAsync();

            //return Json(FundSource);
            return View("~/Views/FundsRealignmentTrustFund/Index.cshtml", FundSourceTrustFund);
        }

        public async Task<IActionResult> realignmentRemaining(int fundsource_trustfund_id, string fundsource_amount_token_trustfund)
        {
            var fund_source_trustfund = await _MyDbContext.FundSourceTrustFund
                                .Include(x => x.FundSourceAmountTrustFund.Where(x => x.FundSourceAmountTokenTrustFund == fundsource_amount_token_trustfund))
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.FundSourceTrustFundId == fundsource_trustfund_id);
            return Json(fund_source_trustfund);
        }

        public async Task<IActionResult> fundSourceAmountRemainingBalance(string fundsource_amount_token_trustfund)
        {
            var fundsource_amount_remaining_balance = await _MyDbContext.FundSourceAmountTrustFund.AsNoTracking().FirstOrDefaultAsync(x => x.FundSourceAmountTokenTrustFund == fundsource_amount_token_trustfund);
            return Json(fundsource_amount_remaining_balance.remaining_balance);
        }

        public async Task<IActionResult> realignmentAmountSave(FundsRealignmentTrustFundSaveAmount calculation)
        {
            FundSourceTrustFund = await _MyDbContext.FundSourceTrustFund
                            .Include(x => x.FundSourceAmountTrustFund.Where(s => s.FundSourceAmountTokenTrustFund == calculation.fundsource_amount_token))
                            .Include(x => x.FundsRealignmentTrustFund.Where(s => s.token == calculation.realignment_token))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceTrustFundId == calculation.fundsource_trustfund_id);


            //funsource calculation
            FundSourceTrustFund.realignment_amount = calculation.realignment_amount;
            FundSourceTrustFund.Remaining_balance = calculation.remaining_balance;
            //fund realignment calculation
            FundSourceTrustFund.FundsRealignmentTrustFund.FirstOrDefault().Realignment_amount = calculation.amount;

            //fundsource amount calculation
            /*FundSourceTrustFund.FundSourceAmountTrustFund.FirstOrDefault().remaining_balance = calculation.fundsource_amount_remaining_balance;
            FundSourceTrustFund.FundSourceAmountTrustFund.FirstOrDefault().realignment_amount = calculation.fundsource_amount_realignment;*/

            _MyDbContext.Update(FundSourceTrustFund);
            await _MyDbContext.SaveChangesAsync();

            return Json(FundSourceTrustFund);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFundsRealignment(List<FundsRealignmentTrustFundData> data)
        {
            var data_holder = _MyDbContext.FundsRealignmentTrustFund;
            var funds_realignment_trustfund = new FundsRealignmentTrustFund(); //CLEAR OBJECT

            foreach (var item in data)
            {

                funds_realignment_trustfund = new FundsRealignmentTrustFund(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    funds_realignment_trustfund = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                funds_realignment_trustfund.FundSourceTrustFundId = item.FundSourceTrustFundId;
                funds_realignment_trustfund.FundSourceAmountTrustFundId = item.Realignment_from;
                funds_realignment_trustfund.Realignment_to = item.Realignment_to;
                funds_realignment_trustfund.Realignment_amount = item.Realignment_amount;
                funds_realignment_trustfund.status = "activated";
                funds_realignment_trustfund.token = item.token;

                _MyDbContext.FundsRealignmentTrustFund.Update(funds_realignment_trustfund);
                await _MyDbContext.SaveChangesAsync();
            }
            return Json(funds_realignment_trustfund);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFundsRealignment(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    SetUpDeleteDataCalculation(many.many_token, data.fundsource_trustfund_id);
            }
            else
                SetUpDeleteDataCalculation(data.single_token, data.fundsource_trustfund_id);

            GetRemainingAndRealignment getRemainingAndRealignment = new();
            getRemainingAndRealignment.remaining_balance = REMAINING_BALANCE;
            getRemainingAndRealignment.realignment_amount = REALIGN_AMOUNT;
            return Json(getRemainingAndRealignment);
        }

        public void SetUpDeleteDataCalculation(string funds_realignment_token_trustfund, int fundsource_trustfund_id)
        {
            var funds_realignment_trustfund = new FundsRealignmentTrustFund(); //CLEAR OBJECT
            funds_realignment_trustfund = _MyDbContext.FundsRealignmentTrustFund
                                .Include(x => x.FundSourceTrustFund)
                                .FirstOrDefault(x => x.token == funds_realignment_token_trustfund);
            funds_realignment_trustfund.status = "deactivated";
            funds_realignment_trustfund.FundSourceTrustFund = _MyDbContext.FundSourceTrustFund.FirstOrDefault(x => x.FundSourceTrustFundId == fundsource_trustfund_id);
            funds_realignment_trustfund.FundSourceTrustFund.realignment_amount += funds_realignment_trustfund.Realignment_amount;
            funds_realignment_trustfund.FundSourceTrustFund.realignment_amount -= funds_realignment_trustfund.Realignment_amount;

            _MyDbContext.Update(funds_realignment_trustfund);
            _MyDbContext.SaveChanges();

            REMAINING_BALANCE = funds_realignment_trustfund.FundSourceTrustFund.Remaining_balance;
            REALIGN_AMOUNT = funds_realignment_trustfund.FundSourceTrustFund.realignment_amount;
        }

    }
}
