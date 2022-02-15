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
    public class RespoCenterTrustFundController : Controller
    {
        private readonly RespoCenterTrustFundContext _context;
        private readonly MyDbContext _MyDbcontext;
        public RespoCenterTrustFundController(RespoCenterTrustFundContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }




        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_tr ust_fund", "");
            return View(await _context.RespoCenterTrustFund.ToListAsync());
        }

  

        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_trust_fund", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Respocentertrustfund,RespocentertrustfundCode")] RespoCenterTrustFund respotrustfundcenter)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_trust_fund", "");
            if (ModelState.IsValid)
            {
                _context.Add(respotrustfundcenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(respotrustfundcenter);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_trust_fund", "");
            if (id == null)
            {
                return NotFound();
            }

            var respotrustfundcenter = await _context.RespoCenterTrustFund.FindAsync(id);
            if (respotrustfundcenter == null)
            {
                return NotFound();
            }
            return View(respotrustfundcenter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Respocentertrustfund,RespocentertrustfundCode,RespocentertrustfundId")] RespoCenterTrustFund respotrustfundcenter)
        {
            ViewBag.filter = new FilterSidebar("trust_fund", "respo_trust_fund", "");
            if (id != respotrustfundcenter.RespocentertrustfundId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(respotrustfundcenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RespoCenterTrustFundExists(respotrustfundcenter.RespocentertrustfundId))
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
            return View(respotrustfundcenter);
        }

        private bool RespoCenterTrustFundExists(int id)
        {
            return _context.RespoCenterTrustFund.Any(e => e.RespocentertrustfundId == id);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var respotrustfundcenter = await _context.RespoCenterTrustFund.Where(p => p.RespocentertrustfundId == ID).FirstOrDefaultAsync();
            _context.RespoCenterTrustFund.Remove(respotrustfundcenter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}