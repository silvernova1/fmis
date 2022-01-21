using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Rotativa.AspNetCore;

namespace fmis.Controllers.Budget
{
    public class UtilizationAmountController : Controller
    {
        private readonly UtilizationAmountContext _context;
        private readonly UacsContext _Ucontext;
        private readonly MyDbContext _MyDbContext;
        private Utilization utilization;
        private decimal REMAINING_BALANCE = 0;
        private decimal UTILIZED_AMOUNT = 0;

        public UtilizationAmountController(UtilizationAmountContext context, UacsContext ucontext, MyDbContext myDbContext)
        {
            _context = context;
            _Ucontext = ucontext;
            _MyDbContext = myDbContext;

        }

        public class UtilizationCalculationData
        {
            public int utilization_id { get; set; }
            public string utilization_token { get; set; }
            public string utilization_amount_token { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal utilized_amount { get; set; }
            public decimal amount { get; set; }
        }

        public class PostUtilizationCalculationData
        {
            public int utilization_id { get; set; }
            public string utilization_token { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal utilized_amount { get; set; }
        }

        public class GetUtilizedAndRemaining
        {
            public decimal remaining_balance { get; set; }
            public decimal utilized_amount { get; set; }
        }

        public class UtilizationAmountData
        {
            public int UtilizationId { get; set; }
            public int UacsId { get; set; }
            public string Expense_code { get; set; }
            public decimal Amount { get; set; }
            public float Total_disbursement { get; set; }
            public float Total_net_amount { get; set; }
            public float Total_tax_amount { get; set; }
            public float Total_others { get; set; }
            public string utilization_token { get; set; }
            public string utilization_amount_token { get; set; }
            public string status { get; set; }
        }

        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public int source_id { get; set; }
            public string source_type { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        public class SourceRemainingAndUtilized
        {
            public decimal remaining_balance { get; set; }
            public decimal utilized_amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUtilizationAmount(List<UtilizationAmountData> data)
        {
            var data_holder = _context.UtilizationAmount;
            decimal utilized_amount = 0;

            foreach (var item in data)
            {
                var utilization_amount = new UtilizationAmount(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.utilization_amount_token == item.utilization_amount_token) != null) //CHECK IF EXIST
                    utilization_amount = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.utilization_amount_token == item.utilization_amount_token);

                utilization_amount.UtilizationId = item.UtilizationId;
                utilization_amount.UacsId = item.UacsId;
                utilization_amount.Expense_code = Convert.ToInt64(item.Expense_code);
                utilization_amount.Amount = item.Amount;
                utilization_amount.Total_disbursement = item.Total_disbursement;
                utilization_amount.Total_net_amount = item.Total_net_amount;
                utilization_amount.Total_tax_amount = item.Total_tax_amount;
                utilization_amount.Total_others = item.Total_others;
                utilization_amount.status = "activated";
                utilization_amount.utilization_token = item.utilization_token;
                utilization_amount.utilization_amount_token = item.utilization_amount_token;
                utilized_amount += item.Amount;

                _context.UtilizationAmount.Update(utilization_amount);
                await _context.SaveChangesAsync();
            }
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> calculateUtilizedAmount(UtilizationCalculationData calculation_data)
        {
            decimal remaining_balance = 0;
            decimal utilized_amount = 0;

            if (calculation_data.utilization_id != 0)
            {
                utilization = await _MyDbContext.Utilization
                    .Include(x => x.UtilizationAmount)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == calculation_data.utilization_id);
            }
            else
            {
                utilization = await _MyDbContext.Utilization
                    .Include(x => x.UtilizationAmount)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.utilization_token == calculation_data.utilization_token);
            }

            //Obligate/calculation the fundsource/suballotment
            if (utilization.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == utilization.source_id).FirstOrDefaultAsync();
                fund_source.Remaining_balance = calculation_data.remaining_balance;
                fund_source.utilized_amount = calculation_data.utilized_amount;

                remaining_balance = fund_source.Remaining_balance;
                utilized_amount = fund_source.utilized_amount;

                _MyDbContext.FundSources.Update(fund_source);
                _MyDbContext.SaveChanges();
            }
            else if (utilization.source_type == "sub_allotment")
            {
                //code ni amalio

                var sub_allotment = await _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == utilization.source_id).FirstOrDefaultAsync();
                sub_allotment.Remaining_balance = calculation_data.remaining_balance;
                sub_allotment.utilized_amount = calculation_data.utilized_amount;

