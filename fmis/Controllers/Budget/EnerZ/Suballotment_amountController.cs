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
using fmis.Filters;

namespace fmis.Controllers
{

    public class Suballotment_amountController : Controller
    {
        private readonly Suballotment_amountContext _context;

        public Suballotment_amountController(Suballotment_amountContext context)
        {
            _context = context;
        }

        public class Suballotment_amountData
        {
            public int Id { get; set; }
            public int Expenses { get; set; }
            public float Amount { get; set; }
            public int Fund_source { get; set; }
            public string status { get; set; }
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

        // GET: Suballotment_amount
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "Suballotment_amount");
            var json = JsonSerializer.Serialize(_context.Suballotment_amount.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            return View("~/Views/Suballotment_amount/Index.cshtml");
        }

        // GET: Suballotment_amount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suballotment_amount == null)
            {
                return NotFound();
            }

            return View(suballotment_amount);
        }

        // GET: Suballotment_amount/Create
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
        public IActionResult SaveSuballotment_amount(List<Suballotment_amountData> data)
        {
            var data_holder = this._context.Suballotment_amount;

            foreach (var item in data)
            {
                if (item.Id == 0) //save
                {
                    var suballotment_amount = new Suballotment_amount(); //clear object
                    suballotment_amount.Id = item.Id;
                    suballotment_amount.Expenses = item.Expenses;
                    suballotment_amount.Amount = item.Amount;
                    suballotment_amount.Fund_source = item.Fund_source;
                    suballotment_amount.status = "activated";
                    suballotment_amount.token = item.token;

                    this._context.Suballotment_amount.Update(suballotment_amount);
                    this._context.SaveChanges();
                }
                else
                { //update
                    data_holder.Find(item.Id).Expenses = item.Expenses;
                    data_holder.Find(item.Id).Amount = item.Amount;
                    data_holder.Find(item.Id).Fund_source = item.Fund_source;

                    data_holder.Find(item.Id).status = "activated";

                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        // POST: Suballotment_amount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suballotment_amount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suballotment_amount);
        }

        [HttpPost]

        public ActionResult AddSuballotment_amount(IEnumerable<Suballotment_amount> Suballotment_amountInput)

        {

            var p = Suballotment_amountInput;
            return null;

        }

        // GET: Suballotment_amount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount.FindAsync(id);
            if (suballotment_amount == null)
            {
                return NotFound();
            }
            return View(suballotment_amount);
        }

        // POST: Suballotment_amount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Expenses,Amount,Fund_source")] Suballotment_amount suballotment_amount)
        {
            if (id != suballotment_amount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suballotment_amount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Suballotment_amountExists(suballotment_amount.Id))
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
            return View(suballotment_amount);
        }

        // GET:  Suballotment_amount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suballotment_amount = await _context.Suballotment_amount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suballotment_amount == null)
            {
                return NotFound();
            }

            return View(suballotment_amount);
        }

        // POST: Suballotment_amount/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSuballotment_amount(DeleteData data)
        {
            if (data.many_id.Count > 1)
            {
                var data_holder = this._context.Suballotment_amount;
                foreach (var many in data.many_id)
                {
                    data_holder.Find(many.many_id).status = "deactivated";
                    data_holder.Find(many.many_id).token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Suballotment_amount;
                data_holder.Find(data.single_id).status = "deactivated";
                data_holder.Find(data.single_id).token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        private bool Suballotment_amountExists(int id)
        {
            return _context.Suballotment_amount.Any(e => e.Id == id);
        }
    }
}