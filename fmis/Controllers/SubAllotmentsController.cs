﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using fmis.Filters;

namespace fmis.Controllers.Budget
{
    public class SubAllotmentsController : Controller
    {
        private readonly Sub_allotmentContext _context;
        private readonly UacsContext _uContext;
        private readonly Budget_allotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly MyDbContext _MyDbContext;


        public SubAllotmentsController(Sub_allotmentContext context, UacsContext uContext, Budget_allotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext)
        {
            _context = context;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
        }


        public class Sub_allotmentamountData
        {
            public int Id { get; set; }
            public int Prexe_code { get; set; }
            public string Suballotment_code { get; set; }
            public string Suballotment_title { get; set; }
            public int Ors_head { get; set; }
            public string Responsibility_number { get; set; }
            public string Description { get; set; }
        }


        // GET: Sub_allotment
        public async Task<IActionResult> Index(int? id)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            return View(await _context.Sub_allotment.ToListAsync());



        }

        // GET: Sub_allotment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            /*PopulateHeadDropDownList();

            List<Ors_head> oh = new List<Ors_head>();

            oh = (from c in _orssContext.Ors_head select c).ToList();
            oh.Insert(0, new Ors_head { Id = 0, Head_name = "--Select ORS Head--" });*/

            //ViewBag.message = oh;
            ViewBag.BudgetId = id;
            if (id == null)
            {
                return NotFound();
            }
            var sub_allotment = await _MyDbContext.Budget_allotments
                .Include(s => s.Sub_allotments)
                .Include(s => s.Personal_Information)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BudgetAllotmentId == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // GET: Sub_allotmentController/Create
        public ActionResult Create(int? id)
        {
            ViewBag.BudgetId = id;
            ViewBag.FundsId = id;   
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            PopulatePrexcsDropDownList();
            var sub_allotment = _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: SubAllotmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Suballotment_code,Suballotment_title,Responsibility_number,Description,Budget_allotmentBudgetAllotmentId,PId")] Sub_allotment sub_allotment)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(sub_allotment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulatePrexcsDropDownList(sub_allotment.PId);
            //return View(await _context.FundSource.Include(c => c.Budget_allotment).Where());

            return View(sub_allotment);
            /*return View("~/Views/Budget_allotments/Index.cshtml");*/
        }
        // GET: Sub_allotment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment.FindAsync(id);
            if (sub_allotment == null)
            {
                return NotFound();
            }
            PopulatePrexcsDropDownList(sub_allotment.Id);
            return View(sub_allotment);
        }

        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.PId = new SelectList((from s in _pContext.Prexc.ToList()
                                          select new
                                          {
                                              PId = s.Id,
                                              prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                          }),
       "PId",
       "prexc",
       null);

        }


        /*new SelectList(departmentsQuery.AsNoTracking(), "Id", "pap_title", selectedDepartment);
}*/

        /*private void PopulatePrexcsDropDownList(object selectedPrexc = null)
        {
            var prexsQuery = from d in _pContext.Prexc
                             orderby d.pap_title
                             select d;

            *//*ViewBag.Id = new SelectList(prexsQuery, "Id", "pap_title", selectedPrexc);*//*

            ViewBag.Id = new SelectList((from s in _pContext.Prexc.ToList()
                                         select new
                                         {
                                             Id = s.Id,
                                             prexc = s.pap_title + " ( " + s.pap_code1 + ")"
                                         }),
       "Id",
       "prexc",
       null);

        }*/

        // POST: Sub_allotment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sub_allotment sub_allotment)
        {

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sub_allotment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Sub_allotmentExists(sub_allotment.Id))
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
            return View(sub_allotment);
        }


        // GET: Sub_allotment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            if (id == null)
            {
                return NotFound();
            }

            var sub_allotment = await _context.Sub_allotment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sub_allotment == null)
            {
                return NotFound();
            }

            return View(sub_allotment);
        }

        // POST: Sub_allotment/Delete/5
        [HttpPost]
        public IActionResult DeleteSuballotment_amount(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            var fundsourceamount = this._MyDbContext.FundSourceAmount.Find(id);
            this._MyDbContext.FundSourceAmount.Remove(fundsourceamount);
            this._MyDbContext.SaveChangesAsync();
            return Json(id);
        }

        private bool Sub_allotmentExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");
            return _context.Sub_allotment.Any(e => e.Id == id);
        }


        /* private static PdfPCell PhraseCell(Phrase phrase, int align)
         {
             PdfPCell cell = new PdfPCell(phrase);
             cell.BorderColor = BaseColor.BLACK;
             cell.VerticalAlignment = Element.ALIGN_TOP;
             cell.HorizontalAlignment = align;
             cell.PaddingBottom = 2f;
             cell.PaddingTop = 0f;
             return cell;
         }*/


    }
}