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
using fmis.Filters;

namespace fmis.Controllers
{
    public class PrexcController : Controller
    {
        private readonly PrexcContext _context;

        public PrexcController(PrexcContext context)
        {
            _context = context;
        }

        public class PrexcData
        {
            public int Id { get; set; }
            public string pap_title { get; set; }
            public string pap_code1 { get; set; }
            public string pap_type { get; set; }
            public string status { get; set; }
            public string token { get; set; }
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

        // GET: GAS
        public IActionResult GAS()
        {
            ViewBag.filter = new FilterSidebar("master_data", "prexc","gas");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Prexc.Where(s => s.status == "activated" && s.pap_type == "GAS").ToList());
            ViewBag.temp = json;
            return View("~/Views/Prexc/GAS.cshtml");
        }

        // GET: STO
        public IActionResult STO()
        {
            ViewBag.filter = new FilterSidebar("master_data", "prexc", "sto");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Prexc.Where(s => s.status == "activated" && s.pap_type == "STO").ToList());
            ViewBag.temp = json;
            return View("~/Views/Prexc/STO.cshtml");
        }

        // GET: OPERATION
        public IActionResult OPERATIONS()
        {
            ViewBag.filter = new FilterSidebar("master_data", "prexc", "operations");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(_context.Prexc.Where(s => s.status == "activated" && s.pap_type == "OPERATIONS").ToList());
            ViewBag.temp = json;
            return View("~/Views/Prexc/OPERATIONS.cshtml");
        }


        
        // GET: Prexc/Create
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
        public IActionResult SavePrexcGAS(List<PrexcData> data)
        {
            var data_holder = this._context.Prexc;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "GAS";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_code1 != null || item.pap_type != null) //save
                {
                    var prexc = new Prexc(); //clear object
                    prexc.Id = item.Id;
                    prexc.PapTypeID = 1;
                    prexc.pap_title = item.pap_title;
                    prexc.pap_code1 = item.pap_code1;
                    prexc.pap_type = "GAS";
                    prexc.status = "activated";
                    prexc.token = item.token;

                    this._context.Prexc.Update(prexc);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePrexcSTO(List<PrexcData> data)
        {
            var data_holder = this._context.Prexc;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "STO";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_code1 != null || item.pap_type != null) //save
                {
                    var prexc = new Prexc(); //clear object
                    prexc.Id = item.Id;
                    prexc.PapTypeID = 2;
                    prexc.pap_title = item.pap_title;
                    prexc.pap_code1 = item.pap_code1;
                    prexc.pap_type = "STO";
                    prexc.status = "activated";
                    prexc.token = item.token;
                    this._context.Prexc.Update(prexc);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePrexcOPERATIONS(List<PrexcData> data)
        {
            var data_holder = this._context.Prexc;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_title = item.pap_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_code1 = item.pap_code1;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().pap_type = "OPERATIONS";

                    this._context.SaveChanges();
                }
                else if (item.pap_title != null || item.pap_code1 != null || item.pap_type != null) //save
                {
                    var prexc = new Prexc(); //clear object
                    prexc.Id = item.Id;
                    prexc.PapTypeID = 3;
                    prexc.pap_title = item.pap_title;
                    prexc.pap_code1 = item.pap_code1;
                    prexc.pap_type = "OPERATIONS";
                    prexc.status = "activated";
                    prexc.token = item.token;

                    this._context.Prexc.Update(prexc);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        // POST: Prexc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,pap_title,pap_code,pap_code2")] Prexc prexc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prexc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prexc);
        }

        [HttpPost]

        public ActionResult AddPrexc(IEnumerable<Prexc> PrexcInput)

        {
            var p = PrexcInput;
            return null;
        }

        // GET: Prexc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc.FindAsync(id);
            if (prexc == null)
            {
                return NotFound();
            }
            return View(prexc);
        }

        // POST: Prexc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Prexc prexc)
        {
            var prex = await _context.Prexc.Where(x => x.Id == prexc.Id).AsNoTracking().FirstOrDefaultAsync();
            prex.pap_title = prexc.pap_title;
            prex.pap_code1 = prexc.pap_code1;
            prex.pap_type = prexc.pap_type;
            prex.status = prexc.status;

            _context.Update(prex); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
   

        // GET:  Prexc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prexc = await _context.Prexc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prexc == null)
            {
                return NotFound();
            }

            return View(prexc);
        }

        // POST: Prexc/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePrexc(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.Prexc;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Prexc;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }
        private bool PrexcExists(int id)
        {
            return _context.Prexc.Any(e => e.Id == id);
        }
    }
}