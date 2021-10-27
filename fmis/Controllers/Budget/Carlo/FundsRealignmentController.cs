﻿using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data.Carlo;
using System.Text.Json;
using fmis.Models.Carlo;
using fmis.Data;

namespace fmis.Controllers.Budget.Carlo
{
    public class FundsRealignmentController : Controller
    {

        private readonly FundsRealignmentContext _context;
        private readonly UacsContext _UacsContext;

        public FundsRealignmentController(FundsRealignmentContext context, UacsContext UacsContext)
        {
            _context = context;
            _UacsContext = UacsContext;
        }

        public class FundsRealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public int Realignment_amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public string token { get; set; }
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

        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var json = JsonSerializer.Serialize(_context.FundsRealignment.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            var uacs_data = JsonSerializer.Serialize(_UacsContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;

            return View("~/Views/Carlo/FundsRealignment/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveFundsRealignment(List<FundsRealignmentData> data)
        {
            var data_holder = this._context.FundsRealignment;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //UPDATE
                {

                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Realignment_from = item.Realignment_from;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Realignment_to = item.Realignment_to;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Realignment_amount = item.Realignment_amount;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";

                    this._context.SaveChanges();
                }
                else if (item.Realignment_from.ToString() != null || item.Realignment_to.ToString() != null || item.Realignment_amount.ToString() != null) //SAVE
                         
                {  
                    var funds = new FundsRealignment(); //CLEAR OBJECT
                    funds.Id = item.Id;
                    funds.Realignment_from = item.Realignment_from;
                    funds.Realignment_to = item.Realignment_to;
                    funds.Realignment_amount = item.Realignment_amount;
                    funds.status = "activated";
                    funds.token = item.token;

                    this._context.FundsRealignment.Update(funds);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFundsRealignment(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.FundsRealignment;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.FundsRealignment;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}