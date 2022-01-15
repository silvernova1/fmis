using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using System.Text.Json;
using fmis.Models;
using Microsoft.EntityFrameworkCore;

namespace fmis.Controllers
{
    public class SubAllotment_RealignmentController : Controller
    {
        private readonly SubAllotment_RealignmentContext _context;
        private readonly UacsContext _UacsContext;
        private readonly Suballotment_amountContext _SAContext;
        private readonly MyDbContext _MyDbContext;
        private readonly Sub_allotmentContext _SContext;
        private Sub_allotment SubAllotment;


        public SubAllotment_RealignmentController(SubAllotment_RealignmentContext context, UacsContext UacsContext, Suballotment_amountContext SAContext, Sub_allotmentContext SContext, MyDbContext MyDbContext)
        {
            _context = context;
            _UacsContext = UacsContext;
            _SAContext = SAContext;
            _SContext = SContext;
            _MyDbContext = MyDbContext;
        }

        public class SubAllotment_RealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public decimal Realignment_amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int SubAllotmentId { get; set; }
            public string token { get; set; }
        }

        public class SubAllotment_RealignmentSaveAmount
        {
            public int sub_allotment_id { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal realignment_amount { get; set; }
            public decimal amount { get; set; }
            public string realignment_token { get; set; }
            public string sub_allotment_amount_token { get; set; }
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

        public async Task<IActionResult> Index(int sub_allotment_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            SubAllotment = await _MyDbContext.Sub_allotment
                            .Include(x => x.SubAllotmentAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.Budget_allotment)
                            .Include(x => x.SubAllotmentRealignment.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.SubAllotmentAmount)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);
            var from_uacs = await _MyDbContext.Suballotment_amount
                            .Where(x => x.SubAllotmentId == sub_allotment_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            SubAllotment.Uacs = await _UacsContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId)).AsNoTracking().ToListAsync();

            //return Json(FundSource);
            return View("~/Views/SubAllotment_Realignment/Index.cshtml", SubAllotment);
        }

       
        public async Task<IActionResult> realignmentRemaining(int sub_allotment_id)
        {
            var sub_allotment = await _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == sub_allotment_id).FirstOrDefaultAsync();
            return Json(sub_allotment);
        }

        public async Task<IActionResult> realignmentAmountSave(SubAllotment_RealignmentSaveAmount calculation)
        {
            SubAllotment = await _MyDbContext.Sub_allotment
                            .Include(x => x.SubAllotmentAmounts)
                            .Include(x => x.SubAllotmentRealignment.Where(s => s.token == calculation.realignment_token))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == calculation.sub_allotment_id);
            //funsource saving
            SubAllotment.realignment_amount = calculation.realignment_amount;
            SubAllotment.Remaining_balance = calculation.remaining_balance;
            //fund realignment saving
            SubAllotment.SubAllotmentRealignment.FirstOrDefault().Realignment_amount = calculation.amount;

            //continue the code in fundsource amount here tomorrow 01/13/2022

            _MyDbContext.Update(SubAllotment);
            await _MyDbContext.SaveChangesAsync();

            return Json(SubAllotment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> SaveSubAllotment_Realignment(List<SubAllotment_RealignmentData> data)
        {
            var data_holder = _context.SubAllotment_Realignment;

            foreach (var item in data)
            {

                var sub_realignment = new SubAllotment_Realignment(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    sub_realignment = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                sub_realignment.SubAllotmentId = item.SubAllotmentId;
                sub_realignment.SubAllotmentAmountId = item.Realignment_from;
                sub_realignment.Realignment_to = item.Realignment_to;
                sub_realignment.Realignment_amount = item.Realignment_amount;
                sub_realignment.status = "activated";
                sub_realignment.token = item.token;

                _context.SubAllotment_Realignment.Update(sub_realignment);
                await _context.SaveChangesAsync();
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
