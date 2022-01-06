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


        [HttpPost]
        public IActionResult ExportSummaryReports()
        {

            DataTable dt = new DataTable("Summary Report");
            /* dt.Columns.AddRange(new DataColumn[10]
                                           { new DataColumn("UACS"),
                                             new DataColumn("FUND SOURCE"),
                                             new DataColumn("PREXC"),
                                             new DataColumn("ALLOTMENTS"),
                                             new DataColumn("REALIGNMENT"),
                                             new DataColumn("OBLIGATIONS"),
                                             new DataColumn("BALANCE"),
                                             new DataColumn("ORS HEAD"),
                                             new DataColumn("ALLOTMENT CLASS"),
                                             new DataColumn("APPROPRIATION TYPE")});
             // dt.Columns.Add("000055");*/

            var customers = from customer in _context.SummaryReport.ToList()
                            select customer;

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                var currentRow = 3;
                Double total = 0.00;
                Double allotment_total = 0;


                // ws.PageSetup.CenterHorizontally = 1;

                ws.Cell("C1").RichText.AddText("1");
                ws.Cell("D1").RichText.AddText("2");
                ws.Cell("E1").RichText.AddText("3");
                ws.Cell("F1").RichText.AddText("4");
                ws.Cell("G1").RichText.AddText("5");
                ws.Cell("H1").RichText.AddText("6");
                ws.Cell("Q1").RichText.AddText("7");

                ws.Cell("A2").Style.Font.SetBold();
                ws.Cell("A2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Rows(11, 1).AdjustToContents();
                ws.Cell("A2").RichText.AddText("UACS");


                ws.Cell("B2").Style.Font.SetBold();
                ws.Cell("B2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("B2").RichText.AddText("FUND SOURCE");

                ws.Cell("C2").Style.Font.SetBold();
                ws.Cell("C2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("C2").RichText.AddText("PROGRAM");

                ws.Cell("D2").Style.Font.SetBold();
                ws.Cell("D2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("D2").RichText.AddText("ALLOTMENTS");

                ws.Cell("E2").Style.Font.SetBold();
                ws.Cell("E2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("E2").RichText.AddText("REALIGNMENT");

                ws.Cell("F2").Style.Font.SetBold();
                ws.Cell("F2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("F2").RichText.AddText("OBLIGATIONS");

                ws.Cell("G2").Style.Font.SetBold();
                ws.Cell("G2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("G2").RichText.AddText("BALANCE");

                ws.Cell("H2").Style.Font.SetBold();
                ws.Cell("H2").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("H2").RichText.AddText("CODE");


       

                /*var budget_allotments = _MyDbContext.Budget_allotments
                     .Include(budget_allotment => budget_allotment.FundSources)
                     .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                     .ThenInclude(uacs => uacs.Uacs);*/

                /*var summary = _context.FundSources
                    .Include(Uacs => Uacs.Uacs)
                    .ThenInclude(FundSource => FundSource.FundSource)
                    */
                var budget_allotments = _context.Budget_allotments
                    .Include(budget_allotment => budget_allotment.FundSources)
                    .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                    .ThenInclude(uacs => uacs.Uacs);

                /*from fundsource in _context.FundSources
                join prexc in _context.Prexc
                on fundsource.PrexcId equals prexc.Id
                join fundsource_amount in _context.FundSourceAmount
                on fundsource.FundSourceId equals fundsource_amount.FundSourceId
                join uacs in _context.Uacs
                on fundsource_amount.UacsId equals uacs.UacsId;*/


                foreach (Budget_allotment budget_allotment in budget_allotments)
                {
                    

                   /* var fsh = _context.FundSources.Where(p => p.Budget_allotmentBudgetAllotmentId == budget_allotment.BudgetAllotmentId).ToString();*/

                    foreach (FundSource fundSource in budget_allotment.FundSources)
                    {
                      
                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts)
                        {
                           
                            ws.Cell(currentRow, 1).Value = _context.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

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






                            currentRow++;
                            total = (double)fundsource_amount.Amount;

                        }

                    }
                  
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Summary Report.xlsx");
                }
            }
        }

        public IActionResult DownloadSaob()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            var allotments = (from list in _context.Yearly_reference where list.YearlyReference == "2021" select list).ToList();
            return PartialView(allotments);
        }
    }
}
