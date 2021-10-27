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

    public class UtilizationController : Controller
    {
        private readonly UtilizationContext _context;

        public UtilizationController(UtilizationContext context)
        {
            _context = context;
        }

        public class UtilizationData
        {
            public int Id { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime Date_recieved { get; set; }

            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
            public DateTime Time_recieved { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime Date_released { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime Time_released { get; set; }

            public string token { get; set; }
            public string status { get; set; }

        }

        public class Many
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<Many> many_token { get; set; }
        }

        // GET: Utilization
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("ors", "utilization");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Utilization.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            return View("~/Views/Utilization/Index.cshtml");
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
        [ValidateAntiForgeryToken]
        public IActionResult SaveUtilization(List<UtilizationData> data)
        {

            var data_holder = this._context.Utilization;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {

                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Date = item.Date;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Dv = item.Dv;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Pr_no = item.Pr_no;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Po_no = item.Po_no;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Payer = item.Payer;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Address = item.Address;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Particulars = item.Particulars;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Ors_no = item.Ors_no;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Fund_source = item.Fund_source;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Gross = item.Gross;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Created_by = item.Created_by;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Date_recieved = item.Date_recieved;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Time_recieved = item.Time_recieved;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Date_released = item.Date_released;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Time_released = item.Time_released;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";

                    this._context.SaveChanges();


                }
                else if ((item.Date.ToString() != null || item.Dv != null) && (item.Pr_no != null || item.Po_no != null) && (item.Payer != null ||
                         item.Address != null) && (item.Particulars != null || item.Ors_no.ToString() != null) && (item.Fund_source != null ||
                         item.Gross.ToString() != null) && (item.Created_by.ToString() != null || item.Date_recieved.ToString() != null) &&
                         (item.Time_recieved.ToString() != null || item.Date_released.ToString() != null) && (item.Time_released.ToString() != null)) //save
                {

                    var utilization = new Utilization(); //CLEAR OBJECT
                    utilization.Id = item.Id;
                    utilization.Date = item.Date;
                    utilization.Dv = item.Dv;
                    utilization.Pr_no = item.Pr_no;
                    utilization.Po_no = item.Po_no;
                    utilization.Payer = item.Payer;
                    utilization.Address = item.Address;
                    utilization.Particulars = item.Particulars;
                    utilization.Ors_no = item.Ors_no;
                    utilization.Fund_source = item.Fund_source;
                    utilization.Gross = item.Gross;
                    utilization.Created_by = item.Created_by;
                    utilization.Date_recieved = item.Date_recieved;
                    utilization.Time_recieved = item.Time_recieved;
                    utilization.Date_released = item.Date_released;
                    utilization.Time_released = item.Time_released;
                    utilization.status = "activated";
                    utilization.token = item.token;

                    this._context.Utilization.Update(utilization);
                    this._context.SaveChanges();
                }
            }

            return Json(data);

        }

        // POST: Utilization/Create
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

        // GET: Utilization/Edit/5
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

        // GET: Utilization/Delete/5
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

        // POST: Utilization/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUtilization(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.Utilization;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Utilization;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

        private bool UtilizationExists(int id)
        {
            return _context.Utilization.Any(e => e.Id == id);
        }
    }
}