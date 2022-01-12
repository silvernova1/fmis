﻿using fmis.Filters;
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
            public decimal amount { get; set; }
            public string realignment_token { get; set; }
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

        public async Task<IActionResult> Index(int fundsource_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            FundSource = await _FContext.FundSource
                            .Include(x => x.FundSourceAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.BudgetAllotment)
                            .Include(x => x.FundsRealignment.Where(w => w.status == "activated"))
                            .Include(x => x.Uacs)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.FundSourceId == fundsource_id);
            FundSource.Uacs = await _UacsContext.Uacs.AsNoTracking().ToListAsync();

            return View("~/Views/Carlo/FundsRealignment/Index.cshtml", FundSource);
        }

        public async Task<IActionResult> realignmentRemaining(int fundsource_id) {
            var fund_source = await _FContext.FundSource.Where(s => s.FundSourceId == fundsource_id).FirstOrDefaultAsync();
            return Json(fund_source);
        }

        public async Task<IActionResult> realignmentAmountSave(FundRealingmentSaveAmount calculation)
        {
            var fund_source = await _FContext.FundSource.AsNoTracking().FirstOrDefaultAsync(s => s.FundSourceId == calculation.fundsource_id);
            fund_source.realignment_amount = calculation.realignment_amount;
            fund_source.Remaining_balance = calculation.remaining_balance;

            _FContext.FundSource.Update(fund_source);
            await _FContext.SaveChangesAsync();

            var realignment_amount = await _context.FundsRealignment.AsNoTracking().FirstOrDefaultAsync(s => s.token == calculation.realignment_token);
            realignment_amount.Realignment_amount = calculation.amount;

            _context.FundsRealignment.Update(realignment_amount);
            await _context.SaveChangesAsync();

            return Json(fund_source);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> SaveFundsRealignment(List<FundsRealignmentData> data)
        {
            var data_holder = _context.FundsRealignment;

            foreach (var item in data)
            {

                var funds_realignment = new FundsRealignment(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    funds_realignment = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                funds_realignment.FundSourceId = item.FundSourceId;
                funds_realignment.Realignment_from = item.Realignment_from;
                funds_realignment.Realignment_to = item.Realignment_to;
                funds_realignment.Realignment_amount = item.Realignment_amount;
                funds_realignment.status = "activated";
                funds_realignment.token = item.token;

                _context.FundsRealignment.Update(funds_realignment);
                await _context.SaveChangesAsync();
            }
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFundsRealignment(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._allContext.FundsRealignment;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _allContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._allContext.FundsRealignment;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _allContext.SaveChangesAsync();
            }

            return Json(data);
        }


        // GET: FundsRealignment/Delete/5
        public async Task<IActionResult> Delete(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            var funds_realignment = await _context.FundsRealignment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funds_realignment == null)
            {
                return NotFound();
            }

            return View(funds_realignment);
        }
        /*   {
               if (id == null)
               {
                   return NotFound();
               }

               var funds_realignment = await _context.FundsRealignment
                   .FirstOrDefaultAsync(m => m.Id == id);
               if (funds_realignment == null)
               {
                   return NotFound();
               }

               return View(funds_realignment);
           }*/



    }
}
