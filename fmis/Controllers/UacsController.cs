using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Rotativa.AspNetCore;

namespace fmis.Controllers
{

    public class UacsController : Controller
    {
        private readonly UacsContext _context;

        public UacsController(UacsContext context)
        {
            _context = context;
        }

        public class UacsData
        {
            public string Account_title { get; set; }
            public string Expense_code { get; set; }
            public int Id { get; set; }
            public string token { get; set; }
        }

        public class ManyId
        {
            public int many_id { get; set; }
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public int single_id { get; set; }
            public string single_token { get; set; }
            public List<ManyId> many_id { get; set; }
        }


        // GET: Uacs
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Uacs.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            return View("~/Views/Uacs/Index.cshtml");
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await _context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // GET: Obligations/Create
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult AddData(List<string[]> dataListFromTable)
        {
            var dataListTable = dataListFromTable;
            return Json("Response, Data Received Successfully");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacs(List<UacsData> data)
        {
            var data_holder = this._context.Uacs;

            foreach (var item in data)
            {
                if (item.Id == 0) //save
                {
                    var uacs = new Uacs(); //clear object
                    uacs.Id = item.Id;
                    uacs.Account_title = item.Account_title;
                    uacs.Expense_code = item.Expense_code;
                    uacs.status = "activated";
                    uacs.token = item.token;

                    this._context.Uacs.Update(uacs);
                    this._context.SaveChanges();
                }
                else
                { //update
                    data_holder.Find(item.Id).Account_title = item.Account_title;
                    data_holder.Find(item.Id).Expense_code = item.Expense_code;
                    data_holder.Find(item.Id).status = "activated";

                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        // POST: Uacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Account_title,Expense_code")] Uacs uacs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uacs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uacs);
        }

        [HttpPost]

        public ActionResult AddUacs(IEnumerable<Uacs> UacsInput)

        {

            var p = UacsInput;
            return null;

        }

        // GET: Uacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacs = await _context.Uacs.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Account_title,Expense_code")] Uacs uacs)
        {
            if (id != uacs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uacs);
                    await _context.SaveChangesAsync();
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

            var uacs = await _context.Uacs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacs == null)
            {
                return NotFound();
            }

            return View(uacs);
        }

        // POST: Uacs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUacs(DeleteData data)
        {
            if (data.many_id.Count > 1)
            {
                var data_holder = this._context.Uacs;
                foreach (var many in data.many_id)
                {
                    data_holder.Find(many.many_id).status = "deactivated";
                    data_holder.Find(many.many_id).token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Uacs;
                data_holder.Find(data.single_id).status = "deactivated";
                data_holder.Find(data.single_id).token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        private bool UacsExists(int id)
        {
            return _context.Uacs.Any(e => e.Id == id);
        }
    }
}

