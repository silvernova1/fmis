using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Models.John;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Rotativa.AspNetCore;

namespace fmis.Controllers
{
    public class ObligationAmountController : Controller
    {
        private readonly ObligationAmountContext _context;
        private readonly UacsContext _Ucontext;
        private readonly MyDbContext _MyDbContext;
        private Obligation obligation;
        private decimal REMAINING_BALANCE = 0;
        private decimal OBLIGATED_AMOUNT = 0;

        public ObligationAmountController(ObligationAmountContext context, UacsContext ucontext, MyDbContext myDbContext)
        {
            _context = context;
            _Ucontext = ucontext;
            _MyDbContext = myDbContext;
        }

        public class ObligationCalculationData 
        {
            public int obligation_id { get; set; }
            public string obligation_token { get; set; }
            public string obligation_amount_token { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
            public decimal amount { get; set; }
        }

        public class PostObligationCalculationData
        {
            public int obligation_id { get; set; }
            public string obligation_token { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
        }

        public class GetObligatedAndRemaining {
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
        }

        public class ObligationAmountData
        {
            public int ObligationId { get; set; }
            public int UacsId { get; set; }
            public string Expense_code { get; set; }
            public decimal Amount { get; set; }
            public float Total_disbursement { get; set; }
            public float Total_net_amount { get; set; }
            public float Total_tax_amount { get; set; }
            public float Total_others { get; set; }
            public string obligation_token { get; set; }
            public string obligation_amount_token { get; set; }
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

        public class SourceRemainingAndObligated { 
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SaveObligationAmount(List<ObligationAmountData> data)
        {
            var data_holder = _context.ObligationAmount;
            decimal utilized_amount = 0;

            foreach (var item in data)
            {
                var obligation_amount = new ObligationAmount(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == item.obligation_amount_token) != null) //CHECK IF EXIST
                    obligation_amount =  await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == item.obligation_amount_token);

                obligation_amount.ObligationId = item.ObligationId;
                obligation_amount.UacsId = item.UacsId;
                obligation_amount.Expense_code = Convert.ToInt64(item.Expense_code);
                obligation_amount.Amount =  item.Amount;
                obligation_amount.Total_disbursement = item.Total_disbursement;
                obligation_amount.Total_net_amount = item.Total_net_amount;
                obligation_amount.Total_tax_amount = item.Total_tax_amount;
                obligation_amount.Total_others = item.Total_others;
                obligation_amount.status = "activated";
                obligation_amount.obligation_token = item.obligation_token;
                obligation_amount.obligation_amount_token = item.obligation_amount_token;
                utilized_amount += item.Amount;

                _context.ObligationAmount.Update(obligation_amount);
                await _context.SaveChangesAsync();
            }
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> calculateObligatedAmount(ObligationCalculationData calculation_data) 
        {
            decimal remaining_balance = 0;
            decimal obligated_amount = 0;

            if (calculation_data.obligation_id != 0)
            {
                obligation = await _MyDbContext.Obligation
                    .Include(x => x.ObligationAmounts)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == calculation_data.obligation_id);
            }
            else
            {
                obligation = await _MyDbContext.Obligation
                    .Include(x => x.ObligationAmounts)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.obligation_token == calculation_data.obligation_token);
            }

            //Obligate/calculation the fundsource/suballotment
            if (obligation.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == obligation.source_id).FirstOrDefaultAsync();
                fund_source.Remaining_balance = calculation_data.remaining_balance;
                fund_source.obligated_amount = calculation_data.obligated_amount;

                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;

                _MyDbContext.FundSources.Update(fund_source);
                _MyDbContext.SaveChanges();
            }
            else if (obligation.source_type == "sub_allotment")
            {
                //code ni amalio

                var sub_allotment = await _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == obligation.source_id).FirstOrDefaultAsync();
                sub_allotment.Remaining_balance = calculation_data.remaining_balance;
                sub_allotment.obligated_amount = calculation_data.obligated_amount;

