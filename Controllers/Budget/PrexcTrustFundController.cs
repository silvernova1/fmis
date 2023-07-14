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
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget
{
    [Authorize(Policy = "BudgetAdmin")]
    public class PrexcTrustFundController : Controller
    {
        private readonly PrexcTrustFundContext _context;
        private readonly MyDbContext _MyDbContext;

        public PrexcTrustFundController(PrexcTrustFundContext context, MyDbContext MyDbContext)
        {
            _context = context;
            _MyDbContext = MyDbContext;
        }

        public class PrexcTrustFundData
        {
            public int PrexcTrustFundId { get; set; }
            public string pap_title { get; set; }
            public string pap_code1 { get; set; }
            public string pap_type { get; set; }
            public string status { get; set; }
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


        // GET: GAS
        [Route("PapTrustFund/GAS")]
        public IActionResult GAS_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "pap_trust_fund", "gas_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.PrexcTrustFund.Where(s => s.status == "activated" && s.pap_type == "GAS").ToList());
            ViewBag.temp = json;
            return View("~/Views/PrexcTrustFund/GAS_Trust_Fund.cshtml");
        }

        // GET: STO
        [Route("PapTrustFund/STO")]
        public IActionResult STO_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "pap_trust_fund", "sto_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.PrexcTrustFund.Where(s => s.status == "activated" && s.pap_type == "STO").ToList());
            ViewBag.temp = json;
            return View("~/Views/PrexcTrustFund/STO_Trust_Fund.cshtml");
        }

        // GET: OPERATION
        [Route("PapTrustFund/OPERATIONS")]
        public IActionResult OPERATIONS_Trust_Fund()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "pap_trust_fund", "operations_trust_fund");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.PrexcTrustFund.Where(s => s.status == "activated" && s.pap_type == "OPERATIONS").ToList());
            ViewBag.temp = json;
            return View("~/Views/PrexcTrustFund/OPERATIONS_Trust_Fund.cshtml");
        }

        // GET: Prexc/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePrexcGASTrustFund(List<PrexcTrustFundData> data)
        {
            var data_holder = this._context.PrexcTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "GAS";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_code1 != null || item.pap_type != null) //save
                {
                    var prexc_trust_fund = new PrexcTrustFund(); //clear object
                    prexc_trust_fund.PrexcTrustFundId = item.PrexcTrustFundId;
                    prexc_trust_fund.pap_title = item.pap_title;
                    prexc_trust_fund.pap_code1 = item.pap_code1;
                    prexc_trust_fund.pap_type = "GAS";
                    prexc_trust_fund.status = "activated";
                    prexc_trust_fund.token = item.token;

                    this._context.PrexcTrustFund.Update(prexc_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePrexcSTOTrustFund(List<PrexcTrustFundData> data)
        {
            var data_holder = this._context.PrexcTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "STO";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_code1 != null || item.pap_type != null) //save
                {
                    var prexc_trust_fund = new PrexcTrustFund(); //clear object
                    prexc_trust_fund.PrexcTrustFundId = item.PrexcTrustFundId;
                    prexc_trust_fund.pap_title = item.pap_title;
                    prexc_trust_fund.pap_code1 = item.pap_code1;
                    prexc_trust_fund.pap_type = "STO";
                    prexc_trust_fund.status = "activated";
                    prexc_trust_fund.token = item.token;

                    this._context.PrexcTrustFund.Update(prexc_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePrexcOPERATIONSTrustFund(List<PrexcTrustFundData> data)
        {
            var data_holder = this._context.PrexcTrustFund;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "OPERATIONS";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_title != null || item.pap_type != null) //save
                {
                    var prexc_trust_fund = new PrexcTrustFund(); //clear object
                    prexc_trust_fund.PrexcTrustFundId = item.PrexcTrustFundId;
                    prexc_trust_fund.pap_title = item.pap_title;
                    prexc_trust_fund.pap_code1 = item.pap_code1;
                    prexc_trust_fund.pap_type = "OPERATIONS";
                    prexc_trust_fund.status = "activated";
                    prexc_trust_fund.token = item.token;

                    this._context.PrexcTrustFund.Update(prexc_trust_fund);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrexcTrustFund prexc_trustFund)
        {
            var prexc_trust_fund = await _context.PrexcTrustFund.Where(x => x.PrexcTrustFundId == prexc_trustFund.PrexcTrustFundId).AsNoTracking().FirstOrDefaultAsync();
            prexc_trust_fund.pap_title = prexc_trustFund.pap_title;
            prexc_trust_fund.pap_code1 = prexc_trustFund.pap_code1;
            prexc_trust_fund.pap_type = prexc_trustFund.pap_type;
            prexc_trust_fund.status = prexc_trustFund.status;

            _context.Update(prexc_trust_fund);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePrexcTrustFund(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.PrexcTrustFund;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.PrexcTrustFund;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

    }
}
