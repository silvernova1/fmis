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
    public class Ors_headController : Controller
    {
        private readonly Ors_headContext _context;

        public Ors_headController(Ors_headContext context)
        {
            _context = context;
        }

        public class Ors_headData
        {

            public string Head_name { get; set; }
            public string Position { get; set; }
        }

        // GET: Ors_head
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Ors_head.ToList());
            ViewBag.temp = json;
            return View(await _context.Ors_head.ToListAsync());
        }

        // GET: Ors_head/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ors_head = await _context.Ors_head
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ors_head == null)
            {
                return NotFound();
            }

            return View(ors_head);
        }

        // GET: Ors_head/Create
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
        public IActionResult saveOrs_head(List<Ors_headData> data)
        {
            var ors_heads = new List<Ors_head>();
            var ors_head = new Ors_head();


            foreach (var item in data)
            {

                ors_head.Head_name = item.Head_name;
                ors_head.Position = item.Position;

                ors_heads.Add(ors_head);
            }


            this._context.Ors_head.Add(ors_head);
            this._context.SaveChanges();
            return Json(data);
        }

        // POST:Ors_head/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Head_name,Position")] Ors_head ors_head)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ors_head);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ors_head);
        }

        [HttpPost]

        public ActionResult AddOrs_head(IEnumerable<Ors_head> Ors_headInput)

        {

            var p = Ors_headInput;
            return null;

        }

        // GET: Ors_head/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ors_head = await _context.Ors_head.FindAsync(id);
            if (ors_head == null)
            {
                return NotFound();
            }
            return View(ors_head);
        }

        // POST: Ors_head/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Head_name,Position")] Ors_head ors_head)
        {
            if (id != ors_head.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ors_head);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Ors_headExists(ors_head.Id))
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
            return View(ors_head);
        }

        // GET: Ors_head/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ors_head = await _context.Ors_head
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ors_head == null)
            {
                return NotFound();
            }

            return View(ors_head);
        }

        // POST: Ors_head/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ors_head = await _context.Ors_head.FindAsync(id);
            _context.Ors_head.Remove(ors_head);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Ors_headExists(int id)
        {
            return _context.Ors_head.Any(e => e.Id == id);
        }
    }
}