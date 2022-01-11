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
            public string Realignment_from { get; set; }
            public string Realignment_to { get; set; }
            public float Realignment_amount { get; set; }
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
                            .Include(x => x.SubAllotmentRealignment)
                            .Include(x => x.Uacs)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);
            SubAllotment.Uacs = await _UacsContext.Uacs.AsNoTracking().ToListAsync();

            return View("~/Views/SubAllotment_Realignment/Index.cshtml", SubAllotment);
        }
        /*  {
              ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

              SubAllotment = await _MyDbContext.Sub_allotment
                                      .Include(x => x.SubAllotmentRealignment)
                                      .Include(x => x.Uacs)
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.SubAllotmentId == sub_allotment_id);
              SubAllotment.Uacs = await _UacsContext.Uacs.AsNoTracking().ToListAsync();

              return View("~/Views/SubAllotment_Realignment/Index.cshtml", SubAllotment);

          }*/

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
                sub_realignment.Realignment_from = item.Realignment_from;
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
