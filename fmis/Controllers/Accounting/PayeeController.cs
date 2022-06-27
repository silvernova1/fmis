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

namespace fmis.Controllers.Accounting
{
    public class PayeeController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public PayeeController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            return View(await _MyDbContext.Payee.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayeeId,PayeeDescription")] Payee payee)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(payee);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payee);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "");
            if (id == null)
            {
                return NotFound();
            }

            var payee = await _MyDbContext.Payee.FindAsync(id);
            if (payee == null)
            {
                return NotFound();
            }
            return View(payee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payee payee)
        {

            var pay = await _MyDbContext.Payee.Where(x => x.PayeeId == payee.PayeeId).AsNoTracking().FirstOrDefaultAsync();
            pay.PayeeDescription = payee.PayeeDescription;

            _MyDbContext.Update(pay);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var payee = await _MyDbContext.Payee.Where(p => p.PayeeId == ID).FirstOrDefaultAsync();
            _MyDbContext.Payee.Remove(payee);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
