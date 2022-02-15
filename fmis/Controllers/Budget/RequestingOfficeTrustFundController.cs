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
    public class RequestingOfficeTrustFundController : Controller
    {
        private readonly RequestingOfficeTrustFundContext _context;
        private readonly MyDbContext _MyDbcontext;
        public RequestingOfficeTrustFundController(RequestingOfficeTrustFundContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }




        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            return View(await _context.RequestingOfficeTrustFund.ToListAsync());
        }



        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "requestinghead_fund", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Headname,Division,Section,Headinformation")] RequestingOfficeTrustFund requestinghead)
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

            var requestinghead = await _context.RequestingOfficeTrustFund.FindAsync(id);
            if (requestinghead == null)
            {
                return NotFound();
            }
            return View(requestinghead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Headname, Division, Section, Headinformation, HeadnameId")] RequestingOfficeTrustFund requestinghead)
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
                    if (!RequestingOfficeTrustFundExists(requestinghead.HeadnameId))
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

        private bool RequestingOfficeTrustFundExists(int id)
        {
            return _context.RequestingOfficeTrustFund.Any(e => e.HeadnameId == id);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var requestinghead = await _context.RequestingOfficeTrustFund.Where(p => p.HeadnameId == ID).FirstOrDefaultAsync();
            _context.RequestingOfficeTrustFund.Remove(requestinghead);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}