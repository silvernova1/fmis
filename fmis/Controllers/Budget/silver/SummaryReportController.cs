using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Filters;
using fmis.Data;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using fmis.Models.John;

namespace fmis.Controllers.Budget.silver
{
    public class SummaryReportController : Controller
    {
        private readonly MyDbContext _context;

        public SummaryReportController(MyDbContext context)
        {
            _context = context;
        }

        // GET: AllotmentClasses
        public async Task<IActionResult> Index(int?id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass");

            var sumfunds = _context.FundSourceAmount.Sum(x => x.Amount);
            ViewBag.sumfunds = sumfunds;



            return View(await _context.Uacs
                .Include(i => i.FundsRealignments)



                .ToListAsync());

        }

        [HttpPost]
        public IActionResult ExportSummaryReports()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] {
                new DataColumn("Uacs"),
                new DataColumn("Account Title"),
                new DataColumn("Fund Source"),
                new DataColumn("Program"),
                new DataColumn("Allotment"),
                new DataColumn("Realignment"),
                new DataColumn("Obligations"),
                new DataColumn("Balance"),
                new DataColumn("Prexc"),
                new DataColumn("Code"),
            });

            /*var summary = from Summaryreport in _context.SummaryReport.Take(10)
                        select Summaryreport;

            foreach (var Summaryreport in summary)
            {
                dt.Rows.Add( Summaryreport.Uacs.Account_title, Summaryreport.Uacs.Expense_code);
            }*/

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Categories.xlsx");
                }
            }

        }
    }
}
