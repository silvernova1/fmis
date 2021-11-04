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
using fmis.Models.silver;

namespace fmis.Controllers.Budget.silver
{
    public class ManageUsersController : Controller
    {
        private readonly ManageUsersContext _Context;
        private readonly PersonalInformationMysqlContext _pis_context;

        public ManageUsersController(ManageUsersContext Context, PersonalInformationMysqlContext pis_context)
        {
            _Context = Context;
            _pis_context = pis_context;
        }
        public IActionResult Index(int? id)
        {
            ViewBag.PsId = id;
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");

            var ManageUsers = _Context.ManageUsers.ToList();

            string concat_UserId = "(";
            int count = 0;
            foreach (var item in ManageUsers)
            {
                count++;

                if (count == 1)
                    concat_UserId += "'" + item.UserId + "'";
                else
                    concat_UserId += ",'" + item.UserId + "'";
            }

            concat_UserId += ")";

            if (ManageUsers.Count < 1)
                concat_UserId = "('')"; //default condition para sa mysql

            return View(_pis_context.forRequestingOffice(concat_UserId));
        }
        // DRI USA TA KUTOB HA



        // GET: Requesting_office/Details/5
        public IActionResult Details(string UserId)
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");
            if (UserId == "")
            {
                return NotFound();
            }
            PersonalInformationMysqlContext mysql_context = HttpContext.RequestServices.GetService(typeof(PersonalInformationMysqlContext)) as PersonalInformationMysqlContext;
            var ManageUsers = mysql_context.findPersonalInformation(UserId);

            if (ManageUsers == null)
            {
                return NotFound();
            }
            return View(ManageUsers);
        }

        // GET: Requesting_office/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");
            PopulatePsDropDownList();
            return View();
        }

        // POST: Requesting_office/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,username")] ManageUsers ManageUsers)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");
            try
            {
                if (ModelState.IsValid)
                {
                    _Context.Add(ManageUsers);
                    await _Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePsDropDownList();
            return View(ManageUsers);
        }

        // GET: manageusers/Edit/5
        public async Task<IActionResult> Edit(string UserId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");
            var ManageUsers = await _Context.ManageUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == UserId);

            ViewBag.UserId_existing = _pis_context.findPersonalInformation("'" + UserId + "'");

            PopulatePsDropDownList();

            if (ManageUsers == null)
            {
                return NotFound();
            }
            return View(ManageUsers);
        }
        // POST: Requesting_office/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,UserId")] ManageUsers ManageUsers)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");

            if (ModelState.IsValid)
            {
                try
                {
                    _Context.Update(ManageUsers);
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManageUsersExists(ManageUsers.Id))
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

            return View(ManageUsers);
        }

        // Personal Information Dropdown
        private void PopulatePsDropDownList()
        {
            ViewBag.UserId = new SelectList((from s in _pis_context.allPersonalInformation()
                                                where !_Context.ManageUsers.Any(ro => ro.UserId == s.userid)
                                                select new
                                                {
                                                    UserId = s.userid,
                                                    ps = s.full_name
                                                }),
                                          "UserId",
                                          "ps",
                                           null);

        }

        public async Task<IActionResult> Delete(string UserId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "ManageUsers");


            ViewBag.UserId_existing = _pis_context.findPersonalInformation("'" + UserId + "'");

            //return Json(ViewBag.pi_userid_existing.full_name);

            var ManageUsers = await _Context.ManageUsers
                .FirstOrDefaultAsync(m => m.UserId == UserId);

            PopulatePsDropDownList();

            if (ManageUsers == null)
            {
                return NotFound();
            }

            return View(ManageUsers);
        }

        // POST: Requesting_office/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ManageUsers = await _Context.ManageUsers.FindAsync(id);
            _Context.ManageUsers.Remove(ManageUsers);
            await _Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManageUsersExists(int id)
        {
            return _Context.ManageUsers.Any(e => e.Id == id);
        }


    }
}
