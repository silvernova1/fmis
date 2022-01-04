﻿using System;
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

namespace fmis.Controllers

{
    public class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;
        private readonly UacsContext _uContext;
        private readonly Budget_allotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly MyDbContext _MyDbContext;

        public Sub_allotmentController(Sub_allotmentContext context, UacsContext uContext, Budget_allotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
        }

        public class Suballotment_amountData
        {
            public int Id { get; set; }
            public int UacsId { get; set; }
            public decimal Amount { get; set; }
            public string suballotment_amount_token { get; set; }
            public string suballotment_token { get; set; }
            public int SubAllotmentId { get; set; }
            public int BudgetId { get; set; }
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
        public async Task<IActionResult> Index(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return View(await _context.Sub_allotment.ToListAsync());
        }

        // GET:Sub_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_Allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.SubAllotmentId == id);
            if (sub_Allotment == null)
            {
                return NotFound();
            }
            return View(sub_Allotment);
        }

        // GET: Sub_allotment/Create
        public IActionResult Create(int budget_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            var uacs_data = JsonSerializer.Serialize(_MyDbContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;

            PopulatePrexcDropDownList();

            ViewBag.budget_id = budget_id;

            return View();
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
                suballotment_amount.BudgetId = item.BudgetId;
                suballotment_amount.UacsId = item.UacsId;
                suballotment_amount.Amount = item.Amount;
                suballotment_amount.status = "activated";
                suballotment_amount.suballotment_amount_token = item.suballotment_amount_token;
                suballotment_amount.suballotment_token = item.suballotment_token;
                _MyDbContext.Suballotment_amount.Update(suballotment_amount);
                this._MyDbContext.SaveChanges();
            }

            return Json(data);
        }
   
        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubId,Suballotment_code,Suballotment_title,Responsibility_number,Description,Budget_allotmentBudgetAllotmentId,prexcId,token")] Sub_allotment sub_Allotment, int? id, Suballotment_amount Subsamount, Budget_allotment budget, int budget_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            ViewBag.budget_id = budget_id;

            var suballotment_amount = _MyDbContext.Suballotment_amount.Where(f => f.suballotment_token == sub_Allotment.token).ToList();

            sub_Allotment.Beginning_balance = suballotment_amount.Sum(x => x.Amount);
            sub_Allotment.Remaining_balance = suballotment_amount.Sum(x => x.Amount);

            _context.Add(sub_Allotment);
            await _context.SaveChangesAsync();

            suballotment_amount.ForEach(a => a.SubAllotmentId = sub_Allotment.SubAllotmentId);
            await _MyDbContext.SaveChangesAsync();

            return RedirectToAction("Suballotment", "Budget_allotments", new { budget_id = sub_Allotment.Budget_allotmentBudgetAllotmentId });
        }
        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int budget_id, int sub_allotment_id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            var suballotment = _MyDbContext.Sub_allotment.Where(x => x.SubAllotmentId == sub_allotment_id)
                .Include(x => x.SubAllotmentAmounts.Where(x => x.status == "activated"))
                .FirstOrDefault();

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcDropDownList(suballotment.prexcId);
            return View(suballotment);
        }
        
        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.PrexcId = new SelectList((from s in _pContext.Prexc.ToList()
                                              select new
                                              {
                                                  prexcId = s.Id,
                                                  prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                              }),
                                  "prexcId",
                                  "prexc",
                                   null);

        }
        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sub_allotment sub_allotment, Suballotment_amount Subsamount)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var suballotment_amount = _MyDbContext.Suballotment_amount.Where(f => f.SubAllotmentId == sub_allotment.SubAllotmentId).ToList();
            var beginning_balance = suballotment_amount.Sum(x => x.Amount);
            var remaining_balance = suballotment_amount.Sum(x => x.Amount);

            var suballotment_data = _MyDbContext.Sub_allotment.Where(s => s.SubAllotmentId == sub_allotment.SubAllotmentId).FirstOrDefault();
            suballotment_data.Suballotment_title = suballotment_data.Suballotment_title;
            suballotment_data.Description = suballotment_data.Description;
            suballotment_data.Suballotment_code = suballotment_data.Suballotment_code;
            suballotment_data.Responsibility_number = suballotment_data.Responsibility_number;
            suballotment_data.Beginning_balance = beginning_balance;
            suballotment_data.Remaining_balance = remaining_balance;

            _context.Update(suballotment_data);
            await _context.SaveChangesAsync();

            return RedirectToAction("Suballotment", "Budget_allotments", new { budget_id = suballotment_data.Budget_allotmentBudgetAllotmentId });
        }
       
        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id, int? BudgetId, int budget_id)
        {
            ViewBag.BudgetId = BudgetId;
            ViewBag.budget_id = budget_id;

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_Allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.SubAllotmentId == id);
            if (sub_Allotment == null)
            {
                return NotFound();
            }

            return View(sub_Allotment);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSuballotment_amount(DeleteData data)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (data.many_token.Count > 1)
            {
                var data_holder = _MyDbContext.Suballotment_amount;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.suballotment_amount_token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.suballotment_amount_token == many.many_token).FirstOrDefault().suballotment_amount_token = many.many_token;
                    await _MyDbContext.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = _MyDbContext.Suballotment_amount;
                data_holder.Where(s => s.suballotment_amount_token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.suballotment_amount_token == data.single_token).FirstOrDefault().suballotment_amount_token = data.single_token;

                await _MyDbContext.SaveChangesAsync();
            }

            return Json(data);
        }
        /* {
             ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
             if (data.many_token.Count > 1)
             {
                 var data_holder = this._MyDbContext.Suballotment_amount;
                 foreach (var many in data.many_token)
                 {
                     data_holder.Where(s => s.suballotment_amount_token == many.many_token).FirstOrDefault().status = "deactivated";
                     data_holder.Where(s => s.suballotment_amount_token == many.many_token).FirstOrDefault().suballotment_amount_token = many.many_token;
                     await this. _context.SaveChangesAsync();
                 }
             }
             else
             {
                 var data_holder =this. _MyDbContext.Suballotment_amount;
                 data_holder.Where(s => s.suballotment_amount_token == data.single_token).FirstOrDefault().status = "deactivated";
                 data_holder.Where(s => s.suballotment_amount_token == data.single_token).FirstOrDefault().suballotment_amount_token = data.single_token;

                 await this._context.SaveChangesAsync();
             }

             return Json(data);
         }
 */
        // POST: Sub_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var sub_Allotment = await _context.Sub_allotment.FindAsync(id);
            _context.Sub_allotment.Remove(sub_Allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Suballotment", "Budget_allotments", new { budget_id = sub_Allotment.Budget_allotmentBudgetAllotmentId });
        }
        private bool Sub_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return _context.Sub_allotment.Any(e => e.SubAllotmentId == id);
        }
    }
}