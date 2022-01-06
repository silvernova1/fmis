using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Filters;
using fmis.Data;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using System.Globalization;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using fmis.DataHealpers;
using fmis.Models.Carlo;
using fmis.Models.John;
using fmis.ViewModel;

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
        public async Task<IActionResult> Index(int? id)
        {

            var generate_reports = (from fundsource in _context.FundSources
                                   join prexc in _context.Prexc
                                   on fundsource.PrexcId equals prexc.Id
                                   join fundsource_amount in _context.FundSourceAmount
                                   on fundsource.FundSourceId equals fundsource_amount.FundSourceId
                                   join uacs in _context.Uacs
                                   on fundsource_amount.UacsId equals uacs.UacsId
                                   join budget_allotment in _context.Budget_allotments
                                    on fundsource.Budget_allotmentBudgetAllotmentId equals budget_allotment.BudgetAllotmentId



                                    select new SummaryReportViewModel
                                   {
                                       fund_source = fundsource,
                                       prexc = prexc,
                                       uacs = uacs,
                                       fundsource_amount = fundsource_amount,
                                       budget_allotment = budget_allotment
                                       

                                   }).ToList();

            ViewBag.filter = new FilterSidebar("master_data", "SummaryReports");

            var sumfunds = _context.FundSourceAmount.Where(s => s.BudgetId == id).Sum(x => x.Amount);
            ViewBag.sumfunds = sumfunds.ToString("C", new CultureInfo("en-PH"));

            var totalbudget = _context.FundSourceAmount.Sum(x => x.Amount);
            ViewBag.totalbudget = totalbudget.ToString("C", new CultureInfo("en-PH"));

            return View(generate_reports);

        }

        public ActionResult Filter(DateTime date_from, DateTime end)
        {
            using (MyDbContext db = new MyDbContext())
            {

                var filterTicker = db.SummaryReport
            .Where(x => x.datefrom >= date_from && x.dateto <= end).ToList();


                ViewBag.START = date_from;
                ViewBag.END = end;
                return View(filterTicker);
            }

        }

        [HttpPost]
        public IActionResult ExportSummaryReports()
        {

            /*DateTime date1 = Convert.ToDateTime(date_from);
            DateTime date2 = Convert.ToDateTime(date_to);*/

            DataTable dt = new DataTable("Summary Report");

            var customers = from customer in _context.SummaryReport.ToList()
                            select customer;

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                var currentRow = 3;
                Double total = 0.00;

                // ws.PageSetup.CenterHorizontally = 1;

                ws.Cell("C1").RichText.AddText("1");
                ws.Cell("D1").RichText.AddText("2");
                ws.Cell("E1").RichText.AddText("3");
                ws.Cell("F1").RichText.AddText("4");
                ws.Cell("G1").RichText.AddText("5");
                ws.Cell("H1").RichText.AddText("6");
                ws.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("D2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("E2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("F2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("G2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell("H2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("A2").Style.Font.SetBold();
                ws.Cell("A2").Style.Font.FontSize = 12;
<<<<<<< HEAD
                ws.Cell("A2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("A2").Style.Fill.BackgroundColor = XLColor.White;
=======
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Rows(11, 1).AdjustToContents();
>>>>>>> 3002b0fd69dd4b391b1f9256260b948a6081002a
                ws.Cell("A2").RichText.AddText("UACS");
                ws.Cell("A2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("B2").Style.Font.SetBold();
                ws.Cell("B2").Style.Font.FontSize = 12;
                ws.Cell("B2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("B2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("B2").RichText.AddText("FUND SOURCE");
                ws.Cell("B2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("C2").Style.Font.SetBold();
                ws.Cell("C2").Style.Font.FontSize = 12;
                ws.Cell("C2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("C2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("C2").RichText.AddText("PROGRAM");
                ws.Cell("C2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("D2").Style.Font.SetBold();
                ws.Cell("D2").Style.Font.FontSize = 12;
                ws.Cell("D2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("D2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("D2").RichText.AddText("ALLOTMENTS");
                ws.Cell("D2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("E2").Style.Font.SetBold();
                ws.Cell("E2").Style.Font.FontSize = 12;
                ws.Cell("E2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("E2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("E2").RichText.AddText("REALIGNMENT");
                ws.Cell("E2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("F2").Style.Font.SetBold();
                ws.Cell("F2").Style.Font.FontSize = 12;
                ws.Cell("F2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("F2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("F2").RichText.AddText("OBLIGATIONS");
                ws.Cell("F2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("G2").Style.Font.SetBold();
                ws.Cell("G2").Style.Font.FontSize = 12;
                ws.Cell("G2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("G2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("G2").RichText.AddText("BALANCE");
                ws.Cell("G2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("H2").Style.Font.SetBold();
                ws.Cell("H2").Style.Font.FontSize = 12;
                ws.Cell("H2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("H2").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell("H2").RichText.AddText("CODE");
                ws.Cell("H2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

<<<<<<< HEAD
=======

       

                /*var budget_allotments = _MyDbContext.Budget_allotments
                     .Include(budget_allotment => budget_allotment.FundSources)
                     .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                     .ThenInclude(uacs => uacs.Uacs);*/
>>>>>>> 3002b0fd69dd4b391b1f9256260b948a6081002a

                var budget_allotments = _context.Budget_allotments
                    .Include(budget_allotment => budget_allotment.FundSources)
                    .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                    .ThenInclude(uacs => uacs.Uacs)
                    .ThenInclude(uacs => uacs.FundSource);

                foreach (Budget_allotment budget_allotment in budget_allotments)
                {
                    

                   /* var fsh = _context.FundSources.Where(p => p.Budget_allotmentBudgetAllotmentId == budget_allotment.BudgetAllotmentId).ToString();*/

                    foreach (FundSource fundSource in budget_allotment.FundSources)
                    {
                      
                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts)
                        {

                            ws.Cell(currentRow, 1).Value = _context.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 2).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.FundSourceTitle;
                            ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

<<<<<<< HEAD
                            ws.Cell(currentRow, 3).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_title;
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 4).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.Beginning_balance;
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 8).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
=======
                            ws.Cell(currentRow, 2).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.FundSourceTitle;
                            ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 3).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_title;
                            ws.Cell(currentRow, 3).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 4).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.Beginning_balance;
                            ws.Cell(currentRow, 4).Style.Alignment.Indent = 4;

                            ws.Cell(currentRow, 8).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 8).Style.Alignment.Indent = 8;
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "00";

                            // Adjust column width
                            ws.Column(1).AdjustToContents();

                            // Adjust row heights
                            ws.Rows(3, 3).AdjustToContents();





>>>>>>> 3002b0fd69dd4b391b1f9256260b948a6081002a

                            currentRow++;
                            total = (double)fundsource_amount.Amount;

                        }

                    }
                  
                }
                ws.Columns("A", "H").AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Summary Report.xlsx");
                }
            }
        }

        public IActionResult DownloadSaob()
        {
            ViewBag.filter = new FilterSidebar("master_data", "SummaryReport");

            var reports = (from list in _context.Yearly_reference where list.YearlyReference == "2021" select list).ToList();
            return PartialView(reports);
        }
    }
}
