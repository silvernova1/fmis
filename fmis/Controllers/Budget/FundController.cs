using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using fmis.Filters;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Controllers
{
    public class FundController : Controller
    {
        private readonly FundContext _fundContext;

        public FundController(FundContext fundContext)
        {
            _fundContext = fundContext;
        }

        public async  Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Fund", "");
            return View(await _fundContext.Fund.ToListAsync());
        }


        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Fund", "");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundId,Fund_description,Fund_code_current, Fund_code_conap ")] Fund fund)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Fund", "");
            if (ModelState.IsValid)
            {
                _fundContext.Add(fund);
                await _fundContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fund);
        }


        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Fund", "");
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _fundContext.Fund.FindAsync(id);
            if (fund == null)
            {
                return NotFound();
            }
            return View(fund);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Fund fund)
        {

            var funD = await _fundContext.Fund.Where(x => x.FundId == fund.FundId).AsNoTracking().FirstOrDefaultAsync();
            funD.Fund_description = funD.Fund_description;


            _fundContext.Update(funD);
            await _fundContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var fund = await _fundContext.Fund.Where(p => p.FundId == ID).FirstOrDefaultAsync();
            _fundContext.Fund.Remove(fund);
            await _fundContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
