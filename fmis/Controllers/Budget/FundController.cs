using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using fmis.Filters;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers
{
    [Authorize(Policy = "BudgetAdmin")]
    public class FundController : Controller
    {
        private readonly FundContext _fundContext;
        private readonly MyDbContext _MyDbContext;

        public FundController(FundContext fundContext, MyDbContext MyDbContext)
        {
            _fundContext = fundContext;
            _MyDbContext = MyDbContext;
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
                /*if (fund.Fund_code_current == "101101")
                {
                    fund.AppropriationID = 1;
                }
            else if(fund.Fund_code_conap == "102101")
                {
                    fund.AppropriationID = 2;
                }*/
            
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

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Fund fund)
        {


            var funds = await _fundContext.Fund.Where(x => x.FundId == fund.FundId).AsNoTracking().FirstOrDefaultAsync();
            funds.Fund_code_conap = fund.Fund_code_conap;
            funds.Fund_code_current = fund.Fund_code_current;
            funds.Fund_description = fund.Fund_description;

            _fundContext.Update(funds);
            await _fundContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var fund = await _fundContext.Fund.Include(x=>x.Sub_Allotments).ThenInclude(x=>x.SubAllotmentAmounts).Where(p => p.FundId == ID).FirstOrDefaultAsync();
            _fundContext.Fund.Remove(fund);
            await _fundContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}