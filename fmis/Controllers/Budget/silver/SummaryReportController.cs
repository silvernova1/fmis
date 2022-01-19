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
using fmis.Models.John;
using fmis.ViewModel;
using fmis.Models.silver;

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
                                   on fundsource.BudgetAllotmentId equals budget_allotment.BudgetAllotmentId

                                    select new SummaryReportViewModel
                                   {
                                       fund_source = fundsource,
                                       prexc = prexc,
                                       uacs = uacs,
                                       fundsource_amount = fundsource_amount
                                       //obligationamount = obligationamount

                                    }).ToList();

            ViewBag.filter = new FilterSidebar("master_data", "SummaryReports");

            var sumfunds = _context.FundSourceAmount.Where(s => s.BudgetAllotmentId == id).Sum(x => x.beginning_balance);
            ViewBag.sumfunds = sumfunds.ToString("C", new CultureInfo("en-PH"));

            var totalbudget = _context.FundSourceAmount.Sum(x => x.beginning_balance);
            ViewBag.totalbudget = totalbudget.ToString("C", new CultureInfo("en-PH"));

            return View(generate_reports);

        }

        /*public ActionResult Filter(DateTime date_from, DateTime date_to)
        {
            using (MyDbContext db = new MyDbContext())
            {

                var filter = db.Budget_allotments
            .Where(x => x.Created_at == date_from);
                return View();
            }

        }*/

        [HttpPost]
        public IActionResult ExportSummaryReports()
        {
            
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
                ws.Cell("A2").Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell("A2").Style.Fill.BackgroundColor = XLColor.White;
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


                var budget_allotments = _context.Budget_allotments
                    .Include(budget_allotment => budget_allotment.FundSources)
                    .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                    .ThenInclude(uacs => uacs.Uacs);
/*                    .Include(ObligationAmount => ObligationAmount);*/

                foreach (BudgetAllotment budget_allotment in budget_allotments)
                {

                    foreach (FundSource fundSource in budget_allotment.FundSources)
                    {
                      
                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts)
                        {

                            ws.Cell(currentRow, 1).Value = _context.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 2).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.FundSourceTitle;
                            ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 3).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_title;
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            /*ws.Cell(currentRow, 3).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_title;
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);*/

                            ws.Cell(currentRow, 5).Value = _context.FundsRealignment.FirstOrDefault(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId)?.Realignment_amount;
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 6).Value = _context.ObligationAmount.FirstOrDefault(x => x.ObligationId == fundSource.FundSourceId)?.Amount;
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 7).Value = _context.FundSources.FirstOrDefault(x => x.FundSourceId == fundSource.FundSourceId)?.Remaining_balance;
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 8).Value = _context.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            currentRow++;
                            total = (double)fundsource_amount.beginning_balance;

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
