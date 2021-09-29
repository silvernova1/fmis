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
            public string pap_code2 { get; set; }
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

        // GET: Prexc
        public IActionResult Index()
        {
            var json = JsonSerializer.Serialize(_context.Prexc.Where(s => s.status == "activated").ToList());
            ViewBag.temp = json;
            return View("~/Views/Prexc/Index.cshtml");
        }

        // GET: Prexc/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public IActionResult SavePrexc(List<PrexcData> data)
        {
            var data_holder = this._context.Prexc;

            foreach (var item in data)
            {
                if (item.Id == 0) //save
                {
                    var prexc = new Prexc(); //clear object
                    prexc.Id = item.Id;
                    prexc.pap_title = item.pap_title;
                    prexc.pap_code1 = item.pap_code1;
                    prexc.pap_code2 = item.pap_code2;
                    prexc.status = "activated";
                    prexc.token = item.token;

                    this._context.Prexc.Update(prexc);
                    this._context.SaveChanges();
                }
                else
                { //update
                    data_holder.Find(item.Id).pap_title = item.pap_title;
                    data_holder.Find(item.Id).pap_code1 = item.pap_code1;
                    data_holder.Find(item.Id).pap_code2 = item.pap_code2;

                    data_holder.Find(item.Id).status = "activated";

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

        public ActionResult AddUacs(IEnumerable<Prexc> PrexcInput)

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,pap_title,pap_code,pap_code2")] Prexc prexc)
        {
            if (id != prexc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prexc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrexcExists(prexc.Id))
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
            return View(prexc);
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
            if (data.many_id.Count > 1)
            {
                var data_holder = this._context.Prexc;
                foreach (var many in data.many_id)
                {
                    data_holder.Find(many.many_id).status = "deactivated";
                    data_holder.Find(many.many_id).token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Prexc;
                data_holder.Find(data.single_id).status = "deactivated";
                data_holder.Find(data.single_id).token = data.single_token;

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