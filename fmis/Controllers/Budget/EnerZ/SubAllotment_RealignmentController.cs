using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using System.Text.Json;
using fmis.Models;

namespace fmis.Controllers
{
    public class SubAllotment_RealignmentController : Controller
    {
        private readonly SubAllotment_RealignmentContext _context;
        private readonly UacsContext _UacsContext;

        public SubAllotment_RealignmentController(SubAllotment_RealignmentContext context, UacsContext UacsContext)
        {
            _context = context;
            _UacsContext = UacsContext;
        }

        public class SubAllotment_RealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public float Realignment_amount { get; set; }
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
            var json = JsonSerializer.Serialize(_context.SubAllotment_Realignment.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            var uacs_data = JsonSerializer.Serialize(_UacsContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;

            return View("~/Views/SubAllotment_Realignment/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSubAllotment_Realignment(List<SubAllotment_RealignmentData> data)
        {
            var data_holder = this._context.SubAllotment_Realignment;

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
                    var Subs = new SubAllotment_Realignment(); //CLEAR OBJECT
                    Subs.Id = item.Id;
                    Subs.Realignment_from = item.Realignment_from;
                    Subs.Realignment_to = item.Realignment_to;
                    Subs.Realignment_amount = item.Realignment_amount;
                    Subs.status = "activated";
                    Subs.token = item.token;

                    this._context.SubAllotment_Realignment.Update(Subs);
                    this._context.SaveChanges();
                }
            }
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubAllotment_Realignment(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.SubAllotment_Realignment;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.SubAllotment_Realignment;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }
            return Json(data);
        }
    }
}
