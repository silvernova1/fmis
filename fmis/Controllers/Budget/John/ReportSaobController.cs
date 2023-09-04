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
                                  FundAmountUacsId = fsa.UacsId,
                                  Date = o.Date,
                                  prexPap_title = pr.pap_title,
                                  prexPap_code = pr.pap_code1,
                                  fundsourceId = fs.FundSourceId,
                                  fundsTitle = fs.FundSourceTitle,
                                  Account_title = u.Account_title,
                                  expensesCode = u.Expense_code
                              }).Distinct().ToList();

            var suballotmentAmount = _MyDbContext.FundSources
                                   .Include(x => x.FundSourceAmounts)
                                   .Include(x => x.Uacs).ToList();


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
                    var rangeAU = worksheet.Range("A:U");
                    rangeAU.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
                    rangeAU.Cell(currentRow, 1).Style.Font.SetBold();
                    rangeAU.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                    rangeAU.Cell(currentRow, 1).Value = value;
                    currentRow++;

                    previousValue = value;
                }

            }
            string previousValue1 = null;
            void categoryRed(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                if (value != previousValue)
                {
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Value = value;
                    currentRow++;

                    previousValue1 = value;
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


                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 1).Value = prex.prexc.pap_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = prex.prexc.pap_code1;
                            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
                            currentRow++;

                            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                            var rangeAU = worksheet.Range("A1:U1");
                            rangeAU.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
                            rangeAU.Cell(currentRow, 1).Style.Font.SetBold();
                            rangeAU.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            rangeAU.Cell(currentRow, 1).Value = prex.AllotmentClass.Desc;
                            currentRow++;

                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }


                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Value = prex.Suballotment_title;
                        currentRow++;

                        foreach (var uacs in prex.SubAllotmentAmounts.ToList())
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                            currentRow++;


                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }




                    }// end of if statement 

                } // BudgetAllotment DashBoard year

            }// end of a foreach loop
            ItemSubPrexc(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            ItemSubPrexc(worksheet, ref currentRow, "Aapital Outlays");
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
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 1).Value = prexAdmin.prexc.pap_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = prexAdmin.prexc.pap_code1;
                            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
                            currentRow++;

                            var rangeAU = worksheet.Range("A1:U1");
                            rangeAU.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
                            rangeAU.Cell(currentRow, 1).Style.Font.SetBold();
                            rangeAU.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            rangeAU.Cell(currentRow, 1).Value = prexAdmin.AllotmentClass.Desc;
                            currentRow++;

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
            ItemSubPrexc(worksheet, ref currentRow, "TOTAL SAA'S 2023");
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
            foreach (var funsource in funsources)
            {
                if (funsource.FundAllotmentClassId == 1 && funsource.FundAppropriationId == 1 && funsource.FundBudgetAllotmentId == 3)
                {

                    if (funsource.FundAllotmentClassId == 1 && funsource.FundAppropriationId == 1 && funsource.FundBudgetAllotmentId == 3 && funsource.FundPrexcId == 4)
                    {

                        if (funsource.prexPap_title != previousPrexPapTitle || funsource.prexPap_code != previousPrexPapCode || funsource.AllotmentDesc != previousAllotmentDesc)
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 1).Value = funsource.prexPap_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = funsource.prexPap_code;
                            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
                            currentRow++;

                            var rangeAU = worksheet.Range("A1:U1");
                            rangeAU.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
                            rangeAU.Cell(currentRow, 1).Style.Font.SetBold();
                            rangeAU.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                            rangeAU.Cell(currentRow, 1).Value = funsource.AllotmentDesc;
                            currentRow++;

                            previousPrexPapTitle = funsource.prexPap_title;
                            previousPrexPapCode = funsource.prexPap_code;
                            previousAllotmentDesc = funsource.AllotmentDesc;
                        }


                        if (funsource.FundAllotmentClassId == 1 && funsource.FundAppropriationId == 1 && funsource.FundBudgetAllotmentId == 3 && funsource.fundsourceId == 56)
                        {

                            if (funsource.fundsTitle != preivousFunstitle)
                            {
                                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                                worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                                worksheet.Cell(currentRow, 1).Value = funsource.fundsTitle;
                                currentRow++;
                                preivousFunstitle = funsource.fundsTitle;
                            }

                            if (funsource.Account_title != PrevaccountTitle && funsource.expensesCode != prevExpenseCode)
                            {

                                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                                worksheet.Cell(currentRow, 1).Value = funsource.Account_title;

                                worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                                worksheet.Cell(currentRow, 2).Value = funsource.expensesCode;
                                currentRow++;
                                PrevaccountTitle = funsource.Account_title;
                                prevExpenseCode = funsource.expensesCode;


                            }


                        }//end of if 2023 STO-ORO-PS

                    } // year


                } //end of 2023



            }//end of foreach

            ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
            string duplicate1 = null;
            string duplicate2 = null;
            string duplicate3 = null;

            foreach (var sto_Or in funsources)
            {
                if (sto_Or.FundAllotmentClassId == 2 && sto_Or.FundAppropriationId == 1 && sto_Or.FundBudgetAllotmentId == 3 && sto_Or.fundsourceId == 63)
                {
                    if (sto_Or.fundsTitle != duplicate1)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 1).Value = sto_Or.fundsTitle;
                        currentRow++;

                        duplicate1 = sto_Or.fundsTitle;
                    }


                    if (sto_Or.Account_title != duplicate2 && sto_Or.expensesCode != duplicate3)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = sto_Or.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = sto_Or.expensesCode;
                        currentRow++;
                        duplicate2 = sto_Or.Account_title;
                        duplicate3 = sto_Or.expensesCode;
                    }

                }

            }// end of foreach
            ItemSubPrexc(worksheet, ref currentRow, "TOTAL, STO");
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
            foreach (var suballotInUacs in funsources)
            {
                if (suballotInUacs.AllotmentClassId == 2 && suballotInUacs.AppropriationId == 1 && suballotInUacs.budgetAllotmentId == 3 && suballotInUacs.prexcId == 7)
                {
                    if (suballotInUacs.prexPap_title != duplicate1 && suballotInUacs.prexPap_code != duplicate2 && suballotInUacs.AllotmentDesc != duplicate3)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Value = suballotInUacs.prexPap_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 2).Value = suballotInUacs.prexPap_code;
                        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
                        currentRow++;
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                        ItemSubPrexc(worksheet, ref currentRow, suballotInUacs.AllotmentDesc);
                        duplicate1 = suballotInUacs.prexPap_title;
                        duplicate2 = suballotInUacs.prexPap_code;
                        duplicate3 = suballotInUacs.AllotmentDesc;
                    }
                    if (suballotInUacs.AllotmentClassId == 2 && suballotInUacs.AppropriationId == 1 && suballotInUacs.budgetAllotmentId == 3 && suballotInUacs.fundsourceId == 82)
                    {
                        if (suballotInUacs.fundsTitle != duplicate1)
                        {
                            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                            worksheet.Cell(currentRow, 1).Value = suballotInUacs.fundsTitle;
                            currentRow++;
                            duplicate1 = suballotInUacs.fundsTitle;
                        }

                        if (suballotInUacs.Account_title != suballotUacs1 && suballotInUacs.expensesCode != suballotUacs2)
                        {
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = suballotInUacs.Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = suballotInUacs.expensesCode;
                            currentRow++;

                            suballotUacs1 = suballotInUacs.Account_title;
                            suballotUacs2 = suballotInUacs.expensesCode;
                        }


                    }//END 2023 HSRD 

                }

            }

            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Value = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "SERVICE DELIVERY SUB - PROGRAM";
            currentRow++;


            foreach (var subUacs in subAllotments)
            {
                if (subUacs.AllotmentClassId == 2 && subUacs.AppropriationId == 1 && subUacs.BudgetAllotmentId == 3 && subUacs.prexcId == 9)
                {

                    ItemSubPrexc(worksheet, ref currentRow, subUacs.AllotmentClass.Desc);


                    if (subUacs.AllotmentClassId == 2 && subUacs.AppropriationId == 1 && subUacs.SubAllotmentId == 317)
                    {
                        

                        if (subUacs.Suballotment_title != duplicate1) { 
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
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "TOTAL SAA'S 2023");
                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                
                  
                }
              
            }// end of foreach

            foreach(var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.SubAllotmentId == 441)
                {
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }
                }//end of 441

                if (suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.SubAllotmentId == 440)
                {
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }
                }//end of 440

                if (suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.SubAllotmentId == 441)
                {
                    categoryRed(worksheet, ref currentRow, suballot.Suballotment_title); // SAA 2023-02-000680

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    }
                } // end of 441
                if(suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.SubAllotmentId == 440)
                {
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        currentRow++;
                    } 
                }
            } //end of foreach

            //string previousPapTitle = null;

            //foreach (var item in subAllotments)
            //{



            //    //if (item.prexc.Id == 30 && item.SubAllotmentId == 328)
            //    //{
            //    //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //    //}
            //    //if (item.prexc.Id == 33 && item.SubAllotmentId == 211)
            //    //{
            //    //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //    //}
            //    //if (item.prexc.Id == 38 && item.SubAllotmentId == 417)
            //    //{
            //    //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //    //} 
            //    //if(item.prexc.Id == 24 && item.SubAllotmentId == 376)
            //    //{
            //    //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //    //}
            //    //if (item.prexc.Id == 16 && item.SubAllotmentId == 384)
            //    //{
            //    //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //    //}
            //    //   where ba.BudgetAllotmentId == 2 && sub.AllotmentClassId == 2 && sub.AppropriationId == 2

            //    if (item.prexc.pap_title != previousPapTitle)
            //    {


            //        worksheet.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
            //        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            //        worksheet.Cell(currentRow, 1).Value = item.prexc.pap_title;

            //        worksheet.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
            //        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
            //        worksheet.Cell(currentRow, 2).Value = item.prexc.pap_code1;
            //        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "00";
            //        currentRow++;


            //        if (item.prexc.Id == 1 && item.SubAllotmentId == 402)
            //        {
            //            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
            //            //ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

            //        }
            //        if (item.prexc.Id == 3 && item.SubAllotmentId == 298)
            //        {
            //            ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //            //  ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            //        }
            //        if (item.prexc.Id == 7 && item.SubAllotmentId == 371)
            //        {
            //            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
            //            //  ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //        }
            //        if (item.prexc.Id == 43 && item.SubAllotmentId == 335)
            //        {
            //            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
            //        }
            //        var rangeAU = worksheet.Range("A1:U1");
            //        rangeAU.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#fca1de");
            //        rangeAU.Cell(currentRow, 1).Style.Font.SetBold();
            //        rangeAU.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            //        rangeAU.Cell(currentRow, 1).Value = item.AllotmentClass.Desc;
            //        currentRow++;

            //        previousPapTitle = item.prexc.pap_title;

            //        // Reset Suballotment_title tracker
            //        HashSet<string> displayedSubAllotments = new HashSet<string>();



            //        //if (item.prexc.Id == 8 && item.SubAllotmentId == 363)
            //        //{
            //        //    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
            //        //}




            //    } //end of item.prexc.pap_titl



            //    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            //    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            //    worksheet.Cell(currentRow, 1).Value = item.Suballotment_title;
            //    currentRow++;

            //    foreach (var uacs in item.SubAllotmentAmounts.ToList())
            //    {

            //        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
            //        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

            //        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
            //        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
            //        currentRow++;


            //        //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
            //        //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


            //    }


            //}



            //var filteredFunsources = funsources.Where(o => o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday).ToList();
            // var filteredFunsources = funsources.Where(sa => sa.fundsourceId == sa.fundsourceId).ToList();
            // Get distinct category names
            //var fundsTitle = funsources.Select(fs => fs.fundsTitle).Distinct().ToList();

            //// Iterate through the fundsTitle and display each value in a separate row
            //foreach (var title in fundsTitle)
            //{
            //    if (string.IsNullOrEmpty(title)) // Skip empty or null titles
            //        continue;

            //    worksheet.Cell(currentRow, 1).Value = title;
            //    worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            //    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            //    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
            //    currentRow++;



            //    // Get the items belonging to the current category
            //    var itemsInCategory = funsources.Where(fs => fs.fundsTitle == title).ToList();

            //    // Create separate lists to store unique Account_title and expensesCode
            //    var uniqueAccountTitles = new List<string>();
            //    var uniqueExpensesCodes = new List<string>();

            //    // Iterate through the items and store unique Account_title and expensesCode
            //    foreach (var item in itemsInCategory)
            //    {
            //        if (!uniqueAccountTitles.Contains(item.Account_title))
            //            uniqueAccountTitles.Add(item.Account_title);

            //        if (!uniqueExpensesCodes.Contains(item.expensesCode))
            //            uniqueExpensesCodes.Add(item.expensesCode);
            //    }

            //    // Display the items and expensesCode in the same row for the current category
            //    foreach (var accountTitle in uniqueAccountTitles)
            //    {
            //        worksheet.Cell(currentRow, 1).Value = accountTitle;
            //        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9; // Replace 8 with the desired font size

            //        // Find the corresponding expensesCode for the accountTitle
            //        var expensesCode = itemsInCategory.FirstOrDefault(item => item.Account_title == accountTitle)?.expensesCode;
            //        worksheet.Cell(currentRow, 2).Value = expensesCode;
            //        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9; // Replace 8 with the desired font size

            //        currentRow++;
            //    }

            //    // Increment row to leave a blank row between each category
            //    currentRow++;
            //}






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
