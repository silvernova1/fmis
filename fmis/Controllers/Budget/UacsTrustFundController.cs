using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using fmis.Filters;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Controllers.Budget
{
    public class UacsTrustFundController : Controller
    {

        private readonly UacsTrustFundContext _context;
        private readonly MyDbContext _MyDbContext;

        public UacsTrustFundController(UacsTrustFundContext context, MyDbContext MyDbContext)
        {
            _context = context;
            _MyDbContext = MyDbContext;
        }


        public class UacsDataTrustFund
        {
            public string Account_title { get; set; }
            public string Expense_code { get; set; }
            public string uacs_type { get; set; }
            public int UacsTrustFundId { get; set; }
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


        public async Task<IActionResult> PS_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "uacs_trust_fund", "ps_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.UacsTrustFund.Where(s => s.status == "activated" && s.uacs_type == "PS").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/UacsTrustFund/PS_Trust_Fund.cshtml");

        }

        public async Task<IActionResult> MOOE_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "uacs_trust_fund", "mooe_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.UacsTrustFund.Where(s => s.status == "activated" && s.uacs_type == "MOOE").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/UacsTrustFund/MOOE_Trust_Fund.cshtml");

        }

        public async Task<IActionResult> CO_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "uacs_trust_fund", "co_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.UacsTrustFund.Where(s => s.status == "activated" && s.uacs_type == "CO").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/UacsTrustFund/CO_Trust_Fund.cshtml");

        }

        // GET: Obligations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Uacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Account_title,Expense_code")] UacsTrustFund uacs_trustFund)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uacs_trustFund);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uacs_trustFund);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacsPSTrustFund(List<UacsDataTrustFund> data)
        {
            var data_holder = this._context.UacsTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "PS";

                    this._context.SaveChanges();
                }
                else if (item.Account_title != null || item.Expense_code != null) //save
                {
                    var uacs_trust_fund = new UacsTrustFund(); //clear object
                    uacs_trust_fund.Account_title = item.Account_title;
                    uacs_trust_fund.Expense_code = item.Expense_code;
                    uacs_trust_fund.status = "activated";
                    uacs_trust_fund.uacs_type = "PS";
                    uacs_trust_fund.token = item.token;

                    this._context.UacsTrustFund.Update(uacs_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacsMOOETrustFund(List<UacsDataTrustFund> data)
        {
            var data_holder = this._context.UacsTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "MOOE";

                    this._context.SaveChanges();
                }
                else if (item.Account_title != null || item.Expense_code != null) //save
                {
                    var uacs_trust_fund = new UacsTrustFund(); //clear object
                    uacs_trust_fund.Account_title = item.Account_title;
                    uacs_trust_fund.Expense_code = item.Expense_code;
                    uacs_trust_fund.status = "activated";
                    uacs_trust_fund.uacs_type = "MOOE";
                    uacs_trust_fund.token = item.token;

                    this._context.UacsTrustFund.Update(uacs_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacsCOTrustFund(List<UacsDataTrustFund> data)
        {
            var data_holder = this._context.UacsTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "CO";

                    this._context.SaveChanges();
                }
                else if (item.Account_title != null || item.Expense_code != null) //save
                {
                    var uacs_trust_fund = new UacsTrustFund(); //clear object
                    uacs_trust_fund.Account_title = item.Account_title;
                    uacs_trust_fund.Expense_code = item.Expense_code;
                    uacs_trust_fund.status = "activated";
                    uacs_trust_fund.uacs_type = "CO";
                    uacs_trust_fund.token = item.token;

                    this._context.UacsTrustFund.Update(uacs_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        // GET: Uacs/Edit/5
        public async Task<IActionResult> Edit(UacsTrustFund uacs_trustfund)
        {
            var uacs_trust_fund = await _MyDbContext.UacsTrustFund.Where(x => x.UacsTrustFundId == uacs_trustfund.UacsTrustFundId).AsNoTracking().FirstOrDefaultAsync();
            uacs_trust_fund.Account_title = uacs_trustfund.Account_title;
            uacs_trust_fund.Expense_code = uacs_trustfund.Expense_code;
            uacs_trust_fund.uacs_type = uacs_trustfund.uacs_type;
            uacs_trust_fund.status = uacs_trustfund.status;

            _context.Update(uacs_trust_fund);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUacsTrustFund(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.UacsTrustFund;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.UacsTrustFund;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}
