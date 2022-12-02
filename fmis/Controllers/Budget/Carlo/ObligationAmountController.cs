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
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Controllers
{
    [Authorize(Policy = "BudgetAdmin")]
    public class ObligationAmountController : Controller
    {
        private readonly ObligationAmountContext _context;
        private readonly LogsContext _LContext;
        private readonly MyDbContext _MyDbContext;
        private Obligation obligation;
        private decimal REMAINING_BALANCE = 0;
        private decimal OBLIGATED_AMOUNT = 0;

        public ObligationAmountController(ObligationAmountContext context, LogsContext LContext, MyDbContext myDbContext)
        {
            _context = context;
            _LContext = LContext;
            _MyDbContext = myDbContext;
        }

        public class ObligationCalculationData
        {
            public int obligation_id { get; set; }
            public string obligation_token { get; set; }
            public string obligation_amount_token { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal obligated_amount { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal amount { get; set; }
        }

        public class PostObligationCalculationData
        {
            public int obligation_id { get; set; }
            public string obligation_token { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal beginning_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal obligated_amount { get; set; }
        }

        public class GetObligatedAndRemaining
        {
            [Column(TypeName = "decimal(18,4)")]
            public decimal beginning_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal obligated_amount { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal? overall_beginning_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal? overall_remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal? overall_obligated_balance { get; set; }
        }

        public class ObligationAmountData
        {
            public int ObligationId { get; set; }
            public int UacsId { get; set; }
            public string Expense_code { get; set; }
            [Column(TypeName = "decimal(18,4)")]
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
            [Column(TypeName = "decimal(18,4)")]
            public decimal remaining_balance { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal obligated_amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SaveObligationAmount(List<ObligationAmountData> data)
        {
            var data_holder = _context.ObligationAmount;
            decimal utilized_amount = 0;


            var currentObligation = await _MyDbContext.Obligation
                .Include(x => x.FundSource)
                .Include(x => x.SubAllotment)
                .FirstOrDefaultAsync(x => x.Id == data.First().ObligationId);


            foreach (var item in data)
            {
                var obligation_amount = new ObligationAmount(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().AnyAsync(s => s.obligation_amount_token == item.obligation_amount_token)) //CHECK IF EXIST
                    obligation_amount = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == item.obligation_amount_token);
                if (obligation_amount.Id != 0 && string.IsNullOrEmpty(item.Expense_code))
                {
                    obligation_amount.status = "deactivated";
                    _context.ObligationAmount.Update(obligation_amount);
                    await _context.SaveChangesAsync();
                    break;
                }

                if (currentObligation.source_type == "fund_source" && !string.IsNullOrEmpty(item.Expense_code))
                {
                    if (!_MyDbContext.Uacs.Any(x => x.uacs_type == currentObligation.FundSource.AllotmentClassId && x.Expense_code == item.Expense_code))
                        return BadRequest(new { error_message = "INVALID EXPENSE CODE" });
                }
                else if (currentObligation.source_type == "sub_allotment" && !string.IsNullOrEmpty(item.Expense_code))
                {
                    if (!_MyDbContext.Uacs.Any(x => x.uacs_type == currentObligation.SubAllotment.AllotmentClassId && x.Expense_code == item.Expense_code))
                        return BadRequest(new { error_message = "INVALID EXPENSE CODE" });
                }


                var obligation = await _MyDbContext.Obligation.AsNoTracking().FirstOrDefaultAsync(x => x.obligation_token == item.obligation_token);
                var uacs = await _MyDbContext.Uacs.AsNoTracking().FirstOrDefaultAsync(x => x.Expense_code == item.Expense_code);

                obligation_amount.ObligationId = obligation.Id;
                obligation_amount.UacsId = uacs.UacsId;
                if (item.Expense_code != "")
                    obligation_amount.Expense_code = Convert.ToInt64(item.Expense_code);
                obligation_amount.Amount = item.Amount;
                obligation_amount.status = "activated";
                obligation_amount.obligation_token = item.obligation_token;
                obligation_amount.obligation_amount_token = item.obligation_amount_token;
                utilized_amount += item.Amount;

                _context.ObligationAmount.Update(obligation_amount);
                Console.WriteLine(@"saved Obligation amount {0}", obligation_amount.Expense_code);
                await _context.SaveChangesAsync();
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
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == obligation.FundSourceId).FirstOrDefaultAsync();
                fund_source.Remaining_balance = calculation_data.remaining_balance;
                fund_source.obligated_amount = calculation_data.obligated_amount;

                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;
                fundsource_id = (int)obligation.FundSourceId;

                _MyDbContext.FundSources.Update(fund_source);
                _MyDbContext.SaveChanges();
            }
            else if (obligation.source_type == "sub_allotment")
            {
                //code ni amalio

                var sub_allotment = await _MyDbContext.SubAllotment.Where(s => s.SubAllotmentId == obligation.SubAllotmentId).FirstOrDefaultAsync();
                sub_allotment.Remaining_balance = calculation_data.remaining_balance;
                sub_allotment.obligated_amount = calculation_data.obligated_amount;

                remaining_balance = sub_allotment.Remaining_balance;
                obligated_amount = sub_allotment.obligated_amount;
                suballotment_id = (int)obligation.SubAllotmentId;

                _MyDbContext.SubAllotment.Update(sub_allotment);
                _MyDbContext.SaveChanges();
            }

            NotificationLogs(fundsource_id, suballotment_id, obligation.source_type, "obligated", calculation_data.amount);

            var obligation_amount = await _context.ObligationAmount.AsNoTracking().FirstOrDefaultAsync(s => s.obligation_amount_token == calculation_data.obligation_amount_token);
            obligation_amount.Amount = calculation_data.amount;
            _context.ObligationAmount.Update(obligation_amount);
            _context.SaveChanges();

            GetObligatedAndRemaining get_obligated_remaining = new GetObligatedAndRemaining();
            get_obligated_remaining.remaining_balance = remaining_balance;
            get_obligated_remaining.obligated_amount = await _context.ObligationAmount.AsNoTracking().Where(x => x.obligation_token == calculation_data.obligation_token).SumAsync(s => s.Amount);
            var overall_obligation = await _MyDbContext
                                    .Obligation
                                    .Where(x => x.status == "activated")
                                    .Include(x => x.ObligationAmounts)
                                    .Include(x => x.FundSource)
                                    .Include(x => x.SubAllotment)
                                    .AsNoTracking()
                                    .ToListAsync();

            get_obligated_remaining.overall_beginning_balance = overall_obligation.Sum(x => x.FundSource?.Beginning_balance) + overall_obligation.Sum(x => x.SubAllotment?.Beginning_balance);
            get_obligated_remaining.overall_remaining_balance = overall_obligation.Sum(x => x.FundSource?.Remaining_balance) + overall_obligation.Sum(x => x.SubAllotment?.Remaining_balance);
            get_obligated_remaining.overall_obligated_balance = overall_obligation.Sum(x => x.FundSource?.obligated_amount) + overall_obligation.Sum(x => x.SubAllotment?.obligated_amount);

            return Json(get_obligated_remaining); //get the remaining_balance
        }

        public void NotificationLogs(int fundsource_id, int suballotment_id, string source_type, string logs_type, decimal amount)
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
        }

        [HttpPost]
        public async Task<IActionResult> getRemainigAndObligated(PostObligationCalculationData post_calculation_data)
        {
            decimal beginning_balance = 0;
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
                var fund_source = await _MyDbContext.FundSources.Where(s => s.FundSourceId == obligation.FundSourceId).FirstOrDefaultAsync();
                beginning_balance = fund_source.Remaining_balance;
                remaining_balance = fund_source.Remaining_balance;
                obligated_amount = fund_source.obligated_amount;
            }
            else if (obligation.source_type == "sub_allotment")
            {
                //code ni amalio
                var sub_allotment = await _MyDbContext.SubAllotment.Where(s => s.SubAllotmentId == obligation.SubAllotmentId).FirstOrDefaultAsync();
                beginning_balance = sub_allotment.Remaining_balance;
                remaining_balance = sub_allotment.Remaining_balance;
                obligated_amount = sub_allotment.obligated_amount;
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
                obligation_amount.SubAllotment = _MyDbContext.SubAllotment.FirstOrDefault(x => x.SubAllotmentId == source_id);
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