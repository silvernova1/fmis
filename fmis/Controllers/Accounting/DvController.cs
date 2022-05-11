using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Models.Accounting;
using fmis.Filters;
using Microsoft.AspNetCore.Identity;
using fmis.Areas.Identity.Data;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Rotativa.AspNetCore;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using System.Globalization;
using System.Collections;
using iTextSharp.tool.xml;
using Image = iTextSharp.text.Image;
using Grpc.Core;
using fmis.ViewModel;
using fmis.DataHealpers;



namespace fmis.Controllers.Accounting
{
    public class DvController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public DvController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        [Route("Accounting/Dv/Payee")]
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            ViewData["GetDvNo"] = searchString;

            var dv = from m in _MyDbContext.DV
                     select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                dv = dv.Where(s => s.DvNo!.Contains(searchString));
            }
            return View(await dv.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            PopulateFundClusterDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DvId,FundClusterId,Date,DvNo,Payee,Particulars,Amount")] Dv dv)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(dv);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dv);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            PopulateFundClusterDropDownList();
            if (id == null)
            {
                return NotFound();
            }

            var dv = await _MyDbContext.DV.FindAsync(id);
            if (dv == null)
            {
                return NotFound();
            }
            return View(dv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dv dv)
        {

            var dvs = await _MyDbContext.DV.Where(x => x.DvId == dv.DvId).AsNoTracking().FirstOrDefaultAsync();
            dvs.DvNo = dv.DvNo;

            PopulateFundClusterDropDownList();

            _MyDbContext.Update(dv);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var dvs = await _MyDbContext.DV.Where(p => p.DvId == ID).FirstOrDefaultAsync();
            _MyDbContext.DV.Remove(dvs);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.DV.ToList();
            return Json(branches.Where(x => x.DvId == id).ToList());
        }



        private void PopulateFundClusterDropDownList()
        {
            var departmentsQuery = from d in _MyDbContext.FundCluster
                                   orderby d.FundClusterDescription
                                   select d;
            ViewBag.FundClusterId = new SelectList((from s in _MyDbContext.FundCluster.ToList()
                                                    select new
                                                    {
                                                        FundCluster = s.FundClusterId,
                                                        FundClusterDescription = s.FundClusterDescription
                                                    }),
                                       "FundCluster",
                                       "FundClusterDescription",
                                       null);

        }



        }

    }




