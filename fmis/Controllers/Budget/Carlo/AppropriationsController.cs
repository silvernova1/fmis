using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Filters;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace fmis.Controllers
{
    public class AppropriationsController : Controller
    {
        private readonly AppropriationContext _context;
        public AppropriationsController(AppropriationContext context)
        {
            _context = context;
        }

        // GET: Appropriations
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            return View(await _context.Appropriation.ToListAsync());
        }

  
        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppropriationId,AppropriationSource")] Appropriation appropriation)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            if (ModelState.IsValid)
            {
                _context.Add(appropriation);
                appropriation.CreatedBy = Username;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appropriation);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "Appropriations", "");
            if (id == null)
            {
                return NotFound();
            }

            var appropriation = await _context.Appropriation.FindAsync(id);
            if (appropriation == null)
            {
                return NotFound();
            }
            return View(appropriation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Appropriation appropriation)
        {

            var appro = await _context.Appropriation.Where(x => x.AppropriationId == appropriation.AppropriationId).AsNoTracking().FirstOrDefaultAsync();
            appro.AppropriationSource = appropriation.AppropriationSource;
         

            _context.Update(appro);
            appro.UpdatedBy = Username;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

      
        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var appropriation = await _context.Appropriation.Where(p => p.AppropriationId == ID).FirstOrDefaultAsync();
            _context.Appropriation.Remove(appropriation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private bool AppropriationExists(int id)
        {
            return _context.Appropriation.Any(e => e.AppropriationId == id);
        }

        #region COOKIES
        public string Username => User.FindFirstValue(ClaimTypes.Name);
        #endregion
    }
}
