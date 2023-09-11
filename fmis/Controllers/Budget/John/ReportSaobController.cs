using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using fmis.Data;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Globalization;
using fmis.Filters;
using Microsoft.EntityFrameworkCore;
namespace fmis.Controllers.Budget.John
{
    public class ReportSaobController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public int YearlyRefId { get; private set; }

        public ReportSaobController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        [Route("Dv/Saob2")]
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("budget_report", "ReportSaob", "");

            return View();
        }

        [HttpPost]
        //view FundSource/FundSource
        //view FundSource/FundSourceAmount/Uacs
        public IActionResult DownloadExcel(string fn, string date_from, string date_to, int? post_yearly_reference)
        {
            // string fn = Request.Query["fn"];
            date_from = Request.Query["date_from"];
            date_to = Request.Query["date_to"];
            // int? post_yearly_reference = int.TryParse(Request.Query["post_yearly_reference"], out int reference) ? reference : null;

            var timer = new Stopwatch();
            timer.Start();

            int id = YearlyRefId;
            if (post_yearly_reference != null)
            {
                id = YearlyRefId;
            }
            else
            {
                id = YearlyRefId;
            }

            DateTime date1 = Convert.ToDateTime(date_from);
            DateTime date2 = Convert.ToDateTime(date_to);
            DateTime datefilter = Convert.ToDateTime(date_to);
            String date3 = datefilter.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);

            date1 = date1.Date;
            date2 = date2.Date.AddDays(1).AddTicks(-1);
            DateTime dateTimeNow = date2;
            DateTime dateTomorrow = dateTimeNow.Date.AddDays(1);

            //LASTDAY OF The MONTH
            var firstDayOfMonth = new DateTime(date2.Year, date2.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime lastday = Convert.ToDateTime(lastDayOfMonth);
            lastday = lastday.Date.AddDays(1).AddTicks(-1);

            var funsources = (from fs in _MyDbContext.FundSources
                              join fsa in _MyDbContext.FundSourceAmount on fs.FundSourceId equals fsa.FundSourceId
                              join u in _MyDbContext.Uacs on fsa.UacsId equals u.UacsId
                              join o in _MyDbContext.Obligation on fs.FundSourceId equals o.FundSourceId
                              join pr in _MyDbContext.Prexc on fs.PrexcId equals pr.Id
                              join ba in _MyDbContext.Budget_allotments on fs.BudgetAllotmentId equals ba.BudgetAllotmentId
                              join Ap in _MyDbContext.Appropriation on fs.AppropriationId equals Ap.AppropriationId
                              join al in _MyDbContext.AllotmentClass on fs.AllotmentClassId equals al.Id
                              //  where   o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                              // where o.Date >= date1 && o.Date <= date2
                              select new
                              {

                                  FundAppropriationId = fs.AppropriationId,
                                  AppropriationId = Ap.AppropriationId,
                                  FundPrexcId = fs.PrexcId,
                                  prexcId = pr.Id,
                                  FundAllotmentClassId = fs.AllotmentClassId,
                                  AllotmentClassId = al.Id,
                                  AllotmentDesc = al.Desc,
                                  budgetAllotmentId = ba.BudgetAllotmentId,
                                  FundBudgetAllotmentId = fs.BudgetAllotmentId,
                                  UacsId = u.UacsId,
                                  funsoureAmountUacsId = fsa.UacsId,
                                  Date = o.Date,
                                  prexPap_title = pr.pap_title,
                                  prexPap_code = pr.pap_code1,
                                  fundsourceId = fs.FundSourceId,
                                  fundsTitle = fs.FundSourceTitle,
                                  Account_title = u.Account_title,
                                  expensesCode = u.Expense_code
                              }).ToList();

            var suballotmentAmount = _MyDbContext.FundSources
                                   .Include(x => x.FundSourceAmounts)
                                   .Include(x => x.Uacs).ToList();

            var funsources1 = _MyDbContext.FundSources
                           .Include(x => x.FundSourceAmounts)
                              .ThenInclude(x => x.Uacs)
                            .Include(x => x.Obligations)
                            .Include(x => x.Prexc)
                            .Include(x => x.BudgetAllotment)
                            .Include(x => x.Appropriation)
                            .Include(x => x.AllotmentClass)
                            .ToList();
            // var fundsource3 = (from fs in _MyDbContext.FundSources
            //join fsa in _MyDbContext.FundSourceAmount) 
            //return Json(columnData);

            // return Json(funsources.FirstOrDefault().fundsTitle);



            // Create a new Excel package
            // Create a new workbook
            var workbook = new XLWorkbook();

            // Add a worksheet
            var worksheet = workbook.Worksheets.Add("Sheet1");
            //worksheet.Cells.AutoFitColumns();
            //worksheet.Columns[1].AutoFit();
            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var currentRow = 17;


            worksheet.Cell(1, 9).Style.Font.SetBold();

            worksheet.Cell(2, 9).Style.Font.SetBold();

            worksheet.Cell(1, 9).Value = "STATEMENT OF ALLOTMENTS,OBLIGATIONS,DISBURSEMENT AND BALANCES:";
            worksheet.Cell(2, 9).Value = "As at April 30, 2023";
            worksheet.Cell(1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(2, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(4, 1).Style.Font.SetBold();
            worksheet.Cell(5, 1).Style.Font.SetBold();
            worksheet.Cell(6, 1).Style.Font.SetBold();
            worksheet.Cell(4, 1).Value = "Department:";
            worksheet.Cell(5, 1).Value = "Agency:";
            worksheet.Cell(6, 1).Value = "Fund:";


            //ws.Cell(14, 1).Style.Font.SetBold();
            //ws.Cell(14, 1).Style.Font.FontSize = 10;
            //ws.Cell(14, 1).Style.Font.FontName = "Lucida Bright";
            //ws.Cell(14, 1).Value = "P/A/P/ ALLOTMENT CLASS/ \n OBJECT OF EXPENDITURE";
            //ws.Cell(14, 1).Style.Alignment.WrapText = true;
            //ws.Cell(14, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell(14, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //ws.Range(ws.Cell(14, 1), ws.Cell(17, 11)).Merge();
            ////ws.Cell(14, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ////ws.Cell(17, 11).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            //ws.Rows(14, 19).Height = 16;
            //ws.Columns(1, 11).Width = 2.6
            worksheet.Cell(4, 2).Style.Font.SetBold();
            worksheet.Cell(5, 2).Style.Font.SetBold();
            worksheet.Cell(6, 2).Style.Font.SetBold();
            worksheet.Cell(4, 2).Value = "HEALTH:";
            worksheet.Cell(5, 2).Value = "Central Visayas Center For Health Development:";
            worksheet.Cell(6, 2).Value = "ALL FUND SOURCE";


            worksheet.Cell(8, 1).Value = "2";
            worksheet.Cell(8, 2).Value = "3";
            worksheet.Cell(8, 3).Value = "4";
            worksheet.Cell(8, 4).Value = "5";
            worksheet.Cell(8, 5).Value = "6";
            worksheet.Cell(8, 6).Value = "7:00";
            worksheet.Cell(8, 7).Value = "8.00";
            worksheet.Cell(8, 8).Value = "9.00";
            worksheet.Cell(8, 9).Value = "37.00";
            worksheet.Cell(8, 10).Value = "22.00";
            worksheet.Cell(8, 11).Value = "23.00";
            worksheet.Cell(8, 13).Value = "37.00";
            worksheet.Cell(8, 14).Value = "37.00";
            worksheet.Cell(8, 15).Value = "37.00";
            worksheet.Cell(8, 16).Value = "36.00";
            worksheet.Cell(8, 17).Value = "37.00";


            //Top Border  All Border
            var Range = worksheet.Range("A8:U8");
            Range.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            var Range1 = worksheet.Range("U9:U1000");
            Range1.Style.Border.RightBorder = XLBorderStyleValues.Thick;

            var Range2 = worksheet.Range("C11:U11");
            Range2.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            var Range3 = worksheet.Range("A9:A1000");
            Range3.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range4 = worksheet.Range("B9:B1000");
            Range4.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range5 = worksheet.Range("C9:C1000");
            Range5.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range6 = worksheet.Range("D9:D1000");
            Range6.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range7 = worksheet.Range("E9:E1000");
            Range7.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var range8 = worksheet.Range("F9:F1000");
            range8.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range9 = worksheet.Range("G9:G1000");
            Range9.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range10 = worksheet.Range("H9:H1000");
            Range10.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range11 = worksheet.Range("I10:I1000");
            Range11.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range12 = worksheet.Range("J10:J1000");
            Range12.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var RangeI12 = worksheet.Range("I9:J9");
            RangeI12.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            RangeI12.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range13 = worksheet.Range("K9:K1000");
            Range13.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range14 = worksheet.Range("L9:L1000");
            Range14.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range14LMNOP = worksheet.Range("L9:P9");
            Range14LMNOP.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            var Range15 = worksheet.Range("M9:M1000");
            Range15.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range16 = worksheet.Range("N9:N10000");
            Range16.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range17 = worksheet.Range("O9:O1000");
            Range17.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range18 = worksheet.Range("P9:P1000");
            Range18.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range19 = worksheet.Range("Q9:Q1000");
            Range19.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range20 = worksheet.Range("R9:R1000");
            Range20.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var RangeQR = worksheet.Range("Q9:R9");
            RangeQR.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            var Range21 = worksheet.Range("S9:S1000");
            Range21.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var Range22 = worksheet.Range("T9:T1000");
            Range22.Style.Border.RightBorder = XLBorderStyleValues.Thin;


            ////worksheet.Cell(9, 1).Style.Fill.BackgroundColor = XLColor.Pink;
            ////worksheet.Cell(9, 1).Value = "P/A/P/ ALLOTMENT CLASS/ \n OBJECT OF EXPENDITURE";
            ////worksheet.Cell(9, 1).Style.Font.FontSize = 10;
            //// Merge four rows
            //  Write the data to the worksheet 
            worksheet.Cell(14, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(14, 1).Value = "|. NEW APPROPRIATION (CURRENT)";
            worksheet.Cell(14, 1).Style.Font.FontSize = 9;
            worksheet.Cell(14, 1).Style.Font.SetBold();

            worksheet.Cell(15, 1).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(15, 1).Value = "A. PROGRAMS";
            worksheet.Cell(15, 1).Style.Font.FontSize = 9;
            worksheet.Cell(15, 1).Style.Font.SetBold();

            worksheet.Cell(16, 1).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(16, 1).Value = "|. GENERAL ADMINISTRATION AND SUPPORT";
            worksheet.Cell(16, 1).Style.Font.FontSize = 9;
            worksheet.Cell(16, 1).Style.Font.SetBold();
            //P/A/P/ ALLOTMENTS CLASS OBJECT OF EXPENDITURE





            //return Json(subUacs);



            var range1 = worksheet.Range("A9:A12");
            range1.Merge();
            range1.Value = "P/A/P/ ALLOTMENT \n CLASS/ \n OBJECT OF EXPENDITURE";
            range1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range1.Style.Alignment.WrapText = true;
            range1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range1.Style.Font.SetBold();
            range1.Style.Fill.BackgroundColor = XLColor.Pink;
            range1.Style.Font.FontSize = 10;
            worksheet.Column("A").Width = 25;



            var range2 = worksheet.Range("B9:B12");
            range2.Merge();
            range2.Value = "EXPENSES \n CODE";
            range2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range2.Style.Alignment.WrapText = true;
            range2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range2.Style.Font.SetBold();
            range2.Style.Fill.BackgroundColor = XLColor.Pink;
            range2.Style.Font.FontSize = 10;
            worksheet.Column("B").Width = 15;
            //worksheet.Cell(9, 2).Style.Font.SetBold();
            //worksheet.Cell(9, 2).Value = " EXPENSES \n CODE";
            //worksheet.Cell(9, 2).Style.Font.FontSize = 10;
            // worksheet.Row(9).Height = 16;

            var range3 = worksheet.Range("C9:C12");
            range3.Merge();
            range3.Value = "GAA 2023/\nCONAP BALANCE\n2022";
            range3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range3.Style.Alignment.WrapText = true;
            range3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range3.Style.Font.SetBold();
            range3.Style.Fill.BackgroundColor = XLColor.Pink;
            range3.Style.Font.FontSize = 10;
            worksheet.Column("C").Width = 18;
            //worksheet.Cell(9, 3).Style.Font.SetBold();
            //worksheet.Cell(9, 3).Value = "GAA 2023 /\n CONAP BALANCE \n 2022";
            //worksheet.Cell(9, 3).Style.Font.FontSize = 11;
            //worksheet.Column("C").Width = 20;
            // Set text on top of the second row
            //worksheet.Cell(9, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            //worksheet.Cell(9, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //worksheet.Cell(9, 3).Style.Fill.BackgroundColor = XLColor.Pink;
            //worksheet.Cell(10, 3).Style.Fill.BackgroundColor = XLColor.Pink;
            //worksheet.Cell(11, 3).Style.Fill.BackgroundColor = XLColor.Pink;
            // worksheet.Columns[3].AutoFit();
            var range4 = worksheet.Range("D9:D12");
            range4.Merge();
            range4.Value = "ADDITIONAL\nSARO\n CY2013";
            range4.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range4.Style.Alignment.WrapText = true;
            range4.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range4.Style.Font.SetBold();
            range4.Style.Fill.BackgroundColor = XLColor.Pink;
            range4.Style.Font.FontSize = 10;
            worksheet.Column("D").Width = 12;

            var range5 = worksheet.Range("E9:E12");
            range5.Merge();
            range5.Value = "REALIGNMENT/\nNORSA";
            range5.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range5.Style.Alignment.WrapText = true;
            range5.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range5.Style.Font.SetBold();
            range5.Style.Fill.BackgroundColor = XLColor.Pink;
            range5.Style.Font.FontSize = 10;
            worksheet.Column("E").Width = 15;


            var range6 = worksheet.Range("F9:F12");
            range6.Merge();
            range6.Value = "SAA\nTRANSFER TO\n CO/OU'S CY\n2023";
            range6.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range6.Style.Alignment.WrapText = true;
            range6.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range6.Style.Font.SetBold();
            range6.Style.Fill.BackgroundColor = XLColor.Pink;
            range6.Style.Font.FontSize = 10;
            worksheet.Column("F").Width = 15;

            var range7 = worksheet.Range("G9:G12");
            range7.Merge();
            range7.Value = "SAA TRANSFER\nFROM CO/CHD\nCY2023";
            range7.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range7.Style.Alignment.WrapText = true;
            range7.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range7.Style.Font.SetBold();
            range7.Style.Fill.BackgroundColor = XLColor.Pink;
            range7.Style.Font.FontSize = 10;
            worksheet.Column("G").Width = 18;

            var range8h = worksheet.Range("H9:H12");
            range8h.Merge();
            range8h.Value = "TOTAL ADJUSTED\n ALLOTMENT";
            range8h.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range8h.Style.Alignment.WrapText = true;
            range8h.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range8h.Style.Font.SetBold();
            range8h.Style.Fill.BackgroundColor = XLColor.Pink;
            range8h.Style.Font.FontSize = 10;
            worksheet.Column("H").Width = 18;

            worksheet.Column("I").Width = 21;
            worksheet.Column("J").Width = 21;
            worksheet.Cell("I9").Value = "";
            worksheet.Cell("J9").Value = "";
            var MergeRange = worksheet.Range("I9:J9").Merge();
            MergeRange.FirstCell().Value = "OBLIGATIONS 2023";
            MergeRange.Style.Font.FontSize = 10;
            MergeRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            MergeRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            MergeRange.Style.Font.SetBold();
            MergeRange.Style.Fill.BackgroundColor = XLColor.SkyBlue;

            var range9 = worksheet.Range("I10:I12").Merge();
            range9.Value = "This Report\nApril";
            range9.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range9.Style.Alignment.WrapText = true;
            range9.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range9.Style.Font.SetBold();
            range9.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range9.Style.Font.FontSize = 10;
            worksheet.Column("I").Width = 19;

            var range10 = worksheet.Range("J10:J12").Merge();
            range10.Value = "To Date (JAN-\nDEC 2023)";
            range10.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range10.Style.Alignment.WrapText = true;
            range10.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range10.Style.Font.SetBold();
            range10.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range10.Style.Font.FontSize = 10;
            worksheet.Column("J").Width = 19;

            //worksheet.Cell(10, 10).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            //worksheet.Cell(10, 10).Value = "To Date (JAN-DEC 2023)";
            //worksheet.Cell(10, 10).Style.Font.FontSize = 10;
            //worksheet.Cell(10, 10).Style.Font.SetBold();
            //worksheet.Cell(11, 9).Style.Fill.BackgroundColor = XLColor.SkyBlue;
            //worksheet.Cell(11, 10).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            var range11 = worksheet.Range("K9:K12").Merge();
            range11.Value = "UNOBLIGATED\nBALANCE OF\nALLOTMENT";
            range11.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range11.Style.Alignment.WrapText = true;
            range11.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range11.Style.Font.SetBold();
            range11.Style.Fill.BackgroundColor = XLColor.Pink;
            range11.Style.Font.FontSize = 10;
            worksheet.Column("K").Width = 20;

            // worksheet.Column("K").Width = 22;
            // worksheet.Cell(9, 11).Value = "UNOBLIGATED BALANCE OF ALLOTMENT";
            // worksheet.Cell(9, 11).Style.Font.FontSize = 10;
            // worksheet.Cell(9, 11).Style.Font.SetBold();
            // worksheet.Cell(9, 11).Style.Fill.BackgroundColor = XLColor.Pink;
            // worksheet.Cell(10, 11).Style.Fill.BackgroundColor = XLColor.Pink;
            // worksheet.Cell(11, 11).Style.Fill.BackgroundColor = XLColor.Pink;
            // worksheet.Cell(9, 11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            //// worksheet.Cell(9, 11).Style.Alignment.WrapText = true;
            // worksheet.Cell(9, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Column("L").Width = 20;
            worksheet.Column("M").Width = 20;
            worksheet.Column("N").Width = 20;
            worksheet.Column("O").Width = 20;
            worksheet.Column("P").Width = 20;
            //Merge cells
            worksheet.Cell("L9").Value = "";
            worksheet.Cell("M9").Value = "";
            worksheet.Cell("N9").Value = "";
            worksheet.Cell("O9").Value = "";
            worksheet.Cell("P9").Value = "";

            var MergedColumns = worksheet.Range("L9:P9").Merge();
            MergedColumns.FirstCell().Value = "DISBURSEMENTS 2023";
            MergedColumns.Style.Font.FontSize = 10;
            MergedColumns.Style.Font.SetBold();
            MergedColumns.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            MergedColumns.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            MergedColumns.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            var range12 = worksheet.Range("L10:L12").Merge();
            range12.Value = "THIS REPORT\nJANUARY";
            range12.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range12.Style.Alignment.WrapText = true;
            range12.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range12.Style.Font.SetBold();
            range12.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range12.Style.Font.FontSize = 10;
            worksheet.Column("L").Width = 20;

            var range13 = worksheet.Range("M10:M12").Merge();
            range13.Value = "THIS REPORT\nFEBRUARY";
            range13.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range13.Style.Alignment.WrapText = true;
            range13.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range13.Style.Font.SetBold();
            range13.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range13.Style.Font.FontSize = 10;
            worksheet.Column("M").Width = 20;

            //worksheet.Cell(10, 13).Value = "THIS REPORT FEBRUARY";
            //worksheet.Cell(10, 13).Style.Font.FontSize = 10;
            //worksheet.Cell(10, 13).Style.Fill.BackgroundColor = XLColor.SkyBlue;

            var range14 = worksheet.Range("N10:N12").Merge();
            range14.Value = "THIS REPORT\nMARCH";
            range14.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range14.Style.Alignment.WrapText = true;
            range14.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range14.Style.Font.SetBold();
            range14.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range14.Style.Font.FontSize = 10;
            worksheet.Column("N").Width = 20;


            var range15 = worksheet.Range("O10:O12").Merge();
            range15.Value = "THIS REPORT DECEMBER";
            range15.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range15.Style.Alignment.WrapText = true;
            range15.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range15.Style.Font.SetBold();
            range15.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range15.Style.Font.FontSize = 10;
            worksheet.Column("O").Width = 25;

            var range16 = worksheet.Range("P10:P12").Merge();
            range16.Value = "TO DATE(JAN-\nDEC 2023)";
            range16.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range16.Style.Alignment.WrapText = true;
            range16.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range16.Style.Font.SetBold();
            range16.Style.Fill.BackgroundColor = XLColor.SkyBlue;
            range16.Style.Font.FontSize = 10;
            worksheet.Column("P").Width = 18;

            var range16r = worksheet.Range("Q9:R9").Merge();
            range16r.Value = "UNPAID OBLIGATIONS";
            range16r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range16r.Style.Alignment.WrapText = true;
            range16r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range16r.Style.Font.SetBold();
            range16r.Style.Fill.BackgroundColor = XLColor.Yellow;
            range16r.Style.Font.FontSize = 10;

            var range17 = worksheet.Range("Q10:Q12").Merge();
            range17.Value = "DUE AND\nDEMANDABLE";
            range17.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range17.Style.Alignment.WrapText = true;
            range17.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range17.Style.Font.SetBold();
            range17.Style.Fill.BackgroundColor = XLColor.Yellow;
            range17.Style.Font.FontSize = 10;
            worksheet.Column("Q").Width = 18;

            var range18 = worksheet.Range("R10:R12").Merge();
            range18.Value = "NOT YET DUE AND\nDEMANDABLE";
            range18.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range18.Style.Alignment.WrapText = true;
            range18.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range18.Style.Font.SetBold();
            range18.Style.Fill.BackgroundColor = XLColor.Yellow;
            range18.Style.Font.FontSize = 10;
            worksheet.Column("R").Width = 18;

            var range19 = worksheet.Range("S9:S12").Merge();
            range19.Value = "% OBLIG\nRATE";
            range19.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range19.Style.Alignment.WrapText = true;
            range19.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range19.Style.Font.SetBold();
            range19.Style.Fill.BackgroundColor = XLColor.Pink;
            range19.Style.Font.FontSize = 10;
            worksheet.Column("S").Width = 9;

            var range20 = worksheet.Range("T9:T12").Merge();
            range20.Value = "% DISB\nRATE";
            range20.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range20.Style.Alignment.WrapText = true;
            range20.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range20.Style.Font.SetBold();
            range20.Style.Fill.BackgroundColor = XLColor.Pink;
            range20.Style.Font.FontSize = 10;
            worksheet.Column("T").Width = 9;

            var range21 = worksheet.Range("U9:U12").Merge();
            range21.Value = "REMARKS";
            range21.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range21.Style.Alignment.WrapText = true;
            range21.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range21.Style.Font.SetBold();
            range21.Style.Fill.BackgroundColor = XLColor.Pink;
            range21.Style.Font.FontSize = 10;
            worksheet.Column("U").Width = 23;

            // Freeze the first Top Row column
            worksheet.SheetView.Freeze(12, 5);

            // Freeze the second side column
            worksheet.SheetView.FreezeColumns(2);


            //var subAllotments = _MyDbContext.SubAllotment
            //                    .Include(x => x.SubAllotmentAmounts)
            //                        .ThenInclude(x=>x.Uacs)
            //                    .Include(x => x.prexc)
            //                    .OrderByDescending(x => x.Suballotment_title)
            //                    .ToList();

            var subAllotments = _MyDbContext.SubAllotment
                .Include(X => X.AllotmentClass)
                .Include(x => x.SubAllotmentAmounts)
                     .ThenInclude(x => x.Uacs)
                .Include(x => x.prexc)
                .OrderBy(x => x.prexc.pap_title)
                .ThenByDescending(x => x.Suballotment_title)
                .ToList();

            string previousValue = null;

            void ItemSubPrexc(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                if (value != previousValue)
                {
                    // Define the range from cell A to U in the row
                    var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21)); // Columns A to U (1 to 21)

                    // Set the background color for each cell within the range
                    for (int col = 1; col <= 21; col++) // Loop through columns A to U
                    {
                        var cell = rangeAtoU.Cell(1, col);
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
                    }

                    // Set the value in the first cell of the row (column A)
                    rangeAtoU.FirstCell().Value = value;

                    // Apply a thin border to the entire range
                    rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Increment currentRow for the next row
                    currentRow++;

                    // Update the previousValue
                    previousValue = value;
                }
            }



            string previousValue2 = null;
            void TOTALSaa(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                if (value != previousValue)
                {

                    var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21)); // Columns A to U (1 to 21)
                    // Set the background color for each cell within the range
                    for (int col = 1; col <= 21; col++) // Loop through columns A to U
                    {
                        var cell = rangeAtoU.Cell(1, col);
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#DAD3D1");
                    }
                    // Set the value in the first cell of the row (column A)
                    rangeAtoU.Style.Font.SetBold();
                    rangeAtoU.FirstCell().Value = value;
                    // Apply a thin border to the entire range
                    rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    //currentRow++;
                    previousValue2 = value;
                }
            }

            // string previousValue1 = null;
            void SubAllotTitleRed(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                //if (value != previousValue)
                //  {
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                worksheet.Cell(currentRow, 1).Value = value;
                currentRow++;

                // previousValue1 = value;
                // }

            }

            string notduplicate1 = null;
            void Prexc_papTitle(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                if (value != notduplicate1)
                {
                    var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                    rangeAtoU.Style.Fill.BackgroundColor = XLColor.FromHtml("#BBDBD0"); // Green
                    rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 1).Value = value;
                    notduplicate1 = value;
                }

            }
            string notDuplicate = null;
            void Prexc_papcode(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                if (value != notDuplicate)
                {
                    var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                    rangeAtoU.Style.Fill.BackgroundColor = XLColor.FromHtml("#BBDBD0"); // Green
                    rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 2).Value = value;
                    worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
                    currentRow++;
                    notDuplicate = value;
                }

            }



            bool totalSaa = false;
            string paptitle = null;
            string papcode = null;
            string previousPapTitle1 = null;
            foreach (var prex in subAllotments)

            {
                if (prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3)
                {

                    if (prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1)
                    {
                        if (prex.prexc.pap_title != paptitle || prex.prexc.pap_code1 != papcode)
                        {

                            Prexc_papTitle(worksheet, ref currentRow, prex.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, prex.prexc.pap_code1);
                            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", prex.Beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            ItemSubPrexc(worksheet, ref currentRow, prex.AllotmentClass.Desc);
                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }


                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Value = prex.Suballotment_title;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", prex.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;

                        foreach (var uacs in prex.SubAllotmentAmounts.ToList())
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                         
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", _MyDbContext.Suballotment_amount.FirstOrDefault(x => x.UacsId == uacs.UacsId).beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                            
                          
                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }
                    }// end of if statement 

                } // BudgetAllotment DashBoard year

            }// end of a foreach loop


            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            bool staticDataAdded = false;
            foreach (var prexAdmin in subAllotments)
            {
                if (prexAdmin.AllotmentClassId == 1 && prexAdmin.AppropriationId == 1 && prexAdmin.BudgetAllotmentId == 3)
                {


                    if (prexAdmin.BudgetAllotmentId == 3 && prexAdmin.AppropriationId == 1 && prexAdmin.AllotmentClassId == 1 && prexAdmin.prexcId == 2)
                    {
                        if (prexAdmin.prexc.pap_title != paptitle && prexAdmin.prexc.pap_code1 != papcode)
                        {
                            Prexc_papTitle(worksheet, ref currentRow, prexAdmin.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, prexAdmin.prexc.pap_code1);
                            ItemSubPrexc(worksheet, ref currentRow, prexAdmin.AllotmentClass.Desc);

                            paptitle = prexAdmin.prexc.pap_title;
                            papcode = prexAdmin.prexc.pap_code1;
                        }


                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Value = prexAdmin.Suballotment_title;
                        currentRow++;


                        foreach (var uacs in prexAdmin.SubAllotmentAmounts.ToList())
                        {
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                            currentRow++;

                        }

                    }// end of if statement 


                }// BudgetAllotment DashBoard year

            }// end of a foreach loop


            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "||. SUPPORT TO OPERATIONS";
            currentRow++;
            string preivousFunstitle = null;
            string PrevaccountTitle = null;
            string prevExpenseCode = null;
            string previousPrexPapTitle = null;
            string previousPrexPapCode = null;
            string previousAllotmentDesc = null;
            var uniqueValues3 = new HashSet<string>();
            var uniqueValues2 = new HashSet<string>();
           

            foreach (var funsource in funsources1)
            {
                if (funsource.AllotmentClassId == 1 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 4)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Value = funsource.FundSourceTitle;
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsource.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;
                        currentRow++;
                    }
                } // year condition

                if (funsource.AllotmentClassId == 2 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 4)
                   {

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsource.Beginning_balance);
                    ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc); 

                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Value = funsource.FundSourceTitle;

                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsource.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;

                    foreach (var uacs in funsource.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", _MyDbContext.FundSourceAmount.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                        

                    }

                } // year

            }//end of foreach

            string duplicate1 = null;
            string duplicate2 = null;
            string duplicate3 = null;

            TOTALSaa(worksheet, ref currentRow, "TOTAL, STO");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Value = "|||. OPERATIONS";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to promotive and preventive health care services improved";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "HEALTH POLICY AND STANDARDS DEVELOPMENT PROGRAM";
            currentRow++;

            string suballotUacs1 = null;
            string suballotUacs2 = null;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 7)
                {
                    if (funsorce.Prexc.pap_title != duplicate1 && funsorce.Prexc.pap_code1 != duplicate2 && funsorce.AllotmentClass.Desc != duplicate3)
                    {
                        Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                        duplicate1 = funsorce.Prexc.pap_title;
                        duplicate2 = funsorce.Prexc.pap_code1;
                        duplicate3 = funsorce.AllotmentClass.Desc;
                    }

                    if (funsorce.FundSourceTitle != duplicate1)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                        worksheet.Cell(currentRow, 1).Value = funsorce.FundSourceTitle;
                        currentRow++;
                        duplicate1 = funsorce.FundSourceTitle;
                    }

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }

                }

            }//end of foreach 

            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            var uniqueValues = new HashSet<string>();
            var uniqueValues1 = new HashSet<string>();
            foreach (var saaSub in subAllotments)
            {


                if (saaSub.AllotmentClassId == 2 && saaSub.AppropriationId == 1 && saaSub.BudgetAllotmentId == 3 && saaSub.prexcId == 8)
                {
                    var valueAdd = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
                    if (uniqueValues.Add(valueAdd))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        currentRow++;
                    }
                    var valueAd1 = "SERVICE DELIVERY SUB - PROGRAM";
                    if (uniqueValues.Add(valueAd1))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = valueAd1;
                        currentRow++;
                    }

                    //if (saaSub.prexc.pap_title != duplicate1 && saaSub.prexc.pap_code1 != duplicate2 && saaSub.AllotmentClass.Desc != duplicate3)
                    //{
                    Prexc_papTitle(worksheet, ref currentRow, saaSub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, saaSub.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, saaSub.AllotmentClass.Desc);

                    //duplicate1 = saaSub.prexc.pap_title;
                    //duplicate2 = saaSub.prexc.pap_code1;
                    //duplicate3 = saaSub.AllotmentClass.Desc;
                    //}
                    SubAllotTitleRed(worksheet, ref currentRow, saaSub.Suballotment_title);


                    foreach (var uacs in saaSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                    }//list of Item
                }

            } //end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;

            foreach (var subUacs in subAllotments)
            {
                if (subUacs.AllotmentClassId == 2 && subUacs.AppropriationId == 1 && subUacs.BudgetAllotmentId == 3 && subUacs.prexcId == 9)
                {
                    Prexc_papTitle(worksheet, ref currentRow, subUacs.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, subUacs.prexc.pap_code1);

                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    if (subUacs.Suballotment_title != duplicate1)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Value = subUacs.Suballotment_title;
                        currentRow++;
                        duplicate1 = subUacs.Suballotment_title;
                    }

                    foreach (var uacs in subUacs.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                    }
                } //end year


            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");


            foreach (var listUacs in subAllotments)
            {
                if (listUacs.AllotmentClassId == 3 && listUacs.AppropriationId == 1 && listUacs.BudgetAllotmentId == 3 && listUacs.prexcId == 9)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, listUacs.Suballotment_title);
                    foreach (var uacs in listUacs.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }
                }//end of 441


            }//end of foreach   
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;

            HashSet<string> uniqueAccountTitles1 = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes1 = new HashSet<string>();
            foreach (var fundsorce in funsources1)
            {

                if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 10)
                {
                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);

                    if (fundsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);
                        duplicate1 = fundsorce.FundSourceTitle;
                    }

                    foreach (var uacs in fundsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }
                }


            }//end of foreach
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 10)
                {
                    //  if (suballot.Suballotment_title != duplicate1)
                    if (suballot.Suballotment_title != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;
                    }


                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                        //   duplicate1 = suballot.Suballotment_title;
                    }
                }
            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;


            foreach (var subaalot in subAllotments)
            {
                if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 11)
                {
                    Prexc_papTitle(worksheet, ref currentRow, subaalot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, subaalot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");


                    if (subaalot.Suballotment_title != duplicate1)
                    {

                        SubAllotTitleRed(worksheet, ref currentRow, subaalot.Suballotment_title);
                    }
                    foreach (var uacs in subaalot.SubAllotmentAmounts.ToList())
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                    }


                } // year
            } //end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;
            HashSet<string> uniqueAccountTitles = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes = new HashSet<string>();
            foreach (var funsource in funsources1)
            {
                if (funsource.AllotmentClassId == 2 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 13)
                {
                    var valueAdd = "HEALTH HUMAN RESOURCE SUB - PROGRAM";
                    if (uniqueValues.Add(valueAdd))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        currentRow++;
                    }

                    Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);

                    if (funsource.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsource.FundSourceTitle);
                        duplicate1 = funsource.FundSourceTitle;
                    }

                    foreach (var uacs in funsource.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }



                } //year and prexc
            } // end of foreach
            foreach (var suballot1 in subAllotments)
            {
                if (suballot1.AllotmentClassId == 2 && suballot1.AppropriationId == 1 && suballot1.BudgetAllotmentId == 3 && suballot1.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot1.Suballotment_title);

                    foreach (var uacs in suballot1.SubAllotmentAmounts.ToList())
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                    }

                }

            }//end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;
            HashSet<string> uniqueAccountTitles2 = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes2 = new HashSet<string>();
            foreach (var fundsorce in funsources1)
            {

                if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 32)
                {
                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);

                    if (fundsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);
                        duplicate1 = fundsorce.FundSourceTitle;
                    }
                    foreach (var uacs in fundsorce.FundSourceAmounts)
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }

                }
            } //end of foreach

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 1 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 32)
                {

                    if (suballot.Suballotment_title != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;
                    }
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;

                    }

                }
            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;

            HashSet<string> uniqueAccountTitles3 = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes3 = new HashSet<string>();
            foreach (var funsoce in funsources1)
            {
                if (funsoce.AllotmentClassId == 2 && funsoce.AppropriationId == 1 && funsoce.BudgetAllotmentId == 3 && funsoce.PrexcId == 32)
                {
                    ItemSubPrexc(worksheet, ref currentRow, funsoce.AllotmentClass.Desc);

                    if (funsoce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsoce.FundSourceTitle);
                        duplicate1 = funsoce.FundSourceTitle;
                    }
                    foreach (var uacs in funsoce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }



                }
            }//end of foreach
            currentRow++;
            string duplicate4 = null;
            string duplicate5 = null;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 32)
                {
                    if (suballot.Suballotment_title != duplicate5)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate5 = suballot.Suballotment_title;
                    }
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }

                }

            }//end of foreach 
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;


            HashSet<string> uniqueAccountTitles4 = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes4 = new HashSet<string>();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Health PromotionSUB - PROGRAM";
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 14)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");


                    if (funsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        duplicate1 = funsorce.FundSourceTitle;
                    }

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }

                } //prexcId

            }//end foreach

            foreach (var suballot in subAllotments)
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 14)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }

            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH PROGRAM";
            var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Locally-Funded Project(s)";
            var rangeAtoU1 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU1.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 40)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH MANAGEMENT SUB - PROGRAM";
            var rangeAtoU2 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU2.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            HashSet<string> uniqueAccountTitles5 = new HashSet<string>();
            HashSet<string> uniqueExpenseCodes5 = new HashSet<string>();
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 15)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    var valueAdd = "GAA 2023";
                    if (uniqueValues.Add(valueAdd))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        currentRow++;
                    }

                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    if (funsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        duplicate1 = funsorce.FundSourceTitle;
                    }

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            } // end of foreach
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 15)
                {
                    if (suballot.Suballotment_title != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;
                    }

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }// end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "ENVIRONMENTAL AND OCCUPATIONAL HEALTH SUB - PROGRAM";
            var rangeAtoU3 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU3.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 16)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                    if (suballot.Suballotment_title != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;
                    }

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                    TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
                    currentRow++;
                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                }
            }//end of foreach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "FAMILY HEALTH SUB - PROGRAM";
            var rangeAtoU4 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU4.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 18)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreaach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU5 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU5.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 37)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreaach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Epidemiology and SurveillancePROGRAM";
            var rangeAtoU6 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU6.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU6.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 24)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }

            } //end of foreach
            foreach (var subaalot in subAllotments)
            {
                if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 24)
                {
                    Prexc_papTitle(worksheet, ref currentRow, subaalot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, subaalot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, subaalot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, subaalot.Suballotment_title);

                    foreach (var uacs in subaalot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH EMERGENCY MANAGEMENT PROGRAM";
            var rangeAtoU7 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU7.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU7.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 24)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }

            }//end of foreach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to curative and rehabilitative health care services improved";
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES OPERATION PROGRAM";
            var rangeAtoU8 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU8.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU8.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "CURATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU9 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU9.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU9.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 27)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }


                }
            }//end of foreach
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 27)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 34)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "REHABILITATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU10 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU10.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU10.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 28)
                {
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach 
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 28)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach 
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to safe and quality health commodities, devices and facilities ensured";
            var rangeAtoU11A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU11A.Style.Fill.BackgroundColor = XLColor.FromHtml("#13ecec"); //skyblue
            rangeAtoU11A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH REGULATORY PROGRAM";
            var rangeAtoU11 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU11.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU11.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES AND SERVICES REGULATION SUB - PROGRAM";
            var rangeAtoU12 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU12.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU12.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var funsorce in funsources1)
            {
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 29)
                {

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }

            }//end of foreach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to social health protection assured";
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "SOCIAL HEALTH PROTECTION PROGRAM";
            var rangeAtoU13 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU13.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU13.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 30)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    //ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }

            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Locally-Funded Project(s)";
            var rangeAtoU14 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU14.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU14.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 17)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, OPERATIONS";
            var rangeAtoU15 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU15.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU15.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL NEW APPROPRIATIONS";
            var rangeAtoU16 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU16.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU16.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Personnel Services";
            var rangeAtoU17 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU17.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Maintenance & Other Operating Expenses";
            var rangeAtoU18 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU18.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU18.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Capital Outlays";
            var rangeAtoU19 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU19.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU19.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "II. AUTOMATIC APPROPRIATIONS";
            var rangeAtoU20 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU20.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU20.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var funsorce in funsources1)
            {
                //FOR Retirement and Life Insurance Premium
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 4)
                {
                    if (funsorce.FundSourceId == 60)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                        foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                        {
                            var rangeAtoU31 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                            rangeAtoU31.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCC3FF"); //pink
                            rangeAtoU31.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            currentRow++;
                        }
                    }
                }
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 15)
                {
                    if (funsorce.FundSourceId == 62)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                        {

                            var rangeAtoU31 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                            rangeAtoU31.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCC3FF"); //pink
                            rangeAtoU31.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            currentRow++;
                        }
                    }
                }


                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 29)
                {
                    if (funsorce.FundSourceId == 61)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                        foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                        {

                            var rangeAtoU31 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                            rangeAtoU31.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCC3FF"); //pink
                            rangeAtoU31.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            currentRow++;
                        }

                    }
                }

            }//end of foreach

            //Retirement and Life Insurance Premium
            //2023 STO-ORO-RLIP
            //2023 PHM-RLIP
            //2023 RRHFS-RLIP



            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL CURRENT APPROPRIATION";
            var rangeAtoU21 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU21.Style.Fill.BackgroundColor = XLColor.FromHtml("#EEC20C");
            rangeAtoU21.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = "TOTAL CURRENT APPROPRIATION";
            var rangeAtoU22 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU22.Style.Fill.BackgroundColor = XLColor.FromHtml("#151410");
            rangeAtoU22.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;



            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Value = "|. NEW APPROPRIATION (CONAP)";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(currentRow, 1).Value = "A. PROGRAMS";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Black;
            worksheet.Cell(currentRow, 1).Value = "|. GENERAL ADMINISTRATION AND SUPPORT";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            currentRow++;

            foreach (var conapsub in subAllotments)
            {
                if (conapsub.AllotmentClassId == 2 && conapsub.AppropriationId == 1 && conapsub.BudgetAllotmentId == 1 && conapsub.prexcId == 1)
                {

                    Prexc_papTitle(worksheet, ref currentRow, conapsub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, conapsub.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, conapsub.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, conapsub.Suballotment_title);

                    foreach (var uacs in conapsub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }


            }//end of foreach
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, GAS";
            var rangeAtoU23 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU23.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU23.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "II. SUPPORT TO OPERATIONS";
            currentRow++;

            //Health Information Technolog / 200000100001000
            //CONAP SAA 2022-10-4920

            foreach (var conapSub in subAllotments)
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 1 && conapSub.BudgetAllotmentId == 1 && conapSub.prexcId == 35)
                {
                    Prexc_papTitle(worksheet, ref currentRow, conapSub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, conapSub.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, conapSub.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);

                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }

            }//end of foreach 

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;
            foreach (var conapSub in subAllotments)
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 2 && conapSub.BudgetAllotmentId == 3 && conapSub.prexcId == 35)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);

                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }


            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            //Capital Outlays 
            //CONAP SAA 2022-03-0961 
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, GAS";
            var rangeAtoU26 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU26.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU26.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "II. SUPPORT TO OPERATIONS";
            currentRow++;

            //Health Information Technolog / 200000100001000
            //CONAP SAA 2022-10-4920

            foreach (var conapSub in subAllotments)
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 1 && conapSub.BudgetAllotmentId == 1 && conapSub.prexcId == 35)
                {
                    Prexc_papTitle(worksheet, ref currentRow, conapSub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, conapSub.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, conapSub.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);

                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }

            }//end of foreach 

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;
            foreach (var conapSub in subAllotments)
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 2 && conapSub.BudgetAllotmentId == 3 && conapSub.prexcId == 35)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);

                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }


            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            //Capital Outlays 
            //CONAP SAA 2022-03-0961 
            //Building  5060404001
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, STO";
            var rangeAtoU28 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU28.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU28.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "III. OPERATIONS";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to promotive and preventive health care services improved";
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
            var rangeAtoU27 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU27.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU27.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "SERVICE DELIVERY SUB - PROGRAM";
            currentRow++;
            foreach (var prexcData in subAllotments)
            {
                if (prexcData.prexcId == 9)
                {
                    Prexc_papTitle(worksheet, ref currentRow, prexcData.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, prexcData.prexc.pap_code1);
                }
            }//end of foreach

            ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            currentRow++;

            //CONAP SAA 2022-02-0539
            //Hospital and health center
            //medical equipment
            //CONAP SAA 2022-02-0539
            //Motor Vehicles
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 11)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH HUMAN RESOURCE SUB - PROGRAM";
            var rangeAtoU29 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU29.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU29.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 13)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }//end of if statement

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 32)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Health PromotionSUB - PROGRAM";
            var rangeAtoU30 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU30.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU30.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            //CONAP 2022 HEALTH PROMOTION
            //Health Promotion - 310203100001000

            foreach (var SubAllot in subAllotments)
            {
                if (SubAllot.AllotmentClassId == 2 && SubAllot.AppropriationId == 1 && SubAllot.BudgetAllotmentId == 1 && SubAllot.prexcId == 14)
                {
                    Prexc_papTitle(worksheet, ref currentRow, SubAllot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, SubAllot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, SubAllot.AllotmentClass.Desc);
                    // No Data CONAP 2022 HEALTH PROMOTION
                    SubAllotTitleRed(worksheet, ref currentRow, SubAllot.Suballotment_title);

                    foreach (var uacs in SubAllot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }

                }
            }//end foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            //Public Health Emergency Benefits and Allowances for Health Care - 310300200003000 , CONAP SAA 2022-02-0791

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 40)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            currentRow++;

            foreach (var suballot in subAllotments)
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 38)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }


            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;

            foreach (var suballot in subAllotments)
            {


                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 38)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 31)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;

            //worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            //worksheet.Cell(currentRow, 1).Value = "Foreign-Assisted Projects";
            //var rangeAtoU32 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            //rangeAtoU32.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            //rangeAtoU32.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //currentRow++;

            //foreach(var suballot in subAllotments)
            //{
            //    if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 42)
            //    {
            //        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
            //        Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
            //        ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
            //    }
            //}
            //TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            //currentRow++;
            //currentRow++;
            //World Bank Philippine Multi-Sectoral Nutrition Project
            //CONAP SAA 23-03-00000461


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "FAMILY HEALTH SUB - PROGRAM";
            var rangeAtoU32 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU32.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU32.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 18)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU33 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU33.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU33.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 37)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");

            currentRow++;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 37)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH PROGRAM";
            var rangeAtoU36 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU36.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU36.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;


            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 24)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            //SAA # 2023 - No data in database
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            //CONAP SAA 2022-05-2673 - No data in database

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH EMERGENCY MANAGEMENT PROGRAM";
            var rangeAtoU34 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU34.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU34.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 25)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 26)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to curative and rehabilitative health care services improved";
            var rangeAtoU11C = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU11C.Style.Fill.BackgroundColor = XLColor.FromHtml("#13ecec"); //skyblue
            rangeAtoU11C.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES OPERATION PROGRAM";
            var rangeAtoU836 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU836.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU836.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "CURATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU37 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU37.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU37.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 27)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 34)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "REHABILITATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU38 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU38.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU38.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;


            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 28)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to safe and quality health commodities, devices and facilities ensured";
            var rangeAtoU11B = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU11B.Style.Fill.BackgroundColor = XLColor.FromHtml("#13ecec"); //skyblue
            rangeAtoU11B.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH REGULATORY PROGRAM";
            var rangeAtoU39 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU39.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoU39.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES AND SERVICES REGULATION SUB - PROGRAM";
            var rangeAtoU40 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU40.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU40.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;


            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 41)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to social health protection assured";
            var rangeAtoU11D = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU11D.Style.Fill.BackgroundColor = XLColor.FromHtml("#13ecec"); //skyblue
            rangeAtoU11D.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "SOCIAL HEALTH PROTECTION PROGRAM";
            var rangeAtoUE = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoUE.Style.Fill.BackgroundColor = XLColor.FromHtml("#DD96D7");
            rangeAtoUE.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;



            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 30)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            currentRow++;
            currentRow++;


            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 30)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, OPERATIONS";
            var rangeAtoU15A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU15A.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU15A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL NEW APPROPRIATIONS";
            var rangeAtoU16A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU16A.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU16A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Maintenance & Other Operating Expenses ";
            var rangeAtoU17A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17A.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU17A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Capital Outlays ";
            var rangeAtoU18A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU18A.Style.Fill.BackgroundColor = XLColor.FromHtml("E1D9D9");
            rangeAtoU18A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL CURRENT APPROPRIATION";
            var rangeAtoU21A = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU21A.Style.Fill.BackgroundColor = XLColor.FromHtml("#EEC20C");
            rangeAtoU21A.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Row(1).Height = 40;
            worksheet.Cell(currentRow, 1).Value = "GRAND TOTAL ";
            var rangeAtoUA = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoUA.Style.Fill.BackgroundColor = XLColor.FromHtml("#33FFBD");
            rangeAtoUA.Style.Border.BottomBorder = XLBorderStyleValues.Double;
            currentRow++;
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 1).Value = "Prepared by:";

            worksheet.Cell(currentRow, 6).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 6).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 6).Value = "Certified Correct:";

            worksheet.Cell(currentRow, 12).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 12).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 12).Value = "Recommending Approval:";

            worksheet.Cell(currentRow, 18).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 18).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 18).Value = "Approved by:";
            currentRow++;
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 1).Value = "TIMOTHY JOHN ARRIESGADO";

            worksheet.Cell(currentRow, 3).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 3).Value = "PHILLIP NIERRAS";

            worksheet.Cell(currentRow, 6).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 6).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 6).Value = "LEONORA A. ANIEL";

            worksheet.Cell(currentRow, 12).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 12).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 12).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 12).Value = "RAMIL R. ABREA, CPA, MBA";

            worksheet.Cell(currentRow, 18).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 18).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 18).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 18).Value = "JAIME S. BERNADAS, MD, MGM, CESO lll";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 1).Value = "Administrative Assistant III";

            worksheet.Cell(currentRow, 3).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 3).Value = "Accountant I";

            worksheet.Cell(currentRow, 6).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 6).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 6).Value = "Budget Officer III";

            worksheet.Cell(currentRow, 12).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 12).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 12).Value = "Chief, Management Support Division";

            worksheet.Cell(currentRow, 18).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(currentRow, 18).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 18).Value = "Director IV";
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Times New Roman";
            worksheet.Row(1).Height = 20;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            var richText2 = worksheet.Cell(currentRow, 3).RichText.AddText("as to obligations.");
            richText2.Italic = true;

            worksheet.Cell(currentRow, 3).Style.Font.FontName = "Times New Roman";
            worksheet.Row(1).Height = 20;
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 12;
            var richText1 = worksheet.Cell(currentRow, 3).RichText.AddText("as to Disbursements");
            richText1.Italic = true;
            worksheet.Cell(currentRow, 6).Style.Font.FontName = "Times New Roman";
            worksheet.Row(1).Height = 20;
            worksheet.Cell(currentRow, 6).Style.Font.FontSize = 12;
            var richText = worksheet.Cell(currentRow, 6).RichText.AddText("as to obligations.");
            richText.Italic = true;
            currentRow++;



            // Create a memory stream to hold the Excel file content
            var stream = new System.IO.MemoryStream();

            // Save the workbook to the memory stream
            workbook.SaveAs(stream);

            // Set the position of the memory stream to 0
            stream.Position = 0;

            // Return the Excel file for download
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FMIS-DOHCVCHD.xlsx");





        }//end of function






    }
}
