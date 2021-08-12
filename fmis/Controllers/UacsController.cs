using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using System.Text.Json;
using System.IO;
using System.Text;

namespace fmis.Controllers
{
    public class UacsController : Controller
    {
       
        public class UacsData
        {
            public string Account_title { get; set; }
            public int Expense_code { get; set; }
        }

        private UacsContext Context { get; }

        public UacsController(UacsContext context)
        {
            this.Context = context;
        }

        // GET: Uacs
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(Context.Uacs.ToList());
            ViewBag.temp = json;
            return View();
        }

        [HttpPost]
        public IActionResult SaveUacs(List<UacsData> data) {
            var uacses = new List<Uacs>();
            var uacs = new Uacs();


            foreach(var item in data)
            { 
            uacs.Account_title = item.Account_title;
            uacs.Expense_code = item.Expense_code;
            uacses.Add(uacs);
            }
            this.Context.Uacs.Add(uacs);
            this.Context.SaveChanges();

            return Json(data);

        }

        // GET: Uacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await Context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // GET: Uacs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Uacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create([Bind("Id,Account_title,Expense_code,Created_at,Updated_at")] Uacs uacs)
        {
            if (ModelState.IsValid)
            {
                Context.Add(uacs);
                /*await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return "Successfuly Added";
        }

        // GET: Uacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await Context.Uacs.FindAsync(id);
            if (uacs == null)
            {
                return NotFound();
            }
            return View(uacs);
        }

        // POST: Uacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Account_title,Expense_code,Created_at,Updated_at,Date_recieved")] Uacs uacs)
        {
            if (id != uacs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(uacs);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UacsExists(uacs.Id))
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
            return View(uacs);
        }

        // GET: Uacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await Context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // POST: Uacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uacs = await Context.Uacs.FindAsync(id);
            Context.Uacs.Remove(uacs);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UacsExists(int id)
        {
            return Context.Uacs.Any(e => e.Id == id);
        }
    }
}
