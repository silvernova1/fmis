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
    //[Authorize(Roles = "accounting_admin , accounting_user")]
    [Authorize(Roles = "accounting_admin, accounting_user")]
    public class CategoryController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public CategoryController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        [Route("Accounting/Category")]
        public async Task <IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "category", "");
            return View(await _MyDbContext.Category.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "category", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryDescription")] Category category)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "category", "");
            if (ModelState.IsValid)
            {

                _MyDbContext.Add(category);
                await Task.Delay(500);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "category", "");
            if (id == null)
            {
                return NotFound();
            }
            Category category = await _MyDbContext.Category.FirstOrDefaultAsync(x => x.CategoryId == id); ;

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {

            var categ = await _MyDbContext.Category.Where(x => x.CategoryId == category.CategoryId).AsNoTracking().FirstOrDefaultAsync();
            categ.CategoryDescription = category.CategoryDescription;

            _MyDbContext.Update(categ);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var category = await _MyDbContext.Category.Where(p => p.CategoryId == ID).FirstOrDefaultAsync();
            _MyDbContext.Category.Remove(category);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
