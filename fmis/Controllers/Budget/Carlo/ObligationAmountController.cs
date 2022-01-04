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

namespace fmis.Controllers
{
    public class ObligationAmountController : Controller
    {
        private readonly ObligationAmountContext _context;
        private readonly UacsContext _Ucontext;
        private readonly MyDbContext _MyDbContext;
      
        public ObligationAmountController(ObligationAmountContext context, UacsContext ucontext, MyDbContext myDbContext)
        {
            _context = context;
            _Ucontext = ucontext;
            _MyDbContext = myDbContext;
                   
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
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        [HttpPost]
        public IActionResult SaveObligationAmount(List<ObligationAmountData> data)
        {
            var data_holder = this._context.ObligationAmount;
            var obligation = this._MyDbContext.Obligation.Where(s => s.Id == data[0].ObligationId).FirstOrDefault();
            decimal utilized_amount = 0;

            foreach (var item in data)
            {
                var obligation_amount = new ObligationAmount(); //CLEAR OBJECT
                if (data_holder.Where(s => s.obligation_amount_token == item.obligation_amount_token).FirstOrDefault() != null) //CHECK IF EXIST
                    obligation_amount = data_holder.Where(s => s.obligation_amount_token == item.obligation_amount_token).FirstOrDefault();

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

                _context.Update(obligation_amount);
                _context.SaveChanges();
            }

            //Utilized/calculation the fundsource/suballotment
            /*if (obligation.source_type == "fund_source")
            {
                var fund_source = _MyDbContext.FundSources.Where(s => s.FundSourceId == obligation.source_id).FirstOrDefault();
                fund_source.utilization_amount += utilized_amount;
                fund_source.Remaining_balance -= fund_source.utilization_amount;

                _MyDbContext.FundSources.Update(fund_source);
                _MyDbContext.SaveChanges();
            }
            else if (obligation.source_type == "sub_allotment")
            {
                //code ni amalio
            }*/

            return Json(data);
        }

      
        // POST: Uacs/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteObligationAmount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.ObligationAmount;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.obligation_amount_token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.obligation_amount_token == many.many_token).FirstOrDefault().obligation_amount_token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.ObligationAmount;
                data_holder.Where(s => s.obligation_amount_token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.obligation_amount_token == data.single_token).FirstOrDefault().obligation_amount_token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

 
    }
}