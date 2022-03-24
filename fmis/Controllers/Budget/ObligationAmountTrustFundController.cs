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
    public class ObligationAmountTrustFundController : Controller
    {
        private readonly ObligationAmountContext _context;
        private readonly LogsContext _LContext;
        private readonly MyDbContext _MyDbContext;
        private ObligationTrustFund ObligationTrustFund;
        private decimal REMAINING_BALANCE = 0;
        private decimal OBLIGATED_AMOUNT = 0;

        public ObligationAmountTrustFundController( MyDbContext MyDbContext)
        {
          
            _MyDbContext = MyDbContext;
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
            public decimal beginning_balance { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
        }

        public class GetObligatedAndRemaining
        {
            public decimal beginning_balance { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
            public decimal? overall_beginning_balance { get; set; }
            public decimal? overall_remaining_balance { get; set; }
            public decimal? overall_obligated_balance { get; set; }
        }

        public class ObligationAmountData
        {
            public int ObligationId { get; set; }
            public int UacsId { get; set; }
            public string Expense_code { get; set; }
            public decimal Amount { get; set; }
            /* public float Total_disbursement { get; set; }
             public float Total_net_amount { get; set; }
             public float Total_tax_amount { get; set; }
             public float Total_others { get; set; }*/
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

        public class SourceRemainingAndObligated
        {
            public decimal remaining_balance { get; set; }
            public decimal obligated_amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SaveObligationAmountTrustFund(List<ObligationAmountData> data)
        {
            var data_holder = _MyDbContext.ObligationAmountTrustFund;
            decimal obligated_amount = 0;

            foreach (var item in data)
            {
                var obligation_amount_trustfund = new ObligationAmountTrustFund(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == item.obligation_amount_token) != null) //CHECK IF EXIST
                    obligation_amount_trustfund = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == item.obligation_amount_token);

                var obligation_trustfund = await _MyDbContext.ObligationTrustFund.AsNoTracking().FirstOrDefaultAsync(x => x.obligation_token == item.obligation_token);
                var uacs = await _MyDbContext.UacsTrustFund.AsNoTracking().FirstOrDefaultAsync(x => x.Expense_code == item.Expense_code);

                obligation_amount_trustfund.ObligationTrustFundId = obligation_trustfund.Id;
                obligation_amount_trustfund.UacsId = uacs.UacsTrustFundId;
                if (item.Expense_code != "")
                    obligation_amount_trustfund.Expense_code = Convert.ToInt64(item.Expense_code);
                obligation_amount_trustfund.Amount = item.Amount;
                obligation_amount_trustfund.status = "activated";
                obligation_amount_trustfund.obligation_token = item.obligation_token;
                obligation_amount_trustfund.obligation_amount_token = item.obligation_amount_token;
                obligated_amount += item.Amount;

                _MyDbContext.ObligationAmountTrustFund.Update(obligation_amount_trustfund);
                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> calculateObligatedAmount(ObligationCalculationData calculation_data)
        {
            decimal remaining_balance = 0;
            decimal obligated_amount = 0;
            int fundsource_id = 0;
            int suballotment_id = 0;

            if (calculation_data.obligation_id != 0)
            {
                ObligationTrustFund = await _MyDbContext.ObligationTrustFund
                    .Include(x => x.ObligationAmountTrustFund)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == calculation_data.obligation_id);
            }
            else
            {
                ObligationTrustFund = await _MyDbContext.ObligationTrustFund
                    .Include(x => x.ObligationAmountTrustFund)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.obligation_token == calculation_data.obligation_token);
            }

            //Obligate/calculation the fundsource/suballotment
            if (ObligationTrustFund.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSourceTrustFund.Where(s => s.FundSourceTrustFundId == ObligationTrustFund.FundSourceTrustFundId).FirstOrDefaultAsync();
                fund_source.Remaining_balance = calculation_data.remaining_balance;
                fund_source.obligated_amount = calculation_data.obligated_amount;

                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;
                fundsource_id = (int)ObligationTrustFund.FundSourceTrustFundId;

                _MyDbContext.FundSourceTrustFund.Update(fund_source);
                _MyDbContext.SaveChanges();
            }
    

            //NotificationLogs(fundsource_id, suballotment_id, ObligationTrustFund.source_type, "obligated", calculation_data.amount);

            var obligation_amount = await _MyDbContext.ObligationAmountTrustFund.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == calculation_data.obligation_amount_token);
            obligation_amount.Amount = calculation_data.amount;
            _MyDbContext.ObligationAmountTrustFund.Update(obligation_amount);
            _MyDbContext.SaveChanges();

            GetObligatedAndRemaining get_obligated_remaining = new GetObligatedAndRemaining();
            get_obligated_remaining.remaining_balance = remaining_balance;
            get_obligated_remaining.obligated_amount = obligated_amount;
            var overall_obligation = await _MyDbContext
                                    .ObligationTrustFund
                                    .Where(x => x.status == "activated")
                                    .Include(x => x.ObligationAmountTrustFund)
                                    .Include(x => x.FundSourceTrustFund)
                                    .AsNoTracking()
                                    .ToListAsync();

            get_obligated_remaining.overall_beginning_balance = overall_obligation.Sum(x => x.FundSourceTrustFund?.Beginning_balance);
            get_obligated_remaining.overall_remaining_balance = overall_obligation.Sum(x => x.FundSourceTrustFund?.Remaining_balance);
            get_obligated_remaining.overall_obligated_balance = overall_obligation.Sum(x => x.FundSourceTrustFund?.obligated_amount);

            return Json(get_obligated_remaining); //get the remaining_balance
        }

       /* public void NotificationLogs(int fundsource_id, int suballotment_id, string source_type, string logs_type, decimal amount)
        {
            Logs logs = new();
            logs.created_id = 1;
            logs.created_name = "Rusel T. Tayong";
            logs.created_designation = "Information System Analyst II";
            logs.created_division = "RD/ARD";
            logs.created_section = "ICTU";
            logs.FundSourceId = fundsource_id;
            logs.SubAllotmentId = suballotment_id;
            logs.source_type = source_type;
            logs.logs_type = logs_type;
            logs.amount = amount;
            _LContext.Logs.Update(logs);
            _LContext.SaveChanges();
        }*/

        [HttpPost]
        public async Task<IActionResult> getRemainigAndObligated(PostObligationCalculationData post_calculation_data)
        {
            decimal beginning_balance = 0;
            decimal remaining_balance = 0;
            decimal obligated_amount = 0;
            if (post_calculation_data.obligation_id != 0)
            {
                ObligationTrustFund = await _MyDbContext.ObligationTrustFund
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == post_calculation_data.obligation_id);
            }
            else
            {
                ObligationTrustFund = await _MyDbContext.ObligationTrustFund
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.obligation_token == post_calculation_data.obligation_token);
            }

            //calculation for funsource or sub allotment
            if (ObligationTrustFund.source_type == "fund_source")
            {
                var fund_source = await _MyDbContext.FundSourceTrustFund.Where(s => s.FundSourceTrustFundId == ObligationTrustFund.FundSourceTrustFundId).FirstOrDefaultAsync();
                beginning_balance = fund_source.Beginning_balance;
                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;
            }


            GetObligatedAndRemaining get_obligated_remaining = new();
            get_obligated_remaining.beginning_balance = beginning_balance;
            get_obligated_remaining.remaining_balance = remaining_balance;
            get_obligated_remaining.obligated_amount = obligated_amount;

            return Json(get_obligated_remaining); //get the remaining_balance
        }


        // POST: Uacs/Delete/5
        [HttpPost]
        public IActionResult DeleteObligationAmount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    SetUpDeleteDataCalculation(many.many_token, data.source_id, data.source_type);
            }
            else
                SetUpDeleteDataCalculation(data.single_token, data.source_id, data.source_type);


            SourceRemainingAndObligated sourceRemainingObligated = new SourceRemainingAndObligated();
            sourceRemainingObligated.remaining_balance = REMAINING_BALANCE;
            sourceRemainingObligated.obligated_amount = OBLIGATED_AMOUNT;
            return Json(sourceRemainingObligated);
        }

        public void SetUpDeleteDataCalculation(string obligation_amount_token, int source_id, string source_type)
        {
            var obligation_amount_trustfund = new ObligationAmountTrustFund(); //CLEAR OBJECT
            obligation_amount_trustfund = _MyDbContext.ObligationAmountTrustFund
                                .Include(x => x.FundSourceTrustFund)
                                .FirstOrDefault(x => x.obligation_amount_token == obligation_amount_token);
            obligation_amount_trustfund.status = "deactivated";
            if (source_type == "fund_source")
            {
                obligation_amount_trustfund.FundSourceTrustFund = _MyDbContext.FundSourceTrustFund.FirstOrDefault(x => x.FundSourceTrustFundId == source_id);
                obligation_amount_trustfund.FundSourceTrustFund.Remaining_balance += obligation_amount_trustfund.Amount;
                obligation_amount_trustfund.FundSourceTrustFund.obligated_amount -= obligation_amount_trustfund.Amount;
            }

            _MyDbContext.Update(obligation_amount_trustfund);
            _MyDbContext.SaveChanges();

            REMAINING_BALANCE = obligation_amount_trustfund.FundSourceTrustFund.Remaining_balance;
            OBLIGATED_AMOUNT = obligation_amount_trustfund.FundSourceTrustFund.obligated_amount;
        }
    }
}