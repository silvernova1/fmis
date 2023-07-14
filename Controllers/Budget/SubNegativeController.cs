using fmis.Filters;
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
using fmis.Models.Carlo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.Carlo
{
    [Authorize(Policy = "BudgetAdmin")]
    public class SubNegativeController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private SubAllotment SubAllotment;

        public SubNegativeController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public class SubNegativeData
        {
            public int Realignment_to { get; set; }
            [Column(TypeName = "decimal(18,4)")]
            public decimal Amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int SubAllotmentId { get; set; }
            public string token { get; set; }
        }


        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public int sub_allotment_id { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        public async Task<IActionResult> Index(int sub_allotment_id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");


            SubAllotment = await _MyDbContext.SubAllotment
                            .Include(x => x.SubAllotmentAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.Budget_allotment)
                            .Include(x => x.SubNegative.Where(w => w.status == "activated"))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);

            var from_uacs = await _MyDbContext.Suballotment_amount
                            .Where(x => x.SubAllotmentId == sub_allotment_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();


            SubAllotment.Uacs = await _MyDbContext.Uacs.Where(x => x.uacs_type == SubAllotment.AllotmentClassId).ToListAsync();

            return View("~/Views/SubNegative/Index.cshtml", SubAllotment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubNegative(List<SubNegativeData> data)
        {
            var data_holder = _MyDbContext.SubNegative;
            var sub_negative = new SubNegative(); //CLEAR OBJECT

            foreach (var item in data)
            {
                sub_negative = new SubNegative(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    sub_negative = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                sub_negative.SubAllotmentId = item.SubAllotmentId;
                sub_negative.SubAllotmentAmountId = item.Realignment_to;
                sub_negative.Amount = item.Amount;
                sub_negative.status = "activated";
                sub_negative.token = item.token;

                _MyDbContext.SubNegative.Update(sub_negative);
                await _MyDbContext.SaveChangesAsync();
            }
            return Json(sub_negative);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubNegative(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._MyDbContext.SubNegative;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _MyDbContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._MyDbContext.SubNegative;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;
                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);
        }


    }
}
