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
using System.Data;

namespace fmis.Controllers.Grid
{
    public partial class ObligationController : Controller
    {
        private readonly SectionContext _context;

        public ObligationController(SectionContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.layout = "_Layout";
            DataTable dt = new DataTable("Section");
            dt.Columns.AddRange(new DataColumn[8] { 
                       new DataColumn("Id", typeof(int)),
                       new DataColumn("Division",typeof(int)),
                       new DataColumn("Description", typeof(string)),
                       new DataColumn("Head", typeof(string)),
                       new DataColumn("Code", typeof(string)),
                       new DataColumn("Remember_Token", typeof(string)),
                        new DataColumn("Created_At",typeof(DateTime)),
                        new DataColumn("Updated_At",typeof(DateTime)),
            });
            int code = 10000;
            for (int i = 1; i < 10; i++)
            {
                dt.Rows.Add(code + 1, i + 0, "ALFKI", "Amals", "Sakalam", "Kuno", new DateTime(1991, 05, 15), new DateTime(1991, 05, 15));
              
            }
            ViewBag.datasource = dt;
            return View("~/Views/Obligation/Index.cshtml");
        }
    }
}

