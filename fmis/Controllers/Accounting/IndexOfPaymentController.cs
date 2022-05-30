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

namespace fmis.Controllers.Accounting
{
    public class IndexOfPaymentController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public IndexOfPaymentController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        //COMMENT
        [Route("Accounting/IndexOfPayment")]
        public async Task<IActionResult> Index()
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



    }
}
