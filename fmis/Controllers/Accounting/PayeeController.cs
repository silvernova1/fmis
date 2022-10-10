using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Models.Accounting;
using fmis.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace fmis.Controllers.Accounting
{
    [Authorize(Policy = "AccountingAdmin")]
    public class PayeeController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public PayeeController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public class PayeeData
        {
            public int PayeeId { get; set; }
            public string PayeeDescription { get; set; }
            public string token { get; set; }
        }

        public class Many
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<Many> many_token { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated").ToListAsync());
            ViewBag.temp = json;
            //return View(await _MyDbContext.Payee.ToListAsync());
            return View("~/Views/Payee/Index.cshtml");
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayeeId,PayeeDescription")] Payee payee)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(payee);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payee);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            if (id == null)
            {
                return NotFound();
            }

            var payee = await _MyDbContext.Payee.FindAsync(id);
            if (payee == null)
            {
                return NotFound();
            }
            return View(payee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payee payee)
        {

            var pay = await _MyDbContext.Payee.Where(x => x.PayeeId == payee.PayeeId).AsNoTracking().FirstOrDefaultAsync();
            pay.PayeeDescription = payee.PayeeDescription;

            _MyDbContext.Update(pay);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var payee = await _MyDbContext.Payee.Where(p => p.PayeeId == ID).FirstOrDefaultAsync();
            _MyDbContext.Payee.Remove(payee);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePayee(List<PayeeData> data)
        {
            var data_holder = this._MyDbContext.Payee;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().PayeeDescription = item.PayeeDescription;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";

                    this._MyDbContext.SaveChanges();
                }
                else  //save
                {
                    var payee = new Payee(); //clear object
                    payee.PayeeDescription = item.PayeeDescription;
                    payee.status = "activated";
                    payee.token = item.token;

                    this._MyDbContext.Payee.Update(payee);
                    this._MyDbContext.SaveChanges();
                }
            }

            return Json(data);
        }


        // POST: Uacs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayee(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._MyDbContext.Payee;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _MyDbContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._MyDbContext.Payee;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);



        }


    }
}
