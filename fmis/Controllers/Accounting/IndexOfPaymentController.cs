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
using fmis.Areas.Identity.Data;
using fmis.Data.Accounting;

namespace fmis.Controllers.Accounting
{
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
        public async Task<IActionResult> Index(int CategoryId, int DeductionId, int DvId, int IndexOfPaymentId)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            PopulateIndexOfPaymentDropDownList();
            PopulatePayeeDropDownList();
            PopulateDeductionDropDownList();
            PopulateDvDropDownList();

            return View(await _MyDbContext.Category.ToListAsync());
        }


        private void PopulateIndexOfPaymentDropDownList()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.CategoryId = new SelectList((from s in _MyDbContext.Category.ToList()
                                              select new
                                              {
                                                  CategoryId = s.CategoryId,
                                                  CategotyDescription = s.CategoryDescription,
                                    
                                              }),
                                       "CategoryId",
                                       "CategotyDescription",
                                       null);

        }

        private void PopulatePayeeDropDownList()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.PayeeId = new SelectList((from s in _MyDbContext.Payee.ToList()
                                                 select new
                                                 {
                                                     PayeeId = s.PayeeId,
                                                     PayeeDescription = s.PayeeDescription,

                                                 }),
                                       "PayeeId",
                                       "PayeeDescription",
                                       null);

        }

        private void PopulateDeductionDropDownList()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.DeductionId = new SelectList((from s in _MyDbContext.Deduction.ToList()
                                              select new
                                              {
                                                  DeductionId = s.DeductionId,
                                                  DeductionDescription = s.DeductionDescription,

                                              }),
                                       "DeductionId",
                                       "DeductionDescription",
                                       null);

        }

        private void PopulateDvDropDownList()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.DvId = new SelectList((from s in _MyDbContext.Dv.ToList()
                                                  select new
                                                  {
                                                      DvId = s.DvId,
                                                      DvNo = s.DvNo,
                                                      Payee = s.Payee,

                                                  }),
                                       "DvId",
                                       "DvNo",
                                       "Payee",
                                       null);

        }
        // GET: Create
        public IActionResult Create(int CategoryId, int DeductionId, int DvId, int IndexOfPaymentId)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");
            ViewBag.CategoryId = CategoryId;
            ViewBag.DeductionId = DeductionId;
            ViewBag.DvId = DvId;

            PopulateIndexOfPaymentDropDownList();
            PopulatePayeeDropDownList();
            PopulateDeductionDropDownList();
            PopulateDvDropDownList();

            /* ViewBag.IndexOfPaymentId = IndexOfPaymentId;*/

            return View(); //open create
        }
        // POST: Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< HEAD
        public  IActionResult Create(IndexOfPayment indexOfPayment)
        {
            indexOfPayment.CreatedAt = DateTime.Now;
            indexOfPayment.UpdatedAt = DateTime.Now;
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            var cat_desc = _MyDbContext.Category.Where(f => f.CategoryId == indexOfPayment.CategoryId).FirstOrDefault().CategoryDescription;

            indexOfPayment.CategoryDescription = cat_desc;

=======
        public IActionResult Create(IndexOfPayment indexOfPayment)
        {
>>>>>>> 3772890c2b9b3e6f22114bd63f5c7c04675c94eb
            _IndexofpaymentContext.Add(indexOfPayment);
            _IndexofpaymentContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
