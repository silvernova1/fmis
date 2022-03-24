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

namespace fmis.Controllers

{
    public class SubAllotmentController : Controller
    {
        private readonly SubAllotmentContext _context;
        private readonly Suballotment_amountContext _subAllotmentAmountContext;
        private readonly UacsContext _uContext;
        private readonly BudgetAllotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly MyDbContext _MyDbContext;
      

        public SubAllotmentController(SubAllotmentContext context, UacsContext uContext, BudgetAllotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext, Suballotment_amountContext subAllotmentAmountContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
            _subAllotmentAmountContext = subAllotmentAmountContext;

        }

        public class Suballotment_amountData
        {
            public int Id { get; set; }
            public int UacsId { get; set; }
            public decimal Amount { get; set; }
            public string suballotment_amount_token { get; set; }
            public string suballotment_token { get; set; }
            public int SubAllotmentId { get; set; }
            public int BudgetAllotmentId { get; set; }
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

        // GET:Sub_allotment
        public async Task<IActionResult> Index(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
          
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment","");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            var budget_allotment = await _MyDbContext.Budget_allotments
            .Include(x => x.SubAllotment.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.RespoCenter)
            .Include(x => x.SubAllotment.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.Appropriation)
            .Include(x => x.SubAllotment.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.AllotmentClass)
            .Include(x => x.Yearly_reference)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BudgetAllotmentId == BudgetAllotmentId);


            return View(budget_allotment);
        }

      
        // GET: Sub_allotment/Create
        public async Task <IActionResult> Create(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());

            ViewBag.uacs = uacs_data;
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            PopulatePrexcDropDownList();
            PopulateRespoDropDownList();
            PopulateFundDropDownList();
            

