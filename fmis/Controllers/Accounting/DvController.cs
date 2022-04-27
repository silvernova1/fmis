using fmis.Data;
using fmis.Filters;
using fmis.Models.Accounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace fmis.Controllers.Accounting
{
    public class DvController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public DvController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var dv = from m in _MyDbContext.DV
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                dv = dv.Where(s => s.DvDescription!.Contains(searchString));
            }


            return View(await _MyDbContext.DV.ToListAsync());

        }





        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DvId,DvDescription,Payee")] Dv dv)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(dv);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dv);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            if (id == null)
            {
                return NotFound();
            }

            var dvs = await _MyDbContext.DV.FindAsync(id);
            if (dvs == null)
            {
                return NotFound();
            }
            return View(dvs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dv dv)
        {

            var div = await _MyDbContext.DV.Where(x => x.DvId == dv.DvId).AsNoTracking().FirstOrDefaultAsync();
            div.DvDescription = div.DvDescription;

            _MyDbContext.Update(div);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var dvs = await _MyDbContext.DV.Where(p => p.DvId == ID).FirstOrDefaultAsync();
            _MyDbContext.DV.Remove(dvs);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.DV.ToList();
            return Json(branches.Where(x => x.DvId == id).ToList());
        }

    }
}
