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

    public class UacsController : Controller
    {
        private readonly UacsContext _context;
        private readonly MyDbContext _MyDbContext;

        public UacsController(UacsContext context, MyDbContext MyDbContext)
        {
            _context = context;
            _MyDbContext = MyDbContext;
        }

        public class UacsData
        {
            public string Account_title { get; set; }
            public string Expense_code { get; set; }
            public string Description_first { get; set; }
            public string Description_second { get; set; }
            public string uacs_type { get; set; }
            public int UacsId { get; set; }
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

        public async Task <IActionResult> PS()
        {
            ViewBag.filter = new FilterSidebar("master_data", "uacs", "ps");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize( await _context.Uacs.Where(s => s.status == "activated" && s.uacs_type == "PS").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Carlo/Uacs/PS.cshtml");

        }

        public async Task <IActionResult> MOOE()
        {
            ViewBag.filter = new FilterSidebar("master_data", "uacs", "mooe");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _context.Uacs.Where(s => s.status == "activated" && s.uacs_type == "MOOE").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Carlo/Uacs/MOOE.cshtml");

        }

        public async Task <IActionResult> CO()
        {
            ViewBag.filter = new FilterSidebar("master_data", "uacs", "co");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _context.Uacs.Where(s => s.status == "activated" && s.uacs_type == "CO").ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Carlo/Uacs/CO.cshtml");

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
        public IActionResult SaveUacsPS(List<UacsData> data)
        {
            var data_holder = this._context.Uacs;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "PS";
             
                    this._context.SaveChanges();
                }
                else if(item.Account_title != null || item.Expense_code != null) //save
                {
                    var uacs = new Uacs(); //clear object
                    uacs.Account_title = item.Account_title;
                    uacs.Expense_code = item.Expense_code;
                    uacs.status = "activated";
                    uacs.uacs_type = "PS";
                    uacs.token = item.token;

                    this._context.Uacs.Update(uacs);
                    this._context.SaveChanges();
                }    
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacsMOOE(List<UacsData> data)
        {
            var data_holder = this._context.Uacs;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "MOOE";

                    this._context.SaveChanges();
                }
                else if (item.Account_title != null || item.Expense_code != null) //save
                {
                    var uacs = new Uacs(); //clear object
                    uacs.Account_title = item.Account_title;
                    uacs.Expense_code = item.Expense_code;
                    uacs.status = "activated";
                    uacs.uacs_type = "MOOE";
                    uacs.token = item.token;

                    this._context.Uacs.Update(uacs);
                    this._context.SaveChanges();
                }
            }

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUacsCO(List<UacsData> data)
        {
            var data_holder = this._context.Uacs;

            foreach (var item in data)
            {
                if (data_holder.Where(s => s.token == item.token).FirstOrDefault() != null) //update
                {
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Account_title = item.Account_title;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().Expense_code = item.Expense_code;
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().status = "activated";
                    data_holder.Where(s => s.token == item.token).FirstOrDefault().uacs_type = "CO";

                    this._context.SaveChanges();
                }
                else if (item.Account_title != null || item.Expense_code != null)  //save
                {
                    var uacs = new Uacs(); //clear object
                    uacs.Account_title = item.Account_title;
                    uacs.Expense_code = item.Expense_code;
                    uacs.status = "activated";
                    uacs.uacs_type = "CO";
                    uacs.token = item.token;

                    this._context.Uacs.Update(uacs);
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
        public async Task<IActionResult> Edit(Uacs uacs)
        {
            var uac = await _context.Uacs.Where(x => x.UacsId == uacs.UacsId).AsNoTracking().FirstOrDefaultAsync();
            uac.Account_title = uacs.Account_title;
            uac.Expense_code = uacs.Expense_code;
            uac.uacs_type = uacs.uacs_type;
            uac.status = uacs.status;

            _context.Update(uac); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /*{
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
        }*/

        // POST: Uacs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUacs(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                var data_holder = this._context.Uacs;
                foreach (var many in data.many_token)
                {
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().status = "deactivated";
                    data_holder.Where(s => s.token == many.many_token).FirstOrDefault().token = many.many_token;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var data_holder = this._context.Uacs;
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().status = "deactivated";
                data_holder.Where(s => s.token == data.single_token).FirstOrDefault().token = data.single_token;

                await _context.SaveChangesAsync();
            }

            return Json(data);
        }

       
    }
}

