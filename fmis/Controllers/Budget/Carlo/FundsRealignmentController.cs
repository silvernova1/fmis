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
            public int Id { get; set; }
            public string token { get; set; }
        }

        public class ManyId
        {
            public int many_id { get; set; }
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public int single_id { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_id { get; set; }
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
                if (item.Id == 0) //save
                {
                    var fundsrealignment = new FundsRealignment(); //clear object
                    fundsrealignment.Id = item.Id;
                    fundsrealignment.Realignment_from = item.Realignment_from;
                    fundsrealignment.Realignment_to = item.Realignment_to;
                    fundsrealignment.status = "activated";
                    fundsrealignment.token = item.token;

                    this._context.FundsRealignment.Update(fundsrealignment);
                    this._context.SaveChanges();
                }
                else
                { //update
                    data_holder.Find(item.Id).Realignment_from = item.Realignment_from;
                    data_holder.Find(item.Id).Realignment_to = item.Realignment_to;
                    data_holder.Find(item.Id).status = "activated";

                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

    }
}
