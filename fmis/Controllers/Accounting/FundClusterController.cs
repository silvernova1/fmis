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
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Accounting
{
    [Authorize(Policy = "AccountingAdmin")]
    public class FundClusterController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public FundClusterController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public async Task <IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "fund_cluster", "");
            return View(await _MyDbContext.FundCluster.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "fund_cluster", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundClusterId,FundClusterDescription")] FundCluster fundcluster)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "fund_cluster", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(fundcluster);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fundcluster);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "fund_cluster", "");
            if (id == null)
            {
                return NotFound();
            }

            var fund_cluster = await _MyDbContext.FundCluster.FindAsync(id);
            if (fund_cluster == null)
            {
                return NotFound();
            }
            return View(fund_cluster);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FundCluster fundcluster)
        {

            var fund_cluster = await _MyDbContext.FundCluster.Where(x => x.FundClusterId == fundcluster.FundClusterId).AsNoTracking().FirstOrDefaultAsync();
            fund_cluster.FundClusterDescription = fundcluster.FundClusterDescription;

            _MyDbContext.Update(fund_cluster);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var fund_cluster = await _MyDbContext.FundCluster.Where(p => p.FundClusterId == ID).FirstOrDefaultAsync();
            _MyDbContext.FundCluster.Remove(fund_cluster);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
