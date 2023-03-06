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
using System;
using Microsoft.AspNet.SignalR;

namespace fmis.Controllers.Accounting
{
/*    [Authorize(Policy = "AccountingAdmin")]*/

    public class AssigneeController : Controller
    {

        private readonly MyDbContext _MyDbContext;
       

        public AssigneeController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
         
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "assignee", "");
            return View(await _MyDbContext.Assignee.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetAssignee()
        {
            var res = _MyDbContext.Assignee.ToList();
            return Ok(res);
        }


        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "assignee", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssigneeId,FullName,Description,Designation")] Assignee assignee)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "assignee", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(assignee);
                await Task.Delay(500);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assignee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "assignee", "");
            if (id == null)
            {
                return NotFound();
            }

            var assignee = await _MyDbContext.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return NotFound();
            }
            return View(assignee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Assignee assignee)
        {

            var assign = await _MyDbContext.Assignee.Where(x => x.AssigneeId == assignee.AssigneeId).AsNoTracking().FirstOrDefaultAsync();
            assign.FullName = assignee.FullName;
            assign.Designation = assignee.Designation;

            _MyDbContext.Update(assign);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var assignee = await _MyDbContext.Assignee.Where(p => p.AssigneeId == ID).FirstOrDefaultAsync();
            _MyDbContext.Assignee.Remove(assignee);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
