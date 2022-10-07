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

using fmis.Data.Accounting;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Accounting
{
    [Authorize(Policy = "AccountingAdmin")]
    public class IndexOfPaymentController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private readonly CategoryContext _CategoryContext;
        private readonly DeductionContext _DeductionContext;
        private readonly DvContext _DvContext;
        private readonly IndexofpaymentContext _IndexofpaymentContext;

        public IndexOfPaymentController(MyDbContext MyDbContext, CategoryContext categoryContext, DeductionContext deductionContext, DvContext dvContext, IndexofpaymentContext indexofpaymentContext)
        {
            _MyDbContext = MyDbContext;
            _CategoryContext = categoryContext;
            _DeductionContext = deductionContext;
            _DvContext = dvContext;
            _IndexofpaymentContext = indexofpaymentContext;
        }
        //COMMENT
        [Route("Accounting/IndexOfPayment")]
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            return View(await _MyDbContext.Indexofpayment
                .Include(x => x.Category)
                .Include(x => x.Dv)
                    .ThenInclude(x => x.Payee)
                .Include(x => x.indexDeductions)
                    .ThenInclude(x => x.Deduction).ToListAsync());


        }
        // GET: Create

        public IActionResult Create(int CategoryId, int DeductionId, int DvId)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.CategoryId = CategoryId;
            ViewBag.DeductionId = DeductionId;
            ViewBag.DvId = DvId;

            PopulateCategoryDropDownList();
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();

            IndexOfPayment newDv = new() { indexDeductions = new List<IndexDeduction>(7) };
            for (int x = 0; x < 7; x++)
            {
                newDv.indexDeductions.Add(new IndexDeduction());
            }
            return View(newDv);
        }
        // POST: Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IndexOfPayment indexOfPayment)
        {
            indexOfPayment.CreatedAt = DateTime.Now;
            indexOfPayment.UpdatedAt = DateTime.Now;
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");
            indexOfPayment.TotalDeduction = indexOfPayment.indexDeductions.Sum(x => x.Amount);
            indexOfPayment.NetAmount = indexOfPayment.GrossAmount - indexOfPayment.TotalDeduction;
            if (ModelState.IsValid)
            {
                indexOfPayment.indexDeductions = indexOfPayment.indexDeductions.Where(x => x.DeductionId != 0 && x.Amount != 0).ToList();
                _MyDbContext.Add(indexOfPayment);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(indexOfPayment);
        }

        private void PopulateCategoryDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Category
                        orderby d.CategoryId
                        select d;
            ViewBag.CategoryId = new SelectList(Query, "CategoryId", "CategoryDescription", selected);
        }
        private void PopulateDvDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Dv
                        orderby d.DvId
                        select d;
            ViewBag.DvId = new SelectList(Query, "DvId", "DvNo", selected);
        }
        private void PopulateDeductionDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Deduction
                        orderby d.DeductionId
                        select d;
            ViewBag.DeductionId = new SelectList(Query, "DeductionId", "DeductionDescription", selected);
        }
    }
}