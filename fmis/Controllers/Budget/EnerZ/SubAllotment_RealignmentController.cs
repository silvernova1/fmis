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
using Microsoft.EntityFrameworkCore;

namespace fmis.Controllers
{
    public class SubAllotment_RealignmentController : Controller
    {
        private readonly SubAllotment_RealignmentContext _context;
        private readonly UacsContext _UacsContext;
        private readonly Suballotment_amountContext _SAContext;
        private readonly Sub_allotmentContext _SContext;
        private readonly MyDbContext _MyDbContext;
        private Sub_allotment SubAllotment;


        public SubAllotment_RealignmentController(SubAllotment_RealignmentContext context, UacsContext UacsContext, Suballotment_amountContext SAContext, Sub_allotmentContext SContext, MyDbContext MyDbContext)
        {
            _context = context;
            _UacsContext = UacsContext;
            _SAContext = SAContext;
            _SContext = SContext;
            _MyDbContext = MyDbContext;
        }

        public class SubAllotmentRealignmentData
        {
            public int Realignment_from { get; set; }
            public int Realignment_to { get; set; }
            public decimal Realignment_amount { get; set; }
            public string status { get; set; }
            public int Id { get; set; }
            public int SubAllotmentId { get; set; }
            public string token { get; set; }
        }

        public class SubAllotmentRealignmentSaveAmount
        {
            public int sub_allotment_id { get; set; }
            public decimal remaining_balance { get; set; }
            public decimal realignment_amount { get; set; }
            public decimal suballotment_amount_remaining_balance { get; set; }
            public decimal amount { get; set; }
            public string realignment_token { get; set; }
            public string suballotment_amount_token { get; set; }
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

            SubAllotment = await _SContext.Sub_allotment
                            .Include(x => x.SubAllotmentAmounts)
                                .ThenInclude(x => x.Uacs)
                            .Include(x => x.Budget_allotment)
                            .Include(x => x.SubAllotmentRealignment.Where(w => w.status == "activated"))
                                .ThenInclude(x => x.SubAllotmentAmount)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);
            var from_uacs = await _SAContext.Suballotment_amount
                            .Where(x => x.SubAllotmentId == sub_allotment_id)
                            .Select(x => x.UacsId)
                            .ToArrayAsync();
            SubAllotment.Uacs = await _UacsContext.Uacs.Where(p => !from_uacs.Contains(p.UacsId)).AsNoTracking().ToListAsync();

            //return Json(FundSource);
            return View("~/Views/SubAllotment_Realignment/Index.cshtml", SubAllotment);
        }

        public async Task<IActionResult> realignmentRemaining(int sub_allotment_id, string suballotment_amount_token)
        {
            var sub_allotment = await _SContext.Sub_allotment
                                .Include(x => x.SubAllotmentAmounts.Where(x => x.suballotment_amount_token == suballotment_amount_token))
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.SubAllotmentId == sub_allotment_id);
            return Json(sub_allotment);
        }

        public async Task<IActionResult> subAllotmentAmountRemainingBalance(string suballotment_amount_token)
        {
            var suballotment_amount_remaining_balance = await _SAContext.Suballotment_amount.AsNoTracking().FirstOrDefaultAsync(x => x.suballotment_amount_token == suballotment_amount_token);
            return Json(suballotment_amount_remaining_balance.remaining_balance);
        }

        public async Task<IActionResult> realignmentAmountSave(SubAllotmentRealignmentSaveAmount calculation)
        {
            SubAllotment = await _SContext.Sub_allotment
                            .Include(x => x.SubAllotmentAmounts.Where(s => s.suballotment_amount_token == calculation.suballotment_amount_token))
                            .Include(x => x.SubAllotmentRealignment.Where(s => s.token == calculation.realignment_token))
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == calculation.sub_allotment_id);
            //funsource calculation
            SubAllotment.realignment_amount = calculation.realignment_amount;
            SubAllotment.Remaining_balance = calculation.remaining_balance;
            //fund realignment calculation
            SubAllotment.SubAllotmentRealignment.FirstOrDefault().Realignment_amount = calculation.amount;
            //fundsource amount calculation
            SubAllotment.SubAllotmentAmounts.FirstOrDefault().remaining_balance = calculation.suballotment_amount_remaining_balance;

            _SContext.Update(SubAllotment);
            await _SContext.SaveChangesAsync();

            return Json(SubAllotment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubAllotmentRealignment(List<SubAllotmentRealignmentData> data)
        {
            var data_holder = _context.SubAllotment_Realignment;
            var suballotment_realignment = new SubAllotment_Realignment(); //CLEAR OBJECT

            foreach (var item in data)
            {

                suballotment_realignment = new SubAllotment_Realignment(); //CLEAR OBJECT
                if (await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token) != null) //CHECK IF EXIST
                    suballotment_realignment = await data_holder.AsNoTracking().FirstOrDefaultAsync(s => s.token == item.token);

                suballotment_realignment.SubAllotmentId = item.SubAllotmentId;
                suballotment_realignment.SubAllotmentAmountId = item.Realignment_from;
                suballotment_realignment.Realignment_to = item.Realignment_to;
                suballotment_realignment.Realignment_amount = item.Realignment_amount;
                suballotment_realignment.status = "activated";
                suballotment_realignment.token = item.token;

                _context.SubAllotment_Realignment.Update(suballotment_realignment);
                await _context.SaveChangesAsync();
            }
            return Json(suballotment_realignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubAllotmentRealignment(DeleteData data)
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
