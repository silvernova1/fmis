using System;
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
    public partial class Sub_allotmentController : Controller
    {
        private readonly Sub_allotmentContext _context;

        public Sub_allotmentController(Sub_allotmentContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            return View(await _context.Sub_allotment.ToListAsync());
        }
    }
}