                remaining_balance = sub_allotment.Remaining_balance;
                utilized_amount = sub_allotment.utilized_amount;

                _MyDbContext.Sub_allotment.Update(sub_allotment);
                _MyDbContext.SaveChanges();
            }

            var utilization_amount = await _context.UtilizationAmount.AsNoTracking().FirstOrDefaultAsync(s => s.utilization_amount_token == calculation_data.utilization_amount_token);
            utilization_amount.Amount = calculation_data.amount;
            _context.UtilizationAmount.Update(utilization_amount);
            _context.SaveChanges();

            GetUtilizedAndRemaining get_utilized_remaining = new GetUtilizedAndRemaining();
            get_utilized_remaining.remaining_balance = remaining_balance;
            get_utilized_remaining.utilized_amount = utilized_amount;
            return Json(get_utilized_remaining); //get the remaining_balance
        }

        [HttpPost]
        public async Task<IActionResult> getRemainingAndUtilized(PostUtilizationCalculationData post_calculation_data)
        {
            decimal remaining_balance = 0;
            decimal utilized_amount = 0;
            if (post_calculation_data.utilization_id != 0)
            {
                utilization = await _MyDbContext.Utilization
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == post_calculation_data.utilization_id);
            }
            else
            {
                utilization = await _MyDbContext.Utilization
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.utilization_token == post_calculation_data.utilization_token);
            }

            //calculation for funsource or sub allotment
            if (utilization.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == utilization.source_id).FirstOrDefaultAsync();
                remaining_balance = fund_source.Remaining_balance;
                utilized_amount = fund_source.utilized_amount;
            }
            else if (utilization.source_type == "sub_allotment")
            {
                //code ni amalio
                var sub_allotment = await _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == utilization.source_id).FirstOrDefaultAsync();
                remaining_balance = sub_allotment.Remaining_balance;
                utilized_amount = sub_allotment.utilized_amount;
            }

            GetUtilizedAndRemaining get_utilized_remaining = new GetUtilizedAndRemaining();
            get_utilized_remaining.remaining_balance = remaining_balance;
            get_utilized_remaining.utilized_amount = utilized_amount;

            return Json(get_utilized_remaining); //get the remaining_balance
        }

        [HttpPost]
        public IActionResult DeleteUtilizationAmount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    SetUpDeleteDataCalculation(many.many_token, data.source_id, data.source_type);
            }
            else
                SetUpDeleteDataCalculation(data.single_token, data.source_id, data.source_type);


            SourceRemainingAndUtilized sourceRemainingUtilized = new SourceRemainingAndUtilized();
            sourceRemainingUtilized.remaining_balance = REMAINING_BALANCE;
            sourceRemainingUtilized.utilized_amount = UTILIZED_AMOUNT;
            return Json(sourceRemainingUtilized);
        }

        public void SetUpDeleteDataCalculation(string utilization_amount_token, int source_id, string source_type)
        {
            var utilization_amount = new UtilizationAmount(); //CLEAR OBJECT
            utilization_amount = _context.UtilizationAmount
                                .Include(x => x.fundSource)
                                .FirstOrDefault(x => x.utilization_amount_token == utilization_amount_token);
            utilization_amount.status = "deactivated";
            if (source_type == "fund_source")
            {
                utilization_amount.fundSource = _MyDbContext.FundSources.FirstOrDefault(x => x.FundSourceId == source_id);
                utilization_amount.fundSource.Remaining_balance += utilization_amount.Amount;
                utilization_amount.fundSource.utilized_amount -= utilization_amount.Amount;
            }
            else if (source_type == "sub_allotment")
            {
                utilization_amount.SubAllotment = _MyDbContext.Sub_allotment.FirstOrDefault(x => x.SubAllotmentId == source_id);
                utilization_amount.SubAllotment.Remaining_balance += utilization_amount.Amount;
                utilization_amount.SubAllotment.utilized_amount -= utilization_amount.Amount;
            }
            _context.Update(utilization_amount);
            _context.SaveChanges();

            REMAINING_BALANCE = utilization_amount.fundSource.Remaining_balance;
            UTILIZED_AMOUNT = utilization_amount.fundSource.utilized_amount;
        }



    }
}