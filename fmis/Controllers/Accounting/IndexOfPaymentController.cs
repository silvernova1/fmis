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

        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            PopulateIndexOfPaymenyDropDownList();

            return View(await _MyDbContext.Category.ToListAsync());
        }


        private void PopulateIndexOfPaymenyDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.IndexOfPaymentId = new SelectList((from s in _MyDbContext.Category.ToList()
                                              select new
                                              {
                                                  CategoryId = s.CategoryId,
                                                  CategotyDescription = s.CategoryDescription,
                                    
                                              }),
                                       "CategoryId",
                                       "CategotyDescription",
                                       null);

        }

    }
}
