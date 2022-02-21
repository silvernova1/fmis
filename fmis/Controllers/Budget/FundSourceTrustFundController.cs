using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Models.John;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using fmis.Filters;
using fmis.Models.silver;

namespace fmis.Controllers.Budget
{
    public class FundSourceTrustFundController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public FundSourceTrustFundController( MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
    
        }

        public class FundSourceAmountTrustFundData
        {
            public int FundSourceTrustFundId { get; set; }
            public int UacsId { get; set; }
            public decimal Amount { get; set; }
            public int Id { get; set; }
            public string FundSourceAmountTokenTrustFund { get; set; }
            public string FundSourceTokenTrustFund { get; set; }
            public int BudgetAllotmentTrustFundId { get; set; }
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


        public async Task<IActionResult> Index(int AllotmentClassId, int AppropriationId, int BudgetAllotmentTrustFundId)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "BudgetAllotment_trust_fund", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;

            var budget_allotment_trust_fund = await _MyDbContext.BudgetAllotmentTrustFund
            .Include(x => x.FundSourceTrustFund.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.RespoCenter)
            .Include(x => x.FundSourceTrustFund.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.Appropriation)
            .Include(x => x.FundSourceTrustFund.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.AllotmentClass)
            .Include(x => x.Yearly_reference)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BudgetAllotmentTrustFundId == BudgetAllotmentTrustFundId);

            return View(budget_allotment_trust_fund);

            
        }


        // GET: FundSource/Create
        public async Task<IActionResult> Create(int AllotmentClassId, int AppropriationId, int BudgetAllotmentTrustFundId)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "BudgetAllotment_trust_fund", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;

            string ac = AllotmentClassId.ToString();

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.UacsTrustFund.Where(x => x.uacs_type == ac).ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcTrustFundDropDownList();
            PopulateRespoDropDownList();
            PopulatePapTypeDropDownList();
            PopulateFundDropDownList();

            ViewBag.BudgetAllotmentTrustFundId = BudgetAllotmentTrustFundId;

            return View(); //open create
        }


        // POST: FundSource/Create 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FundSourceTrustFund fundSourceTrustFund, int PrexcId)
        {

            ViewBag.filter = new FilterSidebar("trust_fund", "BudgetAllotment_trust_fund", "");


            var result = _MyDbContext.PrexcTrustFund.Where(x => x.PrexcTrustFundId == PrexcId).First();

            var fundsource_amount_trust_fund = _MyDbContext.FundSourceAmountTrustFund.Where(f => f.FundSourceTokenTrustFund == fundSourceTrustFund.token).ToList();

            fundSourceTrustFund.BeginningBalance = fundsource_amount_trust_fund.Sum(x => x.BeginningBalance);
            fundSourceTrustFund.RemainingBalance = fundsource_amount_trust_fund.Sum(x => x.RemainingBalance);

            _MyDbContext.Add(fundSourceTrustFund);
            await _MyDbContext.SaveChangesAsync();

            fundsource_amount_trust_fund.ForEach(a => a.FundSourceTrustFundId = fundSourceTrustFund.FundSourceTrustFundId);
            this._MyDbContext.SaveChanges();

            return RedirectToAction("Index", "FundSourceTrustFund", new
            {
                AllotmentClassId = fundSourceTrustFund.AllotmentClassId,
                AppropriationId = fundSourceTrustFund.AppropriationId,
                BudgetAllotmentTrustFundId = fundSourceTrustFund.BudgetAllotmentTrustFundId
            });
        }


        public async Task<IActionResult> Edit(int fund_source_id_trust_fund)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "BudgetAllotment_trust_fund", "");

            var fundsource_trust_fund = _MyDbContext.FundSourceTrustFund.Where(x => x.FundSourceTrustFundId == fund_source_id_trust_fund)
                .Include(x => x.FundSourceAmountTrustFund.Where(x => x.status == "activated"))
                .FirstOrDefault();


            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcTrustFundDropDownList(fundsource_trust_fund.PrexcTrustFundId);
            PopulateRespoDropDownList();
            PopulatePapTypeDropDownList();
            PopulateFundDropDownList();

            return View(fundsource_trust_fund);
        }

        [HttpPost]
        public IActionResult SaveFundSourceAmountTrustFund(List<FundSourceAmountTrustFundData> data)
        {
            var data_holder = _MyDbContext.FundSourceAmountTrustFund;

            foreach (var item in data)
            {
                var fundsource_amount_trust_fund = new FundSourceAmountTrustFund(); //CLEAR OBJECT
                if (data_holder.Where(s => s.FundSourceAmountTokenTrustFund == item.FundSourceAmountTokenTrustFund).FirstOrDefault() != null) //CHECK IF EXIST
                    fundsource_amount_trust_fund = data_holder.Where(s => s.FundSourceAmountTokenTrustFund == item.FundSourceAmountTokenTrustFund).FirstOrDefault();

                fundsource_amount_trust_fund.FundSourceTrustFundId = item.FundSourceTrustFundId == 0 ? null : item.FundSourceTrustFundId;
                fundsource_amount_trust_fund.BudgetAllotmentTrustFundId = item.BudgetAllotmentTrustFundId;
                fundsource_amount_trust_fund.UacsId = item.UacsId;
                fundsource_amount_trust_fund.BeginningBalance = item.Amount;
                fundsource_amount_trust_fund.RemainingBalance = item.Amount;
                fundsource_amount_trust_fund.status = "activated";
                fundsource_amount_trust_fund.FundSourceAmountTokenTrustFund = item.FundSourceAmountTokenTrustFund;
                fundsource_amount_trust_fund.FundSourceTokenTrustFund = item.FundSourceTokenTrustFund;
                _MyDbContext.FundSourceAmountTrustFund.Update(fundsource_amount_trust_fund);
                this._MyDbContext.SaveChanges();
            }

            return Json(data);
        }


        //POST
        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.PrexcTrustFund.ToList();
            return Json(branches.Where(x => x.PrexcTrustFundId == id).ToList());
        }


        private void PopulatePrexcTrustFundDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _MyDbContext.PrexcTrustFund
                                   orderby d.pap_title
                                   select d;
            ViewBag.PrexcTrustFundId = new SelectList((from s in _MyDbContext.PrexcTrustFund.ToList()
                                              select new
                                              {
                                                  PrexcTrustFundId = s.PrexcTrustFundId,
                                                  prexc = s.pap_title
                                              }),
                                       "PrexcTrustFundId",
                                       "prexc",
                                       null);

        }



        private void PopulateRespoDropDownList()
        {
            ViewBag.RespoId = new SelectList((from s in _MyDbContext.RespoCenter.ToList()
                                              select new
                                              {
                                                  RespoId = s.RespoId,
                                                  respo = s.Respo
                                              }),
                                     "RespoId",
                                     "respo",
                                     null);

        }



        //POPULATE PAP TYPE
        private void PopulatePapTypeDropDownList(object selectedDepartment = null)
        {

            ViewBag.PapTypeId = new SelectList((from s in _MyDbContext.Prexc.ToList()
                                                select new
                                                {
                                                    PrexcId = s.Id,
                                                    prexc = s.pap_type
                                                }),
                                       "PrexcId",
                                       "prexc",
                                       null);

        }

        //POPULATE PAP TYPE
        private void PopulateFundDropDownList()
        {

            ViewBag.FundId = new SelectList((from s in _MyDbContext.Fund.ToList()
                                             select new
                                             {
                                                 FundId = s.FundId,
                                                 FundDescription = s.Fund_description
                                             }),
                                       "FundId",
                                       "FundDescription",
                                       null);

        }

    }
}