                remaining_balance = sub_allotment.Remaining_balance;
                obligated_amount = sub_allotment.obligated_amount;

                _MyDbContext.Sub_allotment.Update(sub_allotment);
                _MyDbContext.SaveChanges();
            }

            var obligation_amount = await _context.ObligationAmount.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == calculation_data.obligation_amount_token);
            obligation_amount.Amount = calculation_data.amount;
            _context.ObligationAmount.Update(obligation_amount);
            _context.SaveChanges();

            GetObligatedAndRemaining get_obligated_remaining = new GetObligatedAndRemaining();
            get_obligated_remaining.remaining_balance = remaining_balance;
            get_obligated_remaining.obligated_amount = obligated_amount;
            return Json(get_obligated_remaining); //get the remaining_balance
        }

        [HttpPost]
        public async Task<IActionResult> getRemainigAndObligated(PostObligationCalculationData post_calculation_data)
        {
            decimal remaining_balance = 0;
            decimal obligated_amount = 0;
            if (post_calculation_data.obligation_id != 0)
            {
                obligation = await _MyDbContext.Obligation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == post_calculation_data.obligation_id);
            }
            else
            {
                obligation = await _MyDbContext.Obligation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.obligation_token == post_calculation_data.obligation_token);
            }

            //calculation for funsource or sub allotment
            if (obligation.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == obligation.source_id).FirstOrDefaultAsync();
                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;
            }
            else if (obligation.source_type == "sub_allotment")
            {
                //code ni amalio
                var sub_allotment = await _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == obligation.source_id).FirstOrDefaultAsync();
                remaining_balance = sub_allotment.Remaining_balance;
                obligated_amount = sub_allotment.obligated_amount;
            }

            GetObligatedAndRemaining get_obligated_remaining = new GetObligatedAndRemaining();
            get_obligated_remaining.remaining_balance = remaining_balance;
            get_obligated_remaining.obligated_amount = obligated_amount;

            return Json(get_obligated_remaining); //get the remaining_balance
        }


        // POST: Uacs/Delete/5
        [HttpPost]
        public IActionResult DeleteObligationAmount(DeleteData data)
        {
            if(data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    SetUpDeleteDataCalculation(many.many_token,data.source_id,data.source_type);
            }
            else
                SetUpDeleteDataCalculation(data.single_token,data.source_id,data.source_type);


            SourceRemainingAndObligated sourceRemainingObligated = new SourceRemainingAndObligated();
            sourceRemainingObligated.remaining_balance = REMAINING_BALANCE;
            sourceRemainingObligated.obligated_amount = OBLIGATED_AMOUNT;
            return Json(sourceRemainingObligated);
        }

        public void SetUpDeleteDataCalculation(string obligation_amount_token,int source_id,string source_type)
        {
            var obligation_amount = new ObligationAmount(); //CLEAR OBJECT
            obligation_amount = _context.ObligationAmount
                                .Include(x => x.fundSource)
                                .FirstOrDefault(x => x.obligation_amount_token == obligation_amount_token);
            obligation_amount.status = "deactivated";
            if (source_type == "fund_source")
            {
                obligation_amount.fundSource = _MyDbContext.FundSources.FirstOrDefault(x => x.FundSourceId == source_id);
                obligation_amount.fundSource.Remaining_balance += obligation_amount.Amount;
                obligation_amount.fundSource.obligated_amount -= obligation_amount.Amount;
            }
            else if (source_type == "sub_allotment") 
            {
                obligation_amount.SubAllotment = _MyDbContext.Sub_allotment.FirstOrDefault(x => x.SubAllotmentId == source_id);
                obligation_amount.SubAllotment.Remaining_balance += obligation_amount.Amount;
                obligation_amount.SubAllotment.obligated_amount -= obligation_amount.Amount;
            }
            _context.Update(obligation_amount);
            _context.SaveChanges();

            REMAINING_BALANCE = obligation_amount.fundSource.Remaining_balance;
            OBLIGATED_AMOUNT = obligation_amount.fundSource.obligated_amount;
        }


    }
}