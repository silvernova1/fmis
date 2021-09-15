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

    public class UacsamountsController : Controller
    {
        private readonly UacsamountContext _context;

        public UacsamountsController(UacsamountContext context)
        {
            _context = context;
        }

        public class UacsamountData
        {
            public int ObligationId { get; set; }
            public string Account_title { get; set; }
            public string Expense_code { get; set; }
            public float Amount { get; set; }
            public float Total_disbursement { get; set; }
            public float Total_net_amount { get; set; }
            public float Total_tax_amount { get; set; }
            public float Total_others { get; set; }
            public int Id { get; set; }
        }

        // GET: Uacs
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Uacsamount.ToList());
            ViewBag.temp = json;
            return View("~/Views/Uacsamounts/Index.cshtml");
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacsamount = await _context.Uacsamount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacsamount == null)
            {
                return NotFound();
            }

            return View(uacsamount);
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
        public IActionResult SaveUacsamount(List<UacsamountData> data)
        {
            var uacsamount = new Uacsamount();

            var data_holder = this._context.Uacsamount;

            foreach (var item in data)
            {
                if (item.Id == 0)
                {
                    uacsamount.Id = item.Id;
                    uacsamount.ObligationId = item.ObligationId;
                    uacsamount.Account_title = item.Account_title;
                    uacsamount.Expense_code = item.Expense_code;
                    uacsamount.Amount = item.Amount;
                    uacsamount.Total_disbursement = item.Total_disbursement;
                    uacsamount.Total_net_amount = item.Total_net_amount;
                    uacsamount.Total_tax_amount = item.Total_tax_amount;
                    uacsamount.Total_others = item.Total_others;

                    this._context.Uacsamount.Update(uacsamount);
                    this._context.SaveChanges();
                }
                else
                {
                    data_holder.Find(item.Id).ObligationId = item.ObligationId;
                    data_holder.Find(item.Id).Account_title = item.Account_title;
                    data_holder.Find(item.Id).Expense_code = item.Expense_code;
                    data_holder.Find(item.Id).Amount = item.Amount;
                    data_holder.Find(item.Id).Total_disbursement = item.Total_disbursement;
                    data_holder.Find(item.Id).Total_net_amount = item.Total_net_amount;
                    data_holder.Find(item.Id).Total_tax_amount = item.Total_tax_amount;
                    data_holder.Find(item.Id).Total_others = item.Total_others;

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
        public async Task<IActionResult> Create([Bind("Id,Account_title,Expense_code,Amount,Total_disbursement,Total_net_amount,Total_tax_amount,Total_others")] Uacsamount uacsamount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uacsamount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uacsamount);
        }

        [HttpPost]

        public ActionResult AddUacsamount(IEnumerable<Uacs> UacsamountInput)

        {

            var p = UacsamountInput;
            return null;

        }

        // GET: Uacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacsamount = await _context.Uacsamount.FindAsync(id);
            if (uacsamount == null)
            {
                return NotFound();
            }
            return View(uacsamount);
        }

        // POST: Uacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Account_title,Expense_code,Amount,Total_disbursement,Total_net_amount,Total_tax_amount,Total_others")] Uacsamount uacsamount)
        {
            if (id != uacsamount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uacsamount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UacsamountExists(uacsamount.Id))
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
            return View(uacsamount);
        }

        // GET: Uacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uacsamount = await _context.Uacsamount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uacsamount == null)
            {
                return NotFound();
            }

            return View(uacsamount);
        }

        // POST: Uacs/Delete/5
        [HttpPost]
        public IActionResult DeleteUacsamount(int id)
        {
            var uacsamount = this._context.Uacsamount.Find(id);
            this._context.Uacsamount.Remove(uacsamount);
            this._context.SaveChangesAsync();
            return Json(id);
        }

        private bool UacsamountExists(int id)
        {
            return _context.Uacsamount.Any(e => e.Id == id);
        }
    }
}

