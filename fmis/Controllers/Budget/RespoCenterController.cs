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
    public class RespoCenterController : Controller
    {
        private readonly RespoCenterContext _context;
        private readonly MyDbContext _MyDbcontext;
        public RespoCenterController(RespoCenterContext context, MyDbContext MyDbcontext)
        {
            _context = context;
            _MyDbcontext = MyDbcontext;
        }

        [Route("ResponsibilityCenter")]
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "respo", "");
            PopulateResposDropDownList();
            return View(await _context.RespoCenter.OrderBy(x=>x.RespoCode).ToListAsync());
        }

        private void PopulateResposDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _MyDbcontext.Personal_Information
                                   orderby d.full_name
                                   select d;
            ViewBag.Pi = new SelectList((from s in _MyDbcontext.Personal_Information.ToList()
                                         select new
                                         {
                                             Pid = s.Pid,
                                             full_name = s.full_name
                                         }),
                                       "Pid",
                                       "full_name",
                                       null);


        }


        // GET: Appropriations/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("master_data", "respo", "");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Respo,RespoCode")] RespoCenter respocenter)
        {
            ViewBag.filter = new FilterSidebar("master_data", "respo", "");
            if (ModelState.IsValid)
            {
                _context.Add(respocenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(respocenter);
        }

        // GET: Appropriations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "respo", "");
            if (id == null)
            {
                return NotFound();
            }

            var respocenter = await _context.RespoCenter.FindAsync(id);
            if (respocenter == null)
            {
                return NotFound();
            }
            return View(respocenter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Respo,RespoCode")] RespoCenter respocenter)
        {
            ViewBag.filter = new FilterSidebar("master_data", "respo", "");
            if (id != respocenter.RespoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(respocenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RespoCenterExists(respocenter.RespoId))
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
            return View(respocenter);
        }

        private bool RespoCenterExists(int id)
        {
            return _context.RespoCenter.Any(e => e.RespoId == id);
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var respocenter = await _context.RespoCenter.Where(p => p.RespoId == ID).FirstOrDefaultAsync();
            _context.RespoCenter.Remove(respocenter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}