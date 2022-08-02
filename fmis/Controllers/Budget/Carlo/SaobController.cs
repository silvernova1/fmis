using ClosedXML.Excel;
using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using fmis.Models;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.Carlo
{
    [Authorize(Policy = "BudgetAdmin")]
    public class SaobController : Controller
    {
        private readonly MyDbContext _context;

        public SaobController(MyDbContext context)
        {
            _context = context;
 
        }
        public IActionResult Index()
        {
            ViewBag.Layout = "_Layout";
            ViewBag.filter = new FilterSidebar("budget_report", "saob" , "");
            return View("~/Views/Carlo/Saob/Index.cshtml");
        }

        [HttpPost]
        public IActionResult ExportSaob()
        {
         
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(
                new DataColumn[2]
            {
                new DataColumn("Date From"),
                new DataColumn("Date To"),

            });

       
           /* var saobs = from saob in _context.Saob.Take(10)
                        select saob;*/

          /*  foreach (var saob in saobs)
            {
                dt.Rows.Add(saob.datefrom, saob.dateto);
            }*/

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Saob.xlsx");
                }
            }
        }

    }
}
