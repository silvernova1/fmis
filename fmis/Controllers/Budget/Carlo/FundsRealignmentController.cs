using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data.Carlo;
using System.Text.Json;
using fmis.Models.Carlo;

namespace fmis.Controllers.Budget.Carlo
{
    public class FundsRealignmentController : Controller
    {

        private readonly FundsRealignmentContext _context;

        public FundsRealignmentController(FundsRealignmentContext context)
        {
            _context = context;
        }

        public class FundsRealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public int Realignment_amount { get; set; }
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
            ViewBag.Layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.FundsRealignment.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
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
                    data_holder.Find(many.many_token).status = "deactivated";
                    data_holder.Find(many.many_token).token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.FundsRealignment;
                data_holder.Find(data.single_token).status = "deactivated";
                data_holder.Find(data.single_token).token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}