            return View(); //open create
        }


        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubAllotment subAllotment)

        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");


            var sub_allotment_amount = _MyDbContext.Suballotment_amount.Where(f => f.suballotment_token == subAllotment.token).ToList();

            subAllotment.Beginning_balance = sub_allotment_amount.Sum(x => x.beginning_balance);
            subAllotment.Remaining_balance = sub_allotment_amount.Sum(x => x.remaining_balance);


            var prexcID = _MyDbContext.SubAllotment.Where(x => x.prexcId == subAllotment.prexcId).FirstOrDefault();

            _context.Add(subAllotment);
            
            await _context.SaveChangesAsync();

            sub_allotment_amount.ForEach(a => a.SubAllotmentId = subAllotment.SubAllotmentId);
            this._MyDbContext.SaveChanges();

            return RedirectToAction("Index", "SubAllotment", new
            {
                AllotmentClassId = subAllotment.AllotmentClassId,
                AppropriationId = subAllotment.AppropriationId,
                BudgetAllotmentId = subAllotment.BudgetAllotmentId
            });
        }

        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int sub_allotment_id, int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            var suballotment = _MyDbContext.SubAllotment.Where(x => x.SubAllotmentId == sub_allotment_id)
                .Include(x => x.SubAllotmentAmounts.Where(x => x.status == "activated"))
                .FirstOrDefault();

            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcDropDownList(suballotment.prexcId);
            PopulateRespoDropDownList();
            PopulateFundDropDownList();

            return View(suballotment);
        }


        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubAllotment subAllotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var sub_allotment_amount = await _MyDbContext.Suballotment_amount.Where(f => f.SubAllotmentId == subAllotment.SubAllotmentId && f.status == "activated").AsNoTracking().ToListAsync();
            var beginning_balance = sub_allotment_amount.Sum(x => x.beginning_balance);
            var remaining_balance = sub_allotment_amount.Sum(x => x.remaining_balance);

            var sub_allotment_data = await _MyDbContext.SubAllotment.Where(s => s.SubAllotmentId == subAllotment.SubAllotmentId).AsNoTracking().FirstOrDefaultAsync();
            sub_allotment_data.prexcId = subAllotment.prexcId;
            sub_allotment_data.FundId = subAllotment.FundId;
            sub_allotment_data.Suballotment_title = subAllotment.Suballotment_title;
            sub_allotment_data.Description = subAllotment.Description;
            sub_allotment_data.Suballotment_code = subAllotment.Suballotment_code;
            sub_allotment_data.PapType = subAllotment.PapType;
            sub_allotment_data.RespoId = subAllotment.RespoId;
            sub_allotment_data.Beginning_balance = beginning_balance;
            sub_allotment_data.Remaining_balance = remaining_balance;

            _MyDbContext.Update(sub_allotment_data);
            await _MyDbContext.SaveChangesAsync();

            return RedirectToAction("Index", "SubAllotment", new
            {
                AllotmentClassId = subAllotment.AllotmentClassId,
                AppropriationId = subAllotment.AppropriationId,
                BudgetAllotmentId = subAllotment.BudgetAllotmentId
            });
        }


        [HttpPost]
        public IActionResult SaveSuballotment_amount(List<Suballotment_amountData> data)
        {
            var data_holder = _MyDbContext.Suballotment_amount;

            foreach (var item in data)
            {
                var suballotment_amount = new Suballotment_amount(); //CLEAR OBJECT
                if (data_holder.Where(s => s.suballotment_amount_token == item.suballotment_amount_token).FirstOrDefault() != null) //CHECK IF EXIST
                    suballotment_amount = data_holder.Where(s => s.suballotment_amount_token == item.suballotment_amount_token).FirstOrDefault();

                suballotment_amount.SubAllotmentId = item.SubAllotmentId == 0 ? null : item.SubAllotmentId;
                suballotment_amount.BudgetAllotmentId = item.BudgetAllotmentId;
                suballotment_amount.UacsId = item.UacsId;
                suballotment_amount.beginning_balance = item.Amount;
                suballotment_amount.remaining_balance = item.Amount;
                suballotment_amount.status = "activated";
                suballotment_amount.suballotment_amount_token = item.suballotment_amount_token;
                suballotment_amount.suballotment_token = item.suballotment_token;
                _MyDbContext.Suballotment_amount.Update(suballotment_amount);
                this._MyDbContext.SaveChanges();

            }

            return Json(data);
        }

        //POST
        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.Prexc.ToList();
            return Json(branches.Where(x => x.Id == id).ToList());
        }



        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.PrexcId = new SelectList((from s in _pContext.Prexc.ToList()
                                              select new
                                              {
                                                  prexcId = s.Id,
                                                  prexc = s.pap_title,
                                                  pap_type = s.pap_type
                                              }),
                                  "prexcId",
                                  "prexc",
                                  "pap_type",
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

        private void PopulateAppropriationDropDownList()
        {
            ViewBag.AppropriationId = new SelectList((from s in _MyDbContext.Appropriation.ToList()
                                                      select new
                                                      {
                                                          AppropriationId = s.AppropriationId,
                                                          AppropriationSource = s.AppropriationSource
                                                      }),
                                     "AppropriationId",
                                     "AppropriationSource",
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

     
        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id, int? BudgetId, int budget_id)
        {
            ViewBag.BudgetId = BudgetId;
            ViewBag.budget_id = budget_id;

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            if (id == null)
            {
                return NotFound();
            }

            var subAllotment = await _MyDbContext.SubAllotment
                .FirstOrDefaultAsync(m => m.SubAllotmentId == id);
            if (subAllotment == null)
            {
                return NotFound();
            }

            return View(subAllotment);
        }
        
        // POST: Sub_allotment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSuballotment_amount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                {
                    var sub_allotment_amount = _MyDbContext.Suballotment_amount.FirstOrDefault(s => s.suballotment_amount_token == many.many_token);
                    sub_allotment_amount.status = "deactivated";

                    _MyDbContext.Suballotment_amount.Update(sub_allotment_amount);
                    await _MyDbContext.SaveChangesAsync();

                    var sub_allotment_update = await _MyDbContext.SubAllotment.AsNoTracking().FirstOrDefaultAsync(s => s.token == sub_allotment_amount.suballotment_token);
                    sub_allotment_update.Remaining_balance -= sub_allotment_amount.beginning_balance;

                    //detach para ma calculate ang multiple delete
                    var local = _MyDbContext.Set<SubAllotment>()
                            .Local
                            .FirstOrDefault(entry => entry.token.Equals(sub_allotment_amount.suballotment_token));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _MyDbContext.Entry(local).State = EntityState.Detached;
                    }
                    // set Modified flag in your entry
                    _MyDbContext.Entry(sub_allotment_update).State = EntityState.Modified;
                    //end detach

                    _MyDbContext.SubAllotment.Update(sub_allotment_update);
                    _MyDbContext.SaveChanges();
                }
            }
            else
            {
                var sub_amount = _MyDbContext.Suballotment_amount.FirstOrDefault(s => s.suballotment_amount_token == data.single_token);
                sub_amount.status = "deactivated";

                _MyDbContext.Suballotment_amount.Update(sub_amount);
                await _MyDbContext.SaveChangesAsync();

                var sub_update = await _MyDbContext.SubAllotment.AsNoTracking().FirstOrDefaultAsync(s => s.token == sub_amount.suballotment_token);
                sub_update.Remaining_balance -= sub_amount.beginning_balance;
                _MyDbContext.SubAllotment.Update(sub_update);
                _MyDbContext.SaveChanges();
            }

            return Json(data);
        }
        // POST: Sub_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var sub_Allotment = await _context.SubAllotment.FindAsync(id);
            _context.SubAllotment.Remove(sub_Allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Suballotment", "Budget_allotments", new { budget_id = sub_Allotment.BudgetAllotmentId });
        }
        private bool Sub_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            return _context.SubAllotment.Any(e => e.SubAllotmentId == id);
        }
    }
}