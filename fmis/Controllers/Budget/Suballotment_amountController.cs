﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using Syncfusion.EJ2.Grids;
using System.Configuration;
using System.Data.SqlClient;

namespace fmis.Controllers
{
    public partial class Suballotment_amountController : Controller
    {
        private readonly SectionContext _context;

        public Suballotment_amountController(SectionContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            return View(await _context.Section.ToListAsync());
        }
    }
}    
