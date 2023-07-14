using fmis.Data;
using fmis.Filters;
using fmis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers.Budget
{
    public class SectionsController : Controller
    {
        private readonly SectionsContext _context;
        private readonly MyDbContext _MyDbcontext;
        public SectionsController(SectionsContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }

        //[Route("ResponsibilityCenter")]
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "sections", "");
            //PopulateResposDropDownList();
            return View( _context.Sections.Include(x=>x.RespoCenter).OrderBy(x => x.SectionId).ToList());
        }
        // GET: Sections/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "sections", "");
            PopulateRespoDropDownList();
            return View();
        }
        private void PopulateRespoDropDownList()
        {
            ViewBag.RespoId = new SelectList((from s in _MyDbcontext.RespoCenter.ToList()
                                              select new
                                              {
                                                  RespoId = s.RespoId,
                                                  respo = s.Respo
                                              }),
                                     "RespoId",
                                     "respo",
                                     null);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sections sections)
        {
            ViewBag.filter = new FilterSidebar("master_data", "sections", "");
            if (ModelState.IsValid)
            {
                _context.Add(sections);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sections);
        }
        // GET: Sections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "sections", "");
            PopulateRespoDropDownList();
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Sections.FindAsync(id);
            if (sections == null)
            {
                return NotFound();
            }
            return View(sections);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sections sections)
        {
            ViewBag.filter = new FilterSidebar("master_data", "sections", "");
            if (id != sections.SectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sections);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionsExists(sections.SectionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sections);
        }
        private bool SectionsExists(int id)
        {
            return _context.Sections.Any(e => e.SectionId == id);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var sections = await _context.Sections.Where(p => p.SectionId == ID).FirstOrDefaultAsync();
            _context.Sections.Remove(sections);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
