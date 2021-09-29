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

namespace fmis.Controllers.Budget.EnerZ
{
    public class Ors_headController : Controller
    {
        private readonly Ors_headContext _context;
        private readonly PersonalInformationContext _PContext;
        private readonly DesignationContext _DContext;
        public Ors_headController(Ors_headContext context, PersonalInformationContext PContext, DesignationContext DContext)
        {
            _context = context;
            _PContext = PContext;
            _DContext = DContext;
        }

        // GET:  Ors_head
        public async Task<IActionResult> Index(int? id)
        {
            ViewBag.PsId = id;
            ViewBag.layout = "_Layout";
            var ors_head = _context.Ors_head
            .Include(c => c.Personal_Information)
            .Include(d => d.Designation)
            .AsNoTracking();
            return View(await ors_head.ToListAsync());
            //return View(await _context.Requesting_office.ToListAsync());
        }

        // GET: Requesting_office/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }
            var ors_head = await _context.Ors_head
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ors_head == null)
            {
                return NotFound();
            }
            return View(ors_head);
        }
        // GET: Ors_head/Create
        public IActionResult Create(int? id)
        {
            ViewBag.layout = "_Layout";
            PopulatePsDropDownList();
            PopulateDsDropDownList();

            /*List<Personal_Information> oh = new List<Personal_Information>();

            oh = (from c in _Context.Personal_Information select c).ToList();
            oh.Insert(0, new Personal_Information { id = 0, fname = "--SelectA Ice cream--" });

            ViewBag.message = oh;
            ViewBag.layout = "_Layout";*/
            return View();
        }
        // POST: Ors_head/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Head_name,Position,Created_at,Updated_at,Pid,Did")] Ors_head ors_head)
        {
            /*if (Ors_head.Id == 0)
           {
               ModelState.AddModelError("", "Select ORS Head");
           }
           int SelectValue = Ors_head.Id;
           ViewBag.SelectedValue = Ors_head.Id;*/

            /*List<Prexc> p = new List<Prexc>();
            p = (from c in _pContext.Prexc select c).ToList();
            p.Insert(0, new Prexc { Id = 0, pap_title = "--Select PREXC--" });
            ViewBag.message = p;*/
            try
            {
                if (ModelState.IsValid)
                {
                    List<Prexc> p = new List<Prexc>();
                    _context.Add(ors_head);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePsDropDownList(ors_head.Pid);
            PopulateDsDropDownList(ors_head.Did);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(ors_head);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }
        // GET: Ors_head/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }
            var ors_head = await _context.Ors_head.FindAsync(id);
            if (ors_head == null)
            {
                return NotFound();
            }
            return View(ors_head);
        }
        // POST: Ors_head/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Head_name,Position,Created_at,Updated_at")] Ors_head ors_head)
        {
            ViewBag.layout = "_Layout";
            if (id != ors_head.Id)
            {
                return NotFound();
            }
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
        private void PopulatePsDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _PContext.Personal_Information
                             orderby d.userid
                             select d;
            /*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*/
            ViewBag.Pid = new SelectList((from s in _PContext.Personal_Information.ToList()
                                          select new
                                          {
                                              Pid = s.Pid,
                                              ps = s.fname + " " + s.mname + " " + s.lname
                                          }),
                                             "Pid",
                                             "ps",
                                             null);
        }
        private void PopulateDsDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _DContext.Designation
                             orderby d.Description
                             select d;
            /*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*/
            ViewBag.Did = new SelectList((from s in _DContext.Designation.ToList()
                                          select new
                                          {
                                              Did = s.Did,
                                              ds = s.Description
                                          }),
                                              "Did",
                                              "ds",
                                               null);
        }
        // GET: Ors_head/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.layout = "_Layout";
            if (id == null)
            {
                return NotFound();
            }
            var ors_head = await _context.Ors_head
                .FirstOrDefaultAsync(m => m.Id == id);
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
