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
    [Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
    public class DeductionController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public DeductionController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        [Route("Accounting/Deduction")]
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "deduction", "");
            return View(await _MyDbContext.Deduction.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "deduction", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeductionId,DeductionDescription")] Deduction deduction)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "deduction", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(deduction);
                await _MyDbContext.SaveChangesAsync();
                await Task.Delay(500);
                return RedirectToAction(nameof(Index));
            }
            return View(deduction);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "deduction", "");
            if (id == null)
            {
                return NotFound();
            }

            var deduction = await _MyDbContext.Deduction.FindAsync(id);
            if (deduction == null)
            {
                return NotFound();
            }
            return View(deduction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Deduction deduction)
        {

            var deduct = await _MyDbContext.Deduction.Where(x => x.DeductionId == deduction.DeductionId).AsNoTracking().FirstOrDefaultAsync();
            deduct.DeductionDescription = deduction.DeductionDescription;

            _MyDbContext.Update(deduct);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var deduction = await _MyDbContext.Deduction.Where(p => p.DeductionId == ID).FirstOrDefaultAsync();
            _MyDbContext.Deduction.Remove(deduction);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
