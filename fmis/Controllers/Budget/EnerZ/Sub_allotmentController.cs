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
            public int FundsId { get; set; }
            public int Id { get; set; }
            public string Expenses { get; set; }
            public string Amount { get; set; }
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

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            return View(sub_allotment);
        }

        // GET: Sub_allotment/Create
        public IActionResult Create(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var json = JsonSerializer.Serialize(_MyDbContext.Suballotment_amount
                .Where(f => f.Sub_allotment.SubId == id).ToList());
            ViewBag.temp = json;
            var uacs_data = JsonSerializer.Serialize(_MyDbContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;

            PopulatePrexcsDropDownList();

            ViewBag.BudgetId = id;
            ViewBag.FundsId = id;

            List<Prexc> p = new List<Prexc>();

            p = (from c in _pContext.Prexc select c).ToList();
            p.Insert(0, new Prexc { Id = 0, pap_title = "--Select PREXC--" });

            ViewBag.message = p;

            var sub_allotment = _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSuballotment_amount([Bind("SubId,Prexc_code,Suballotment_code,Suballotment_title,Responsibility_number,Description,Budget_allotmentBudgetAllotmentId,Id")] Sub_allotment sub_allotment, List<Suballotment_amount> data, int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            ViewBag.BudgetId = id;
            var data_holder = _MyDbContext.Suballotment_amount;

            foreach (var item in data)
            {


                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {

                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expenses= item.Expenses;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Amount = item.Amount;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";

                    this._MyDbContext.SaveChanges();
                }
                else
                {
                    var suballotment = new Suballotment_amount();

                    suballotment.Id = item.Id;
                    suballotment.Id = item.Id;
                    suballotment.Expenses = item.Expenses;
                    suballotment.Amount = item.Amount;
                    suballotment.status = "activated";
                    suballotment.token = item.token;

                    _MyDbContext.Suballotment_amount.Update(suballotment);
                    this._MyDbContext.SaveChanges();
                }

                return Json(data);

            }


            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(sub_allotment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Fundsource", "Budget_allotments", new { id = sub_allotment.Budget_allotmentBudgetAllotmentId });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(sub_allotment.Id);

            return RedirectToAction("Fundsource", "Budget_allotments", new { id = sub_allotment.Budget_allotmentBudgetAllotmentId });


        }

        /* [HttpPost]
         public IActionResult SaveSuballotment_amount(List<Suballotment_amountData> data)
         {
             ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
             var data_holder = this._MyDbContext.Suballotment_amount.Include(c => c.Sub_allotment);

             foreach (var item in data)
             {
                 if (item.Id == 0)
                 {
                     var suballotment_amount = new Suballotment_amount();

                     suballotment_amount.Id = item.Id;
                     suballotment_amount.Expenses = item.Expenses;
                     *//*suballotment_amount.Amount = item.Amount;*//*

                     this._MyDbContext.Suballotment_amount.Update(suballotment_amount);
                     this._MyDbContext.SaveChanges();
                 }
                 *//*else
                 {
                     data_holder.Find(item.Id).FundsId = item.FundsId;
                     data_holder.Find(item.Id).Account_title = item.Account_title;
                     data_holder.Find(item.Id).Amount = item.Amount;

                     this._MyDbContext.SaveChanges();
                 }*//*
             }

             return Json(data);
         }*/

        // POST: Sub_allotment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubId,Prexc_code,Suballotment_code,Suballotment_title,Responsibility_number,Description,Budget_allotmentBudgetAllotmentId,Id")] Sub_allotment sub_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(sub_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });
                }
            }
            catch (RetryLimitExceededException)
            {

                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(sub_allotment.Id);

            return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });
        }

        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int? id, int? BudgetId)
        {

            ViewBag.BudgetId = BudgetId;

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            PopulatePrexcsDropDownList(sub_allotment.Id);

           /* return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });*/

            return View(sub_allotment);
        }

        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.Id = new SelectList((from s in _pContext.Prexc.ToList()
                                         select new
                                         {
                                             Id = s.Id,
                                             prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                         }),
       "Id",
       "prexc",
       null);

        }

        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sub_allotment sub_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_allotmentExists(sub_allotment.SubId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              /*  return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });*/
            }
            return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });
        }

        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // POST:Sub_allotment/Delete/5
        [HttpPost]
        public IActionResult DeleteSuballotment_amount(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var suballotment_amount = this._MyDbContext.Suballotment_amount.Find(id);
            this._MyDbContext.Suballotment_amount.Remove(suballotment_amount);
            this._MyDbContext.SaveChangesAsync();
            return Json(id);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteSuballotment_amount(DeleteData data)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (data.many_token.Count > 1)
            {
                var data_holder = _MyDbContext.Suballotment_amount;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = _MyDbContext.Suballotment_amount;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            _context.Sub_allotment.Remove(sub_allotment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Suballotment", "Budget_allotments", new { id = "1" });

        }

        private bool Sub_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return _context.Sub_allotment.Any(e => e.SubId == id);
        }
    }
}