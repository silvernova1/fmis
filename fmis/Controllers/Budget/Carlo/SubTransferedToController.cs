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


namespace fmis.Controllers.Budget.Carlo
{
    public class SubTransferedToController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private SubAllotment SubAllotment;

        public SubTransferedToController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public class SubTransferedToData
        {
            public int Realignment_to { get; set; }
            public string Particulars { get; set; }
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
            public int fundsource_id { get; set; }
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
                            .Include(x => x.SubTransferedTo.Where(w => w.status == "activated"))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);

            var from_uacs = await _MyDbContext.Suballotment_amount
                            .Where(x => x.SubAllotmentId == sub_allotment_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            //SubAllotment.Uacs = await _MyDbContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId) && p.uacs_type == SubAllotment.AllotmentClassId).AsNoTracking().ToListAsync();

            SubAllotment.Uacs = await _MyDbContext.Uacs.Where(x =>  x.uacs_type == SubAllotment.AllotmentClassId).ToListAsync();

            return View("~/Views/SubTransferedTo/Index.cshtml", SubAllotment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubTransferedTo(List<SubTransferedToData> data)
        {
            var data_holder = _MyDbContext.SubTransferedTo;
            var sub_transfered_to = new SubTransferedTo(); //CLEAR OBJECT

            foreach (var item in data)
            {

                sub_transfered_to = new SubTransferedTo(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    sub_transfered_to = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                sub_transfered_to.SubAllotmentId = item.SubAllotmentId;
                sub_transfered_to.SubAllotmentAmountId = item.Realignment_to;
                sub_transfered_to.Particulars = item.Particulars;
                sub_transfered_to.Amount = item.Amount;
                sub_transfered_to.status = "activated";
                sub_transfered_to.token = item.token;

                _MyDbContext.SubTransferedTo.Update(sub_transfered_to);
                await _MyDbContext.SaveChangesAsync();
            }
            return Json(sub_transfered_to);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubTransferedTo(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._MyDbContext.SubTransferedTo;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _MyDbContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._MyDbContext.SubTransferedTo;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}
