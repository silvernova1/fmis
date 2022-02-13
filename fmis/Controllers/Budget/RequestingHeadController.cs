using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Filters;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace fmis.Controllers.Budget
{
    public class RequestingHeadController : Controller
    {
        private readonly RequestingHeadContext _context;
        private readonly MyDbContext _MyDbcontext;
        public RequestingHeadController(RequestingHeadContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }




        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            return View(await _context.RequestingHead.ToListAsync());
        }



        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Headname,Division,Section,Headinformation")] RequestingHead requestinghead)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            if (ModelState.IsValid)
            {
                _context.Add(requestinghead);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestinghead);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            if (id == null)
            {
                return NotFound();
            }

            var requestinghead = await _context.RequestingHead.FindAsync(id);
            if (requestinghead == null)
            {
                return NotFound();
            }
            return View(requestinghead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Headname, Division, Section, Headinformation, HeadnameId")] RequestingHead requestinghead)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_trust_fund", "");
            if (id != requestinghead.HeadnameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestinghead);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestingHeadExists(requestinghead.HeadnameId))
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
            return View(requestinghead);
        }

        private bool RequestingHeadExists(int id)
        {
            return _context.RequestingHead.Any(e => e.HeadnameId == id);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var requestinghead = await _context.RequestingHead.Where(p => p.HeadnameId == ID).FirstOrDefaultAsync();
            _context.RequestingHead.Remove(requestinghead);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}