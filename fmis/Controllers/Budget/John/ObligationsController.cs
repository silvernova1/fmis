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
using Microsoft.AspNetCore.Authorization;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Rotativa.AspNetCore;
using Syncfusion.Drawing;
using System.IO;
using Syncfusion.Pdf.Grid;
using RectangleF = Syncfusion.Drawing.RectangleF;
using SizeF = Syncfusion.Drawing.SizeF;
using Color = Syncfusion.Drawing.Color;
using PointF = Syncfusion.Drawing.PointF;
using Syncfusion.Pdf.Tables;
using fmis.Filters;

namespace fmis.Controllers
{

    public class ObligationsController : Controller
    {
        private readonly ObligationContext _context;
        private readonly UacsamountContext _Ucontext;
        private readonly UacsContext _UacsContext;

        public ObligationsController(ObligationContext context, UacsamountContext Ucontext, UacsContext UacsContext)
        {
            _context = context;
            _Ucontext = Ucontext;
            _UacsContext = UacsContext;
        }

        public IActionResult PrintPdf()
        {

            return new ViewAsPdf("PrintPdf")
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }


        public DateTime CheckExcelDate(string excel_data)
        {
            string dateString = @"d/M/yyyy";

            DateTime date1 = DateTime.ParseExact(dateString, @"d/M/yyyy",
            System.Globalization.CultureInfo.InvariantCulture);
            if (dateString == null)
                return DateTime.ParseExact(dateString, @"d/M/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            return (DateTime)date1;


        }

        public IActionResult CreateD()
        {

            return View("~/Views/Obligations/PrintPdf.cshtml");

        }


        public class ObligationData
        {
            public int Id { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public string Dv { get; set; }
            public string Pr_no { get; set; }
            public string Po_no { get; set; }
            public string Payee { get; set; }
            public string Address { get; set; }
            public string Particulars { get; set; }
            public int Ors_no { get; set; }
            public string Fund_source { get; set; }
            public float Gross { get; set; }
            public int Created_by { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date_recieved { get; set; }
            [DataType(DataType.Time)]
            public DateTime Time_recieved { get; set; }
            [DataType(DataType.Date)]
            public DateTime Date_released { get; set; }
            [DataType(DataType.Time)]
            public DateTime Time_released { get; set; }
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

        // GET: Obligations
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("ors", "obligation");
            var json = JsonSerializer.Serialize(_context.Obligation.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            return View("~/Views/Budget/John/Obligations/Index.cshtml");
        }

        // GET: Obligations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View(obligation);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObligationModal(int? id)
        {
            var json = JsonSerializer.Serialize(_Ucontext.Uacsamount.Where(s => s.ObligationId == id && s.status == "activated").ToList());
            ViewBag.temp = json;
            var uacs_data = JsonSerializer.Serialize(_UacsContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;
           
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View("~/Views/Budget/John/Obligations/ObligationModal.cshtml", obligation);
        }

        /* public IActionResult ObligationModal(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }
             var obligation =  _context.Obligation
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (obligation == null)
             {
                 return NotFound();
             }
             return View("~/Views/Budget/John/Obligations/ObligationModal.cshtml",obligation);
         }*/

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
        public IActionResult SaveObligation(List<ObligationData> data)
        {

            var data_holder = this._context.Obligation;

            foreach (var item in data)
            {
                if (item.Id == 0)//SAVE
                {

                    var obligation = new Obligation(); //CLEAR OBJECT
                    obligation.Id = item.Id;
                    obligation.Date = item.Date;
                    obligation.Dv = item.Dv;
                    obligation.Pr_no = item.Pr_no;
                    obligation.Po_no = item.Po_no;
                    obligation.Payee = item.Payee;
                    obligation.Address = item.Address;
                    obligation.Particulars = item.Particulars;
                    obligation.Ors_no = item.Ors_no;
                    obligation.Fund_source = item.Fund_source;
                    obligation.Gross = item.Gross;
                    obligation.Created_by = item.Created_by;
                    obligation.Date_recieved = item.Date_recieved;
                    obligation.Time_recieved = item.Time_recieved;
                    obligation.Date_released = item.Date_released;
                    obligation.Time_released = item.Time_released;
                    obligation.status = "activated";
                    obligation.token = item.token;

                    this._context.Obligation.Update(obligation);
                    this._context.SaveChanges();
                }
                else
                {
                    //UPDATE
                    data_holder.Find(item.Id).Date = item.Date;
                    data_holder.Find(item.Id).Dv = item.Dv;
                    data_holder.Find(item.Id).Pr_no = item.Pr_no;
                    data_holder.Find(item.Id).Payee = item.Payee;
                    data_holder.Find(item.Id).Address = item.Address;
                    data_holder.Find(item.Id).Particulars = item.Particulars;
                    data_holder.Find(item.Id).Ors_no = item.Ors_no;
                    data_holder.Find(item.Id).Fund_source = item.Fund_source;
                    data_holder.Find(item.Id).Gross = item.Gross;
                    data_holder.Find(item.Id).Created_by = item.Created_by;
                    data_holder.Find(item.Id).Date_recieved = item.Date_recieved;
                    data_holder.Find(item.Id).Time_recieved = item.Time_recieved;
                    data_holder.Find(item.Id).Date_released = item.Date_released;
                    data_holder.Find(item.Id).Time_released = item.Time_released;
                    data_holder.Find(item.Id).status = "activated";

                    this._context.SaveChanges();
                }
            }

            return Json(data);

        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obligation);
        }

        [HttpPost]

        public ActionResult AddObligation(IEnumerable<Obligation> ObligationsInput)

        {

            var p = ObligationsInput;
            return null;

        }



        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation.FindAsync(id);
            if (obligation == null)
            {
                return NotFound();
            }
            return View(obligation);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,Fund_source,Gross,Created_by,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
        {
            if (id != obligation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obligation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObligationExists(obligation.Id))
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
            return View(obligation);
        }

        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View(obligation);
        }

        // POST: Obligations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> DeleteConfirmed(DeleteData data)
        {
            if (data.many_id.Count > 1)
            {
                var data_holder = this._context.Obligation;
                foreach (var many in data.many_id)
                {
                    data_holder.Find(many.many_id).status = "deactivated";
                    data_holder.Find(many.many_id).token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Obligation;
                data_holder.Find(data.single_id).status = "deactivated";
                data_holder.Find(data.single_id).token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        private bool ObligationExists(int id)
        {
            return _context.Obligation.Any(e => e.Id == id);
        }
    }
}