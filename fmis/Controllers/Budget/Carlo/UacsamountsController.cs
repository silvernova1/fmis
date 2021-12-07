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
using System.Drawing;
using Rotativa.AspNetCore;

namespace fmis.Controllers
{
    public class UacsamountsController : Controller
    {
        private readonly UacsamountContext _context;
        private readonly UacsContext _Ucontext;
      
        public UacsamountsController(UacsamountContext context, UacsContext ucontext)
        {
            _context = context;
            _Ucontext = ucontext;
                   
        }

        public class UacsamountData
        {
            public int ObligationId { get; set; }
            public int UacsId { get; set; }
            public string Expense_code { get; set; }
            public float Amount { get; set; }
            public float Total_disbursement { get; set; }
            public float Total_net_amount { get; set; }
            public float Total_tax_amount { get; set; }
            public float Total_others { get; set; }
            public int Id { get; set; }
            public string token { get; set; }
            public string status { get; set; }
        }

        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        // GET: Uacs
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Uacsamount.ToList());
            ViewBag.temp = json;
            return View("~/Views/Carlo/Uacsamounts/Index.cshtml");
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

            var data_holder = this._context.Uacsamount;

            foreach (var item in data)
            {
                
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    if (item.UacsId != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().UacsId = item.UacsId;

                    if (item.Expense_code != "")
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = Int64.Parse(item.Expense_code);

                    if (item.Amount != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Amount = item.Amount;

                    if (item.Total_disbursement != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Total_disbursement = item.Total_disbursement;

                    if (item.Total_net_amount != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Total_net_amount = item.Total_net_amount;

                    if (item.Total_tax_amount != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Total_tax_amount = item.Total_tax_amount;

                    if (item.Total_others != 0)
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().Total_others = item.Total_others;

                    if (item.status != "")
                        data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";

                    this._context.SaveChanges();
                }

                else
                {
                    var uacsamount = new Uacsamount();
                
                    if (item.Id != 0)
                        uacsamount.Id = item.Id;

                    if (item.ObligationId != 0)
                        uacsamount.ObligationId = item.ObligationId;

                    if (item.UacsId != 0)
                        uacsamount.UacsId = item.UacsId;

                    if (item.Expense_code != "")
                        uacsamount.Expense_code = Int64.Parse(item.Expense_code);

                    if (item.Amount != 0)
                        uacsamount.Amount = item.Amount;

                    if (item.Total_disbursement != 0)
                        uacsamount.Total_disbursement = item.Total_disbursement;

                    if (item.Total_net_amount != 0)
                        uacsamount.Total_net_amount = item.Total_net_amount;

                    if (item.Total_tax_amount != 0)
                        uacsamount.Total_tax_amount = item.Total_tax_amount;

                    if (item.Total_others != 0)
                        uacsamount.Total_others = item.Total_others;

                    if (item.status != "")
                        uacsamount.status = "activated";

                    if (item.token != "")
                        uacsamount.token = item.token;

                    this._context.Uacsamount.Update(uacsamount);
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
        public async Task<IActionResult> DeleteUacsamount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.Uacsamount;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Uacsamount;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        private bool UacsamountExists(int id)
        {
            return _context.Uacsamount.Any(e => e.Id == id);
        }
    }
}