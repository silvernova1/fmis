using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace fmis.Controllers.Budget.EnerZ
{
    public class SubAllotmentsController : Controller
    {
        private readonly Sub_allotmentContext _context;
        private readonly MyDbContext _MyDbcontext;

        public SubAllotmentsController(Sub_allotmentContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;

        }
        // GET: SubAllotmentsController
        public async Task<IActionResult> Index(int? id)
        {
            return View(await _context.Sub_allotment.ToListAsync());
        }

        // GET: SubAllotmentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SubAllotmentsController/Create
        public ActionResult Create(int? id)
        {
            PopulatePrexcsDropDownList();
            var sub_allotment = _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: SubAllotmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Suballotment_code,Suballotment_title,Responsibility_number,Description,Budget_allotmentBudgetAllotmentId,PId")] Sub_allotment sub_allotment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(sub_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(sub_allotment.PId);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(sub_allotment);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }

        // GET: SubAllotmentsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SubAllotmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubAllotmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SubAllotmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _MyDbcontext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.Id = new SelectList((from s in _MyDbcontext.Prexc.ToList()
                                         select new
                                         {
                                             Id = s.Id,
                                             prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                         }),
       "Id",
       "prexc",
       null);

        }
    }
}
