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

namespace fmis.Controllers
{
    public class UtilizationController : Controller
    {
        private readonly UtilizationContext _context;

        public UtilizationController(UtilizationContext context)
        {
            _context = context;
        }

        // GET: Utilization
        public async Task<IActionResult> Index()
        {
            var json = JsonSerializer.Serialize(_context.Utilization.ToList());
            ViewBag.temp = json;
            return View(await _context.Utilization.ToListAsync());
        }

    }
}