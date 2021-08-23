﻿using System;
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

namespace fmis.Controllers
{
    public class UtilizationController : Controller
    {
        private readonly UtilizationContext _context;

        public UtilizationController(UtilizationContext context)
        {
            _context = context;
        }

        public class UtilizationData
        {

            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public string Dv { get; set; }
            public string Pr_no { get; set; }
            public string Po_no { get; set; }
            public string Payer { get; set; }
            public string Address { get; set; }
            public string Particulars { get; set; }
            public int Ors_no { get; set; }
            public string Fund_source { get; set; }
            public float Gross { get; set; }
            public int Created_by { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date_recieved { get; set; }
            [DataType(DataType.Date)]
            public DateTime Time_recieved { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date_released { get; set; }
            [DataType(DataType.Date)]
            public DateTime Time_released { get; set; }
        }

        // GET: Utilization
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Utilization.ToList());
            ViewBag.temp = json;
            return View(await _context.Utilization.ToListAsync());
        }

        // GET: Utilization/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilization == null)
            {
                return NotFound();
            }

            return View(utilization);
        }

        // GET: Utilization/Create
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
        public IActionResult saveUtilization(List<Utilization> data)
        {
            var utilizations = new List<Utilization>();
            var utilization = new Utilization();


            foreach (var item in data)
            {

                utilization.Date = item.Date;
                utilization.Dv = item.Dv;
                utilization.Pr_no = item.Pr_no;
                utilization.Po_no = item.Po_no;
                utilization.Payer = item.Payer;
                utilization.Address = item.Address;
                utilization.Particulars = item.Particulars;
                utilization.Ors_no= item.Ors_no;
                utilization.Fund_source = item.Fund_source;
                utilization.Gross = item.Gross;
                utilization.Created_by = item.Created_by;
                utilization.Date_recieved = item.Date_recieved;
                utilization.Date_released = item.Date_released;
                utilization.Time_recieved = item.Time_recieved;
                utilization.Time_released = item.Time_released;

                utilizations.Add(utilization);
            }


            this._context.Utilization.Add(utilization);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST:Ors_head/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payer,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Utilization utilization)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilization);
        }

        [HttpPost]

        public ActionResult AddUtilization(IEnumerable<Utilization> UtilizationInput)

        {

            var p = UtilizationInput;
            return null;

        }

        // GET: Utilzation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization.FindAsync(id);
            if (utilization == null)
            {
                return NotFound();
            }
            return View(utilization);
        }

        // POST: Utilization/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Dv,Pr_no,Po_no,Payer,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Utilization utilization)
        {
            if (id != utilization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizationExists(utilization.Id))
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
            return View(utilization);

        }

        // GET: UtilizationDelete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilization = await _context.Utilization
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilization == null)
            {
                return NotFound();
            }

            return View(utilization);
        }

        // POST: Utiization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilization = await _context.Utilization.FindAsync(id);
            _context.Utilization.Remove(utilization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizationExists(int id)
        {
            return _context.Utilization.Any(e => e.Id == id);
        }
    }
}