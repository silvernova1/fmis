using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Data.silver;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Storage;
using fmis.Filters;

namespace fmis.Controllers.Budget.silver
{
    public class Requesting_officeController : Controller
    {
        private readonly RequestingOfficeContext _context;
        private readonly DesignationContext _DContext;
        private readonly PersonalInformationMysqlContext _pis_context;

        public Requesting_officeController(RequestingOfficeContext context, DesignationContext DContext, PersonalInformationMysqlContext pis_context)
        {
            _context = context;
            _DContext = DContext;
            _pis_context = pis_context;
        }

        // GET: Requesting_office
        public IActionResult Index(int? id)
        {
            ViewBag.PsId = id;
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");

            var requesting_office = _context.Requesting_office.ToList();

            string concat_userid = "(";
            int count = 0;
            foreach (var item in requesting_office)
            {
                count++;

                if (count == 1)
                    concat_userid += "'" + item.pi_userid + "'";
                else
                    concat_userid += ",'" + item.pi_userid + "'";
            }

            concat_userid += ")";

            if (requesting_office.Count < 1)
                concat_userid = "('')"; //default condition para sa mysql

            return View(_pis_context.forRequestingOffice(concat_userid));
        }

        public IActionResult Information()
        {
            var pi_userid_existing = _pis_context.findPersonalInformation("0454");

            return Json(pi_userid_existing);
        }

        // GET: Requesting_office/Details/5
        public IActionResult Details(string pi_userid)
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");
            if (pi_userid == "")
            {
                return NotFound();
            }
            PersonalInformationMysqlContext mysql_context = HttpContext.RequestServices.GetService(typeof(PersonalInformationMysqlContext)) as PersonalInformationMysqlContext;
            var requesting_office = mysql_context.findPersonalInformation(pi_userid);

            if (requesting_office == null)
            {
                return NotFound();
            }
            return View(requesting_office);
        }

        // GET: Requesting_office/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");
            PopulatePsDropDownList();
            return View();
        }

        // POST: Requesting_office/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,pi_userid")] Requesting_office requesting_office)
        {
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(requesting_office);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePsDropDownList();
            return View(requesting_office);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }

        // GET: Requesting_office/Edit/5
        public async Task<IActionResult> Edit(string pi_userid)
        {
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");
            var Requesting_office = await _context.Requesting_office
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.pi_userid == pi_userid);

            ViewBag.pi_userid_existing = _pis_context.findPersonalInformation("'" + pi_userid + "'");

            PopulatePsDropDownList();

            if (Requesting_office == null)
            {
                return NotFound();
            }

            return View(Requesting_office);
        }

        // POST: Requesting_office/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,pi_userid")] Requesting_office requesting_office)
        {
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requesting_office);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Requesting_officeExists(requesting_office.Id))
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

            return View(requesting_office);
        }

        // Personal Information Dropdown
        private void PopulatePsDropDownList()
        {
            ViewBag.pi_userid = new SelectList((from s in _pis_context.allPersonalInformation()
                                                where !_context.Requesting_office.Any(ro => ro.pi_userid == s.userid)
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
            ViewBag.filter = new FilterSidebar("master_data", "requestingoffice");
    

            ViewBag.pi_userid_existing = _pis_context.findPersonalInformation("'" + pi_userid + "'");

            //return Json(ViewBag.pi_userid_existing.full_name);

            var requesting_office = await _context.Requesting_office
                .FirstOrDefaultAsync(m => m.pi_userid == pi_userid);

            PopulatePsDropDownList();

            if (requesting_office == null)
            {
                return NotFound();
            }

            return View(requesting_office);
        }

        // POST: Requesting_office/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requesting_office = await _context.Requesting_office.FindAsync(id);
            _context.Requesting_office.Remove(requesting_office);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Requesting_officeExists(int id)
        {
            return _context.Requesting_office.Any(e => e.Id == id);
        }

    }
}
