using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Storage;
using fmis.Filters;

namespace fmis.Controllers
{
    public class Ors_headController : Controller
    {
        private readonly Ors_headContext _context;
        private readonly DesignationContext _DContext;
        private readonly Personal_InfoMysqlContext _pis_context;

        public Ors_headController(Ors_headContext context, DesignationContext DContext, Personal_InfoMysqlContext pis_context)
        {
            _context = context;
            _DContext = DContext;
            _pis_context = pis_context;
        }

        // GET: Ors_head
        public IActionResult Index(int? id)
        {
            ViewBag.PsId = id;
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");

            var ors_head = _context.Ors_head.ToList();

            string concat_userid = "(";
            int count = 0;
            foreach (var item in ors_head)
            {
                count++;

                if (count == 1)
                    concat_userid += "'" + item.pi_userid + "'";
                else
                    concat_userid += ",'" + item.pi_userid + "'";
            }

            concat_userid += ")";

            if (ors_head.Count < 1)
                concat_userid = "('')"; //default condition para sa mysql

            return View(_pis_context.forOrs_head(concat_userid));
        }

        public IActionResult Information()
        {
            var pi_userid_existing = _pis_context.findPersonalInformation("0454");

            return Json(pi_userid_existing);
        }

        // GET: Ors_head/Details/5
        public IActionResult Details(string pi_userid)
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");
            if (pi_userid == "")
            {
                return NotFound();
            }
            Personal_InfoMysqlContext mysql_context = HttpContext.RequestServices.GetService(typeof(Personal_InfoMysqlContext)) as Personal_InfoMysqlContext;
            var ors_head = mysql_context.findPersonalInformation(pi_userid);

            if (ors_head == null)
            {
                return NotFound();
            }
            return View(ors_head);
        }

        // GET: Ors_head/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");
            PopulatePsDropDownList();
            return View();
        }

        // POST: Ors_head/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,pi_userid")] Ors_head ors_head)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ors_head);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePsDropDownList();
            return View(ors_head);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }

        // GET: Ors_head/Edit/5
        public async Task<IActionResult> Edit(string pi_userid)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");
            var Ors_head = await _context.Ors_head
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.pi_userid == pi_userid);

            ViewBag.pi_userid_existing = _pis_context.findPersonalInformation("'" + pi_userid + "'");

            PopulatePsDropDownList();

            if (Ors_head== null)
            {
                return NotFound();
            }

            return View(Ors_head);
        }

        // POST: Ors_head/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,pi_userid")] Ors_head ors_head)
        {
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ors_head);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Ors_headExists(ors_head.Id))
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

            return View(ors_head);
        }

        // Personal Information Dropdown
        private void PopulatePsDropDownList()
        {
            ViewBag.pi_userid = new SelectList((from s in _pis_context.allPersonalInformation()
                                                where !_context.Ors_head.Any(ro => ro.pi_userid == s.userid)
                                                select new
                                                {
                                                    pi_userid = s.userid,
                                                    ps = s.full_name
                                                }),
                                          "pi_userid",
                                          "ps",
                                           null);

        }

        public async Task<IActionResult> Delete(string pi_userid)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ors_head");


            ViewBag.pi_userid_existing = _pis_context.findPersonalInformation("'" + pi_userid + "'");

            //return Json(ViewBag.pi_userid_existing.full_name);

            var ors_head = await _context.Ors_head
                .FirstOrDefaultAsync(m => m.pi_userid == pi_userid);

            PopulatePsDropDownList();

            if (ors_head == null)
            {
                return NotFound();
            }

            return View(ors_head);
        }

        // POST: Ors_head/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ors_head = await _context.Ors_head.FindAsync(id);
            _context.Ors_head.Remove(ors_head);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Ors_headExists(int id)
        {
            return _context.Ors_head.Any(e => e.Id == id);
        }

    }
}
