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
                            .Include(x => x.Obligations.Where(o => o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday))
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
            //P/A/P/ ALLOTMENTS CLASS OBJECT OF EXPENDITUR
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
            var Beginning_balance = _MyDbContext.Suballotment_amount
                   .Include(x => x.Uacs)
                   .ToList();
            var subAllotments = _MyDbContext.SubAllotment
                .Include(X => X.AllotmentClass)
                .Include(x => x.SubAllotmentAmounts)
                     .ThenInclude(x => x.Uacs)
                .Include(x => x.Obligations.Where(o => o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday))
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

            void SaaGaaBalance(IXLWorksheet worksheet, ref int currentRow, string value)
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = value;
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }

            bool totalSaa = false;
            string paptitle = null;
            string papcode = null;
            HashSet<string> uniqueItems = new HashSet<string>();
            //   var totalBeginningBalance = _MyDbContext.SubAllotment
            //.Where(prex =>
            //    prex.AllotmentClassId == 2 &&
            //    prex.AppropriationId == 1 &&
            //    prex.BudgetAllotmentId == 3 &&
            //    prex.prexcId == 1)
            //.Sum(prex => prex.Beginning_balance);
          


            
            var SaaPS2 = subAllotments.Where(prex => prex.AllotmentClassId == 1 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);
            var SaaMOOE1 = subAllotments.Where(prex => prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);
            var SaaCO1 = subAllotments.Where(prex => prex.AllotmentClassId == 3 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);

            var FundsourcePS1 = funsources1.Where(prex => prex.AllotmentClassId == 1 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);
            var FundsourceMOOE1 = funsources1.Where(prex => prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);
            var FundsourceCO1 = funsources1.Where(prex => prex.AllotmentClassId == 3 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);

            decimal totalPSGeneral = SaaPS2 + FundsourcePS1;
            decimal totalMOOEGeneral = SaaMOOE1 + FundsourceMOOE1;
            decimal totalCOGeneral = SaaCO1 + FundsourceCO1;
            decimal totalPrexcGeneral = totalPSGeneral + totalMOOEGeneral + totalCOGeneral;

      if (funsources1.Any(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 1)) //General Management and Supervision :: for Funsorce
            { 
                foreach (var fundsorce in funsources1)
                {
                    if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 1)
                    {
                        if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                        {
                            
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexcGeneral));
                                Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                            
                            if (totalPSGeneral == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPSGeneral));
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            if (totalMOOEGeneral == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOEGeneral));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            paptitle = fundsorce.Prexc.pap_title;
                            papcode = fundsorce.Prexc.pap_code1;
                        }


                        SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);

                        currentRow++;

                        foreach (var uacs in fundsorce.FundSourceAmounts.ToList())
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }
                    }
                }//end of foreach
      }//end of if Statement to check if there is data
      else
         {
             foreach (var prex in subAllotments)
             {
                 if (prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1)
                  {
                        if (prex.prexc.pap_title != paptitle || prex.prexc.pap_code1 != papcode)
                        {
                            
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexcGeneral));
                                Prexc_papTitle(worksheet, ref currentRow, prex.prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, prex.prexc.pap_code1);
                            
                            if (totalPSGeneral == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPSGeneral));
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }


                            if (totalMOOEGeneral == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOEGeneral));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }

                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }
                    }
             }
        } // end of else statement
          foreach (var prex in subAllotments)//General Management and Supervision //100000100001000 // SAA 2023-03-001317
            {
                if (prex.AllotmentClassId == 2 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 1)
                    {
                        
                        SubAllotTitleRed(worksheet, ref currentRow, prex.Suballotment_title);
                        
                        currentRow++;

                        foreach (var uacs in prex.SubAllotmentAmounts.ToList())
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                       
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }
                } // BudgetAllotment DashBoard year

            }// end of a foreach loop

                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE1));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
                currentRow++;

            if (totalCOGeneral == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                currentRow++;
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCOGeneral));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                currentRow++;
            }


            var SaaPS1 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 2).Sum(x => x.Beginning_balance);
            var SaaMOOE = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 2).Sum(x => x.Beginning_balance);
            var SaaCO = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 2).Sum(x => x.Beginning_balance);

            var FundsourcePS = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 2).Sum(x => x.Beginning_balance);
            var FundsourceMOOE = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 2).Sum(x => x.Beginning_balance);
            var FundsourceCO = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 2).Sum(x => x.Beginning_balance);

            decimal totalPS_Adminis = SaaPS1 + FundsourcePS;
            decimal totalMOOE_Adminis = SaaMOOE + FundsourceMOOE;
            decimal totalCO_Adminis = SaaCO + FundsourceCO;
            decimal totalPrexc_Adminis = totalPS_Adminis + totalMOOE_Adminis + SaaCO + FundsourceCO;
            if (funsources1.Any(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 2)) // check this data if naa
            {
                foreach (var funsorce in funsources1)//Administration of Personnel Benefits// 100000100002000  Fundsorces1
                {
                    if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 2)
                    {
                        if (funsorce.Prexc.pap_title != paptitle && funsorce.Prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Adminis));
                            Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                         if(totalPS_Adminis == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Adminis));
                                ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                            }
                            
                        if(totalMOOE_Adminis == 0)
                            {

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Adminis));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                            }
                           

                            paptitle = funsorce.Prexc.pap_title;
                            papcode = funsorce.Prexc.pap_code1;
                        }
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        currentRow++;
                        foreach (var uacs in funsorce.FundSourceAmounts)
                        {
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }
                    }
                }//end of foreach
            }
            else
            {
                foreach(var prex in subAllotments)
                {
                    if (prex.AllotmentClassId == 1 && prex.AppropriationId == 1 && prex.BudgetAllotmentId == 3 && prex.prexcId == 2)
                    {
                        if (prex.prexc.pap_title != paptitle && prex.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Adminis));
                            Prexc_papTitle(worksheet, ref currentRow, prex.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, prex.prexc.pap_code1);
                            if (totalPS_Adminis == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Adminis));
                                ItemSubPrexc(worksheet, ref currentRow, prex.AllotmentClass.Desc);
                            }

                            if (totalMOOE_Adminis == 0)
                            {

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Adminis));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                            }

                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }
                    }
                }// end of foreach
            }//end of else statement

            bool staticDataAdded = false;
            foreach (var prexAdmin in subAllotments)//Administration of Personnel Benefits// 100000100002000 //SAA 2023-07-003501
            {
                if (prexAdmin.AllotmentClassId == 1 && prexAdmin.AppropriationId == 1 && prexAdmin.BudgetAllotmentId == 3 && prexAdmin.prexcId == 2)
                {

                        SubAllotTitleRed(worksheet, ref currentRow, prexAdmin.Suballotment_title);
                        currentRow++;


                        foreach (var uacs in prexAdmin.SubAllotmentAmounts.ToList())
                        {
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }   
                }// BudgetAllotment DashBoard year

            }// end of a foreach loop
            
         
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaPS1));
            currentRow++;
            if(totalCO_Adminis == 0)
            {
               
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Adminis));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            decimal TotalGas = totalPrexc_Adminis + totalPrexcGeneral;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "TOTAL, GAS";
            var rangeAtoU108 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU108.Style.Fill.BackgroundColor = XLColor.FromHtml("6DCAE3");
            rangeAtoU108.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", TotalGas));

            currentRow++;
            currentRow++;

            string preivousFunstitle = null;
            string PrevaccountTitle = null;
            string prevExpenseCode = null;
            string previousPrexPapTitle = null;
            string previousPrexPapCode = null;
            string previousAllotmentDesc = null;
            var uniqueValues3 = new HashSet<string>();
            var uniqueValues2 = new HashSet<string>();
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "||. SUPPORT TO OPERATIONS";
            currentRow++;
            bool isAllotmentClassDisplayed = false;

            var SaaPS4 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 4).Sum(x => x.Beginning_balance);
            var SaaMOOE3 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 4).Sum(x => x.Beginning_balance); 
            var SaaCO3 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 4).Sum(x => x.Beginning_balance);

            var FundsourcePS3 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var FundsourceMOOE3 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var FundsourceCO3 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            decimal totalPS_Operation = SaaPS4 + FundsourcePS3;
            decimal totalMOOE_Operation = SaaMOOE3 + FundsourceMOOE3;
            decimal totalCO_Operation = SaaCO3 + FundsourceCO3;
            decimal totalPrexc_Operation = totalPS_Operation + totalMOOE_Operation + totalCO_Operation;

            var STO_Oro_Beginning_balanced = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var Sto_Oro_Ps_BeginningBalance = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            decimal totalSumSto_Oro = Sto_Oro_Ps_BeginningBalance + STO_Oro_Beginning_balanced; //Operations of Regional Offices 200000100002000 //2023 STO-ORO-PS

            bool sto_oro_displayed = false;

            var SaaPS3 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 3).Sum(x => x.Beginning_balance);
            var SaaMOOE2 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 3).Sum(x => x.Beginning_balance);
            var SaaCO2 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 3).Sum(x => x.Beginning_balance);

            var FundsourcePS2 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 3).Sum(x => x.Beginning_balance);
            var FundsourceMOOE2 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 3).Sum(x => x.Beginning_balance);
            var FundsourceCO2 =   funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 3).Sum(x => x.Beginning_balance);

            decimal totalPS_Health = SaaPS3 + FundsourcePS2;
            decimal totalMOOE_Health = SaaMOOE2 + FundsourceMOOE2;
            decimal totalCO_Health = SaaCO2 + FundsourceCO2;
            decimal totalPrexc_Health = totalPS_Health + totalMOOE_Health + totalCO_Health;
            if (funsources1.Any(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 3))
            {
                foreach (var funsorce in funsources1)// Health Information Technology 200000100001000
                {
                    if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 3)
                    {
                        if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                        {

                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health));
                            Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                            if (totalPS_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health));
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            if (totalMOOE_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health));
                                ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                            }

                            paptitle = funsorce.Prexc.pap_title;
                            papcode = funsorce.Prexc.pap_code1;
                        }
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        currentRow++;

                        foreach (var uacs in funsorce.FundSourceAmounts)
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;

                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }

                    }


                }//end of foreach
            }
            else
            {
                foreach (var suballot in subAllotments)
                {
                    if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 3)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {

                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health));
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            if (totalMOOE_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }
            }// end of else statement
           

            foreach (var subaalot in subAllotments)// Health Information Technology 200000100001000 continue::
            {
                if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 3)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, subaalot.Suballotment_title);
                    currentRow++;

                    foreach (var uacs in subaalot.SubAllotmentAmounts.ToList())
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                        //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                    }
                }

            }//end of foreach
             if(SaaMOOE2 == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE2));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
              currentRow++;
            if (totalCO_Health == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Health));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }

            currentRow++;

        if(funsources1.Any(x =>x.AllotmentClassId == 1  && x.AllotmentClassId == 2 && x.AllotmentClassId == 3  && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4))
            {
                foreach (var funsource in funsources1)//Operations of Regional Offices 200000100002000 //2023 STO-ORO-PS
                {
                    if (funsource.AllotmentClassId == 1 || funsource.AllotmentClassId == 2 || funsource.AllotmentClassId == 3  && funsource.AllotmentClassId == 3 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 4)
                    {
                        if (funsource.Prexc.pap_title != paptitle || funsource.Prexc.pap_code1 != papcode)
                        {

                        
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Operation));
                            Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);

                            if(totalPS_Operation == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Operation));
                                ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);
                            }
                            if(totalMOOE_Operation == 0)
                            {
                                
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Operation));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            
                            paptitle = funsource.Prexc.pap_title;
                            papcode = funsource.Prexc.pap_code1;
                        }
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Value = funsource.FundSourceTitle;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;

                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
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
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }
                    } // year condition

                }
            }
            else
            {
                foreach (var suballot in subAllotments)
                {
                    if (suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3 && suballot.AllotmentClassId == 2 && suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 4)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {


                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Operation));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Operation == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Operation));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Operation == 0)
                            {

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Operation));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }//end of foreach
            } // end of else
            foreach (var suballot in subAllotments)//Operations of Regional Offices 200000100002000 //suballotments
            {
                if (suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3 && suballot.AllotmentClassId == 2 && suballot.AllotmentClassId == 3 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 4)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        currentRow++;

                        foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                        {

                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;

                            //worksheet.Cell(currentRow, 2).Style.Font.FontSize = 8;
                            //worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        }

                    }
                }
            }//end of foreach



            //foreach (var funsource in funsources1)//2023 STO-ORO
            //{
            //    if (funsource.AllotmentClassId == 2 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 4)
            //    {   

            //        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            //        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            //        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", STO_Oro_Beginning_balanced);

            //        ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc); 

            //        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            //        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
            //        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 1).Value = funsource.FundSourceTitle;

            //        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
            //        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            //        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsource.Beginning_balance);
            //        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            //        currentRow++;

            //        foreach (var uacs in funsource.FundSourceAmounts.ToList())
            //        {
            //            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
            //            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

            //            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
            //            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;
                   
            //            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            //            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
            //            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            //            currentRow++;
              
            //        }

            //    } // year

            //}//end of foreach

            string duplicate1 = null;
            string duplicate2 = null;
            string duplicate3 = null;

         
            TOTALSaa(worksheet, ref currentRow, "TOTAL, STO");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}",  totalSumSto_Oro);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            var PS9 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            var CO7  = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            var totalsaa3 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            decimal total_prexc_CO7 = totalsaa3 + PS9 + CO7; //Health Sector Research Development Health Sector Research Development //    2023 HSRD



            string suballotUacs1 = null;
            string suballotUacs2 = null;
            var SuballotPS9 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 6).Sum(x => x.Beginning_balance);
            var SuballotCO = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 6).Sum(x => x.Beginning_balance);
            var totalSaaMOEE = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 6).Sum(x => x.Beginning_balance);
            //Health Sector Policy and Plan Development  310100100001000 Continue::

            var PS11 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 6).Sum(x => x.Remaining_balance);
            var CO9 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 6).Sum(x => x.Remaining_balance);
            var totalfunsorce4 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 6).Sum(x => x.Remaining_balance);
            decimal totalfuns_CO8_PS11 = totalfunsorce4 + totalSaaMOEE + CO9 + PS11 + SuballotCO + SuballotPS9; //Health Sector Policy and Plan Development  310100100001000

            var totalSaa6 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 5).Sum(x => x.Beginning_balance);
            var SuballotPS = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 5).Sum(x => x.Beginning_balance);
            var CO15 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 5).Sum(x => x.Beginning_balance);
            //International Health Policy Development and Cooperation  310100100001000 continue

            var PS10 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 5).Sum(x => x.Remaining_balance);
            var CO8 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 5).Sum(x => x.Remaining_balance);
            var totalfunsorce3 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 5).Sum(x => x.Remaining_balance);
            decimal totalSaaFunsorce = totalfunsorce3 + totalSaa6;
            decimal totalfuns_CO8_PS10 = totalSaaFunsorce + CO8 + CO15 + PS10 + SuballotPS; //International Health Policy Development and Cooperation  310100100001000
          
            decimal health_and_Standard_Dev = totalfuns_CO8_PS10 + totalfuns_CO8_PS11 + total_prexc_CO7;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Value = "|||. OPERATIONS";
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to promotive and preventive health care services improved";
            var rangeAtoU17A11 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17A11.Style.Fill.BackgroundColor = XLColor.FromHtml("C8E0F4");
            rangeAtoU17A11.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            // 574,873,000 OO : Access to promotive and preventive health care services improved
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 8;
            worksheet.Cell(currentRow, 1).Value = "HEALTH POLICY AND STANDARDS DEVELOPMENT PROGRAM";
            var rangeAtoU17A1 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17A1.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU17A1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

         
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", health_and_Standard_Dev);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            
            currentRow++;

            //  total_prexc_CO7
            foreach (var funsorce in funsources1) //International Health Policy Development and Cooperation  310100100001000
            {
               
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 5) 
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalfuns_CO8_PS10);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
       
                        Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    decimal totalPs = PS10 + SuballotPS;
                    if (totalPs == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPs);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                  
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                      
                         worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                         worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaaFunsorce);
                         worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }

                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                    worksheet.Cell(currentRow, 1).Value = funsorce.FundSourceTitle;

                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
              
           }// end of foreach

           
            foreach (var suballot in subAllotments)//International Health Policy Development and Cooperation  310100100001000 continue
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 5)
                {

                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                    worksheet.Cell(currentRow, 1).Value = suballot.Suballotment_title;

                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;

                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            if(totalSaaFunsorce == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = "-";
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaaFunsorce);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            decimal totalCO4 = CO15 + CO8;
            if (totalCO4 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO4);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }

            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

           
            foreach (var funsorce in funsources1) //Health Sector Policy and Plan Development  310100100001000
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 6)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                    
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalfuns_CO8_PS11);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                  decimal totalPS2 = PS11 + SuballotPS9;
                    if (totalPS2 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPS2);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                     
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalfunsorce4);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                    worksheet.Cell(currentRow, 1).Value = funsorce.FundSourceTitle;

                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            }// end of foreach

            foreach (var suballot in subAllotments)//Health Sector Policy and Plan Development  310100100001000 Continue::
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 6)
                {
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                    worksheet.Cell(currentRow, 1).Value = suballot.Suballotment_title;

                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;

                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }

            }
            if(totalSaaMOEE == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = "-";
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaaMOEE);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            currentRow++;
            decimal totalCO5 = CO9 + SuballotCO;
            if (totalCO5 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO5);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            foreach (var funsorce in funsources1) //Health Sector Research Development Health Sector Research Development //    2023 HSRD
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 7)
                {
                    if (funsorce.Prexc.pap_title != duplicate1 && funsorce.Prexc.pap_code1 != duplicate2 && funsorce.AllotmentClass.Desc != duplicate3)
                    {
                       
                       
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", total_prexc_CO7);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                        if(PS9 == 0)
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS9);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        }
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalsaa3);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                        duplicate1 = funsorce.Prexc.pap_title;
                        duplicate2 = funsorce.Prexc.pap_code1;
                        duplicate3 = funsorce.AllotmentClass.Desc;
                    }
                    
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.25;
                    worksheet.Cell(currentRow, 1).Value = funsorce.FundSourceTitle;

                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                       

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            }//end of foreach 

            if(CO7 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO7);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            var uniqueValues = new HashSet<string>();
            var uniqueValues1 = new HashSet<string>();
            bool SaaDiplayed = false;
            bool saaDisplayed = false;

            var totalSaa3 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 14).Sum(x => x.Beginning_balance);
            var totalfunsorce1 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            var PS4 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            var PS3 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 14).Sum(x => x.Beginning_balance);
            var CO4 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            var CO3 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 14).Sum(x => x.Beginning_balance);
            decimal totalCO3 = CO4 + CO3;
            decimal totalPS3 = PS3 + PS4;
            decimal MOOE = totalSaa3 + totalfunsorce1;
            decimal totalSaa3_totalfunsorce1_PS_CO = MOOE + totalPS3 + totalCO3; ///2023 HEALTH PROMOTION 310203100001000 for Health PromotionSUB - PROGRAM


            var totalCO1 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 9).Sum(x => x.Beginning_balance); // SAA 2023 - 02 - 000162 - INFRA //SAA 2023-02-000680-INFRA
            var PS6 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 9).Sum(x => x.Beginning_balance);
            var totalSaa5 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 9).Sum(x => x.Beginning_balance);
           decimal Prexcs_PS6_COsaa5 = totalSaa5 + totalCO1 + PS6;  //Health Facilities Enhancement Program  310201100002000 /SAA 2023-02-000451
    

            var totalFund = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 10).Sum(x => x.Beginning_balance);
            var totalSaa7 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 10).Sum(x => x.Beginning_balance);
            var SaaPS0 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 10).Sum(x => x.Beginning_balance);
            var PS = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 10).Sum(x => x.Beginning_balance);
             var SaaOL0 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 10).Sum(x => x.Beginning_balance);
            var OL1 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 10).Sum(x => x.Beginning_balance);
            decimal OL0_OL1 = SaaOL0 + OL1;
            decimal PS_SaaPS0 = PS + SaaPS0;
            decimal totalSaaFund = totalFund + totalSaa7;
            decimal totalLocalHealth = totalSaaFund + PS_SaaPS0 + OL0_OL1; //Local Health Systems Development and Assistance  // 310201100003000 

            var totalSaa8 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 11).Sum(x => x.Beginning_balance);
            var PS1 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 11).Sum(x => x.Beginning_balance);
            var CO1 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 11).Sum(x => x.Beginning_balance);
            decimal totalSaaPharmaceu = totalSaa8 + PS1 + CO1; //Pharmaceutical Management

            var Saa_SAA_000199 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            var totalfunsorce = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var NHWSS = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);

            var SaaFunsourceTotal = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            decimal SaaFunsourceTotal_SaaTotal1_NHWSS = SaaFunsourceTotal + Saa_SAA_000199 + totalfunsorce + NHWSS; //NHWSS/
            var PS7 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 8).Sum(x => x.Beginning_balance);
            var totalSaa4 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 8).Sum(x => x.Beginning_balance);
            var totalCO2 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 8).Sum(x => x.Beginning_balance);
            decimal totalPSCO = totalSaa4 + PS7 + totalCO2;

            var COE_1 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 32).Sum(x => x.Beginning_balance);
            var COE_2 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var PS_1 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 32).Sum(x => x.Beginning_balance);
            var PS_2 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var totalsaa5 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 32).Sum(x => x.Beginning_balance);
            var totalSaa9 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 32).Sum(x => x.Beginning_balance);
            decimal COE_1COE_2 = COE_2 + COE_1;
            decimal PS_1PS2 = PS_1 + PS_2;

            decimal MOOE_totalSaa9 = NHWSS + totalSaa9;
            decimal NHWSSGRandTotal = PS_1PS2 + MOOE_totalSaa9 + COE_1COE_2;//total of  National Health Workforce Support System(NHWSS)

            var CO2 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            var CO6 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            var PS2 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            var PS5 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            decimal totalPS = PS2 + PS5; decimal totalCO = CO2 + CO6;
            decimal MOOE1 = SaaFunsourceTotal + Saa_SAA_000199;
            decimal totalPrexTitle_code = totalPS + MOOE1 + totalCO;
            decimal total_Health_HumanSub = totalPrexTitle_code + NHWSSGRandTotal; //NHWSSGRandTotal - National Health Workforce Support System ( NHWSS ) // 310202100002000 // 2023 HRHICM

            decimal service_delivery_sub_Prexc = totalSaaPharmaceu + totalPSCO + totalLocalHealth + Prexcs_PS6_COsaa5;//prexcTotal SERVICE DELIVERY SUB - PROGRAM

            decimal TotalHealthSystem_STRENGTHENING = service_delivery_sub_Prexc + totalSaa3_totalfunsorce1_PS_CO + total_Health_HumanSub;
            foreach (var saaSub in subAllotments) //Health Facility Policy and Plan Development  310201100001000
            {


                if (saaSub.AllotmentClassId == 2 && saaSub.AppropriationId == 1 && saaSub.BudgetAllotmentId == 3 && saaSub.prexcId == 8)
                {
                    var valueAdd = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
                    if (uniqueValues.Add(valueAdd))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        var rangeAtoU101 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                        rangeAtoU101.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
                        rangeAtoU101.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", TotalHealthSystem_STRENGTHENING);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                    var valueAd1 = "SERVICE DELIVERY SUB - PROGRAM";
                    if (uniqueValues.Add(valueAd1))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = valueAd1;
                        var rangeAtoU102 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                        rangeAtoU102.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
                        rangeAtoU102.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", service_delivery_sub_Prexc);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                  if (saaSub.prexc.pap_title != paptitle || saaSub.prexc.pap_code1 != papcode)
                  {
                       

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPSCO);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                  
                    Prexc_papTitle(worksheet, ref currentRow, saaSub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, saaSub.prexc.pap_code1);

                    if(PS7 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS7);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                 
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                   


                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa4);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, saaSub.AllotmentClass.Desc);
                    paptitle = saaSub.prexc.pap_title;
                    papcode = saaSub.prexc.pap_code1;
                 }
                    SubAllotTitleRed(worksheet, ref currentRow, saaSub.Suballotment_title);
                     currentRow++;
                    foreach (var uacs in saaSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                    }//list of Item
                }

            } //end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa4);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            currentRow++;
            currentRow++;
           
            foreach (var subUacs in subAllotments)//Health Facilities Enhancement Program  310201100002000 /SAA 2023-02-000451
            {
                if (subUacs.AllotmentClassId == 2 && subUacs.AppropriationId == 1 && subUacs.BudgetAllotmentId == 3 && subUacs.prexcId == 9)
                {
                    if (subUacs.prexc.pap_title != paptitle || subUacs.prexc.pap_code1 != papcode)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", Prexcs_PS6_COsaa5);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, subUacs.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, subUacs.prexc.pap_code1);
                    if (PS6 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS6);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }

                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa5);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                        paptitle = subUacs.prexc.pap_title;
                        papcode = subUacs.prexc.pap_code1;
                    }

                    SubAllotTitleRed(worksheet, ref currentRow, subUacs.Suballotment_title);
                        currentRow++;
                   
                    foreach (var uacs in subUacs.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;

                    }
                } //end year


            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa5);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            currentRow++;


            
            if (totalCO1 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO1);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays"); 

            foreach (var listUacs in subAllotments) //SAA 2023-02-000162-INFRA //SAA 2023-02-000680-INFRA// SAA 2023-02-000680-INFRA
            {
                if (listUacs.AllotmentClassId == 3 && listUacs.AppropriationId == 1 && listUacs.BudgetAllotmentId == 3 && listUacs.prexcId == 9)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, listUacs.Suballotment_title);

                    currentRow++;
                    foreach (var uacs in listUacs.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;  
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }//end of 441


            }//end of foreach   
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO1);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

        
            foreach (var fundsorce in funsources1) // Local Health Systems Development and Assistance  // 310201100003000 // 2023 LHSDA
            {

                if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 10)
                {
                    if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalLocalHealth);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                    if(PS == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaaFund);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                        paptitle = fundsorce.Prexc.pap_title;
                        papcode = fundsorce.Prexc.pap_code1;
                    }

                    SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);
                        duplicate1 = fundsorce.FundSourceTitle;

                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", fundsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    
                   
                    currentRow++;
                    foreach (var uacs in fundsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;
                        
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", suballot.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    
                    currentRow++;

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                 
                        currentRow++;

                        //   duplicate1 = suballot.Suballotment_title;
                    }
                }
            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa7);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
           
            if (OL0_OL1 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", OL0_OL1);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;
     
             bool saadisplayed = false; bool saadisplayed1 = false;

            foreach (var subaalot in subAllotments) //Pharmaceutical Management // 310201100004000 // SAA 2023-03-000952
            {
                if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 11)
                {
                    if (subaalot.prexc.pap_title != paptitle || subaalot.prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaaPharmaceu);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                  
                        Prexc_papTitle(worksheet, ref currentRow, subaalot.prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, subaalot.prexc.pap_code1);

                    if (!saadisplayed1)
                    {
                        if (PS1 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        saadisplayed1 = true;
                    }
                    else
                    {
                        
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS1);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    
                        ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                        saadisplayed1 = true;
                    }
                       
                    
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa8);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                      
                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                        saadisplayed = true;
                    
                        paptitle = subaalot.prexc.pap_title;
                        papcode = subaalot.prexc.pap_code1;
                 }


                    SubAllotTitleRed(worksheet, ref currentRow, subaalot.Suballotment_title);
                    currentRow++;
                   
                    foreach (var uacs in subaalot.SubAllotmentAmounts.ToList())
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;

                    }


                } // year
            } //end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa8);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        if(CO1 == 0) 
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO1);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            foreach (var funsource in funsources1) // 310202100002000 // 2023 HRHICM 
            {
                if (funsource.AllotmentClassId == 2 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 13)
                {
                    var valueAdd = "HEALTH HUMAN RESOURCE SUB - PROGRAM";
                    if (uniqueValues.Add(valueAdd))
                 
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        var rangeAtoU103 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                        rangeAtoU103.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
                        rangeAtoU103.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", total_Health_HumanSub);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right); //adding 1

                        currentRow++;
                    }
                    if (funsource.Prexc.pap_title != paptitle || funsource.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrexTitle_code); Console.WriteLine();
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);
                    
                    if(totalPS == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPS);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", MOOE1);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);
                        paptitle = funsource.Prexc.pap_title;
                        papcode = funsource.Prexc.pap_code1;
                    }
                    
                        SubAllotTitleRed(worksheet, ref currentRow, funsource.FundSourceTitle);
                        duplicate1 = funsource.FundSourceTitle;

                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsource.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    

                    foreach (var uacs in funsource.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }



                } //year and prexc
            } // end of foreach

            foreach (var suballot1 in subAllotments) // 310202100002000 // 2023 HRHICM continue::
            {
                if (suballot1.AllotmentClassId == 2 && suballot1.AppropriationId == 1 && suballot1.BudgetAllotmentId == 3 && suballot1.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot1.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot1.SubAllotmentAmounts.ToList())
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;

                    }

                }

            }//end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", Saa_SAA_000199);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if(totalCO == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;
          
          
            // decimal totalSaaFunsorce1 = totalfunsorce + totalsaa5;
            foreach (var fundsorce in funsources1) //National Health Workforce Support System (NHWSS) // 310202100003000 //2023 NHWSS-PS
            {

                if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 32)
                {
                 
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", NHWSSGRandTotal);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    
                   
                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS_1PS2);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);

                    if (fundsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);
                        duplicate1 = fundsorce.FundSourceTitle;

                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", fundsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                    foreach (var uacs in fundsorce.FundSourceAmounts)
                    {

                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            } //end of foreach
            foreach (var suballot in subAllotments) // part -SAA 2023-03-000988 2023 NHWSS-PS
            {
                if (suballot.AllotmentClassId == 1 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 32)
                {

                 
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;
                        currentRow++;
                    
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;

                    }

                }
            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            if(totalsaa5 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalsaa5);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            currentRow++;
             
          
           // decimal totalSaaFunsorce2 = totalFunsorce + totalSaa9;
            foreach (var funsoce in funsources1) //2023 NHWSS SAA 2023-03-000897
            {
                if (funsoce.AllotmentClassId == 2 && funsoce.AppropriationId == 1 && funsoce.BudgetAllotmentId == 3 && funsoce.PrexcId == 32)
                {
                    if(MOOE_totalSaa9 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", MOOE_totalSaa9); // adding 3
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                    }
                   
                    ItemSubPrexc(worksheet, ref currentRow, funsoce.AllotmentClass.Desc);

                    if (funsoce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsoce.FundSourceTitle);
                        duplicate1 = funsoce.FundSourceTitle;

                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}",  funsoce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                    foreach (var uacs in funsoce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }



                }
            }//end of foreach
            currentRow++;
            string duplicate4 = null;
            string duplicate5 = null;
           
            foreach (var suballot in subAllotments) //SAA 2023-08-004078 // SAA 2023-03-000897 
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 32)
                {
                    if (suballot.Suballotment_title != duplicate5)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate5 = suballot.Suballotment_title;
                        currentRow++;
                    }
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            }//end of foreach 
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa9);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            currentRow++;
            if(COE_1COE_2 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", COE_1COE_2);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capitaln Outlays");

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Health PromotionSUB - PROGRAM";
            var rangeAtoU104 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU104.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU104.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa3_totalfunsorce1_PS_CO);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        
           
            currentRow++;
            foreach (var funsorce in funsources1)//2023 HEALTH PROMOTION 310203100001000
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 14)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa3_totalfunsorce1_PS_CO);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    
                    
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                   if(totalPS3 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}","-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPS3);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    }
                   
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                   if (MOOE == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", MOOE);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }
                    if (funsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        duplicate1 = funsorce.FundSourceTitle;
                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalfunsorce1);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                } //prexcId

            }//end foreach
           
            //totalSaa3
            foreach (var suballot in subAllotments) //2023 HEALTH PROMOTION 310203100001000 contenue:: SAA 2023-03-001447
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 14)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }

            }//end of foreach
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa3);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
         if(totalCO3 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO3  );
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            var PS15 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            var CO14 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            var MOOE3 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            decimal totalprexc_PS_CO_MOOE = PS15 + MOOE3 + CO14;  //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM

            var PS17 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 37).Sum(x => x.Beginning_balance);
            var OL3 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 37).Sum(x => x.Beginning_balance);

            var totalFuncorce = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 37).Sum(x => x.Beginning_balance);
            decimal total_prevention = PS17 + OL3 + totalFuncorce; //Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD

            var CO11 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 18).Sum(x => x.Beginning_balance);
            var PS16 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 18).Sum(x => x.Beginning_balance);
            var totalSaa12 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 18).Sum(x => x.Beginning_balance);
            Decimal totalPrex_co_ps = CO11 + PS16 + totalSaa12; //Family Health, Nutrition and Responsible Parenting // 2023 FHINRP

            var PS19 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 16).Sum(x => x.Beginning_balance);
            var totalSaa11 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 16).Sum(x => x.Beginning_balance);
            var CO13 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 16).Sum(x => x.Beginning_balance);
            decimal totalPS19_CO123 = PS19 + CO13 + totalSaa11;  //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044

            var PS4A = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS
            var MOOE4B = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS
            var CO1A = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS

            var PS2A = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179
            var SaaMOOE5 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179
            var CO_1 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179

           
            decimal SaaMOO5_MOOE4 = SaaMOOE5 + MOOE4B;
            decimal totalPSA = PS4A + PS2A;
            decimal totalCO31 = CO_1 + CO1A;
            decimal Public_Health_management = totalPSA + SaaMOO5_MOOE4 + totalCO31;  //Public Health Management // 310301100001000 // 2023 PHM-PS

            var PS20 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 40).Sum(x => x.Beginning_balance);
            var CO12 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 40).Sum(x => x.Beginning_balance);
            var totalSaa0 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 40).Sum(x => x.Beginning_balance);
            decimal totalCO12_PS20 = totalSaa0 + CO12 + PS20;


            decimal total_PUBLIC_HEALTHPROGRAM = totalCO12_PS20 + totalprexc_PS_CO_MOOE + total_prevention + totalPS19_CO123 + totalPrex_co_ps + Public_Health_management;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH PROGRAM";
            var rangeAtoU = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", total_PUBLIC_HEALTHPROGRAM);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Locally-Funded Project(s)";
            var rangeAtoU1 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU1.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU1.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO12_PS20);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
           
            bool totalSaaDisplayed = false;
            foreach (var suballot in subAllotments) //Public Health Emergency Benefits and Allowances // 310300200003000 // SAA 2023-07-003434
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 40)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO12_PS20);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);



                        if (PS20 == 0)
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = "-";
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                        else
                        { 
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS20);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        }

                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa0);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                        paptitle = suballot.prexc.pap_title;
                        papcode = suballot.prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                   
                     currentRow++;
                    
                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa0);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if(CO12 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = "-";
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO12);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            currentRow++;

            HashSet<string> uniqueAccountTitles5 = new HashSet<string>();   HashSet<string> uniqueExpenseCodes5 = new HashSet<string>();
           

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH MANAGEMENT SUB - PROGRAM";
            var rangeAtoU2 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU2.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", Public_Health_management);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            foreach (var funsorce in funsources1) //Public Health Management // 310301100001000 // 2023 PHM-PS
            {
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 15)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", Public_Health_management);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    var valueAdd = "GAA 2023";
                    if (uniqueValues.Add(valueAdd))
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 1).Value = valueAdd;
                        currentRow++;
                    }

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPSA);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                  
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }
                    if (funsorce.FundSourceTitle != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        duplicate1 = funsorce.FundSourceTitle;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                        currentRow++;
                    }

                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            } // end of foreach
     
           
            foreach (var funsorce in funsources1) //2023 PHM-HSDS //2023 PHM-HFDS
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 15)
                {
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", SaaMOO5_MOOE4);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                 
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;   
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}" ,uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }
                }
            }// end of foreach
            currentRow++;
            foreach (var suballot in subAllotments) //SAA 2023-03-001503 //SAA 2023-04-002179
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 15)
                {
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }

                   
                }      
                
               
            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", SaaMOOE5);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if (totalCO31 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                worksheet.Cell(currentRow, 3).Value = "-";
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalCO31);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            currentRow++;
           

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "ENVIRONMENTAL AND OCCUPATIONAL HEALTH SUB - PROGRAM";
            var rangeAtoU3 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU3.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPS19_CO123);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            foreach (var suballot in subAllotments) //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044
            {// console Writeline()
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 16)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPS19_CO123);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    if(PS19 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = "-";
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS19);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services ");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa11);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                    paptitle = suballot.prexc.pap_title;
                    papcode = suballot.prexc.pap_code1;
                }
                if (suballot.Suballotment_title != duplicate1)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                        duplicate1 = suballot.Suballotment_title;   
                        currentRow++;
                    }

                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(X => X.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                    TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa11);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;
                    if(CO13 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = "-";
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO13);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                }
            }//end of foreach

            currentRow++;
            

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "FAMILY HEALTH SUB - PROGRAM";
            var rangeAtoU4 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU4.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrex_co_ps);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
        
            foreach (var funsorce in funsources1) //Family Health, Nutrition and Responsible Parenting // 2023 FHINRP
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 18)
                {

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrex_co_ps);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    if(PS16 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS16);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSaa12);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                    if (CO11 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO11);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                }
            }//end of foreaach
            currentRow++;
        

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU5 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU5.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", total_prevention);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

           
            foreach (var funsorce in funsources1) //Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 37)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", total_prevention);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    
                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    if(PS17 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS17);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
    
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services ");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFuncorce);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                 }
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;

                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                   
                    if (OL3 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", OL3);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }

                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                }
            }//end of foreaach
            currentRow++;

           
            
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU24 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU24.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU24.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            foreach (var funsorce in funsources1) //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 23)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                       

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalprexc_PS_CO_MOOE);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    if (PS15 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS15);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", MOOE3);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                    if (CO14 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", CO14);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }

                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                    currentRow++;
                }
            }//foreach          
            currentRow++;


            var PS13 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            var PS14 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            var totalFunsorce1 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Epidemiology and Surveillance PROGRAM";
            var rangeAtoU6 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU6.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU6.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce1);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            decimal TotalPs1 = PS13 + PS14;

            foreach (var funsorce in funsources1)//2023 ES // Epidemiology and Surveillance // 310400100001000
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 24)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce1);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1); 

                    if(TotalPs1 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", TotalPs1);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                       ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                      
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce1);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts) // SAA 2023-05-002387
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }

            } //end of foreach

            var totalSaa13 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 24).Sum(x => x.Beginning_balance);
            var CO = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 24).Sum(x => x.Beginning_balance);
            var CO10  = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            decimal TotalCO = CO + CO10;
            foreach (var subaalot in subAllotments) //2023 HEPR // Health Emergency Preparedness and Response // 310500100001000
            {
                if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 24)
                {
                    Prexc_papTitle(worksheet, ref currentRow, subaalot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, subaalot.prexc.pap_code1);

                    ItemSubPrexc(worksheet, ref currentRow, subaalot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, subaalot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in subaalot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa13);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if (TotalCO == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
               
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", TotalCO);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            var PS12 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            var OL2 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            var totalFunsorce3 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            decimal prexcPs12Ol2 = totalFunsorce3 + PS12 + OL2;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH EMERGENCY MANAGEMENT PROGRAM";
            var rangeAtoU7 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU7.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU7.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce3);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
           
            foreach (var funsorce in funsources1) //HEALTH EMERGENCY MANAGEMENT PROGRAM 2023 HEPR
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 25)
                {
                    if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                    {
                        
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", prexcPs12Ol2);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                    
                    if(PS12 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS12);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }

                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services ");
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce3);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                        paptitle = funsorce.Prexc.pap_title;
                        papcode = funsorce.Prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                    if (OL2 == 0)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", OL2);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    }
                    ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                }

            }//end of foreach
            currentRow++;
            var PS23 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 26).Sum(x => x.Beginning_balance);
            var CO17 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 26).Sum(x => x.Beginning_balance);
            var MOOE7 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 26).Sum(x => x.Beginning_balance);
            // Quick Response Fund 310500100002000 ::
            var PS22 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 26).Sum(x => x.Beginning_balance);
            var CO18 = funsources1.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 26).Sum(x => x.Beginning_balance);
            var MOOE6 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 26).Sum(x => x.Beginning_balance);
            decimal PS23_PS22 = PS23 + PS22;
            decimal CO17_CO18 = CO17 + CO18;
            decimal MOOE6_MOOE7 = MOOE6 + MOOE7; decimal prexcTotal1 = MOOE6_MOOE7 + CO17_CO18 + PS23_PS22;
            foreach (var fundsorce in funsources1)// Quick Response Fund 310500100002000
            {
                if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 26)
                {
                    if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                    {

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", prexcTotal1);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                        if (PS23_PS22 == 0)
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", "-");
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", PS23_PS22);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }

                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services ");
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", MOOE6_MOOE7);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                        paptitle = fundsorce.Prexc.pap_title;
                        papcode = fundsorce.Prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", fundsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in fundsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            }//end og foreach
            
            foreach (var suballot in subAllotments)// Quick Response Fund 310500100002000 ::
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 26)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts)
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
             }//end of foreach
            if(MOOE7 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", MOOE7);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
           
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            currentRow++;
            if (CO17_CO18 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", CO17_CO18);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            currentRow++;

            var totalFunsource4 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 27).Sum(x => x.Beginning_balance);
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to curative and rehabilitative health care services improved";
            var rangeAtoU17A13 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17A13.Style.Fill.BackgroundColor = XLColor.FromHtml("C8E0F4");
            rangeAtoU17A13.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
                
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES OPERATION PROGRAM";
            var rangeAtoU8 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU8.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU8.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "CURATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU9 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU9.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU9.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsource4);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
          
            foreach (var funsorce in funsources1) // 2023 OBCNVBSP // 320101100001000
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 27)
                {
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsource4);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsource4);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }


                }
            }//end of foreach
            currentRow++;
            var totalSaa14 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 27).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //SAA 2023-04-002252
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 27)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa14);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;
            var totalSaa15 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 34).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //SAA 2023-03-000842 //Operation of National Reference Laboratories// 320101100004000
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 34)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa15);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa15);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "REHABILITATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU10 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU10.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU10.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            var totalFunsorce5 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 28).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) // SAA 2023-03-001120
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 28)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce5);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }
                }
            }//end of foreach 
           
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalFunsorce5);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            rangeAtoU11.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU11.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES AND SERVICES REGULATION SUB - PROGRAM";
            var rangeAtoU12 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU12.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU12.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            var totalFunsorce4 = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 29).Sum(x => x.Beginning_balance);
            foreach (var funsorce in funsources1) // 2023 RRHFS-PS // 330101100002000 // Regulation of Regional Health Facilities and Services
            {
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 29)
                {

                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalFunsorce4);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            }//end of foreach
            var totalFunsorce6 = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 29).Sum(x => x.Beginning_balance);
            foreach (var funsorce in funsources1) //2023 RRHFS
            {
                if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 29)
                {


                    Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalFunsorce6);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    currentRow++;
                    foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

             }//end of foreach
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "OO : Access to social health protection assured";
            var rangeAtoU105 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU105.Style.Fill.BackgroundColor = XLColor.FromHtml("#13ecec"); //skyblue
            rangeAtoU105.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "SOCIAL HEALTH PROTECTION PROGRAM";
            var rangeAtoU13 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU13.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU13.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            var PS24 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 30).Sum(x => x.Beginning_balance);
            var CO19 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 30).Sum(x => x.Beginning_balance);
            var totalSaa16 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 30).Sum(x => x.Beginning_balance);
            decimal totalPrexc = totalSaa16 + CO19 + PS24;
            foreach (var suballot in subAllotments)//Assistance to Indigent Patients either Confined// 340100100001000  // SAA 2023-03-000889
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 30)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalPrexc);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                      if(PS24 == 0)
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", PS24);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        }
                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                    
              
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa16);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                        paptitle = suballot.prexc.pap_title;
                        papcode = suballot.prexc.pap_code1;
                    }

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }

            }// end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa16);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if (CO19 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", CO19);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
           

            var PS25 = subAllotments.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 17).Sum(x => x.Beginning_balance);
            var CO20 = subAllotments.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 17).Sum(x => x.Beginning_balance);
            var totalSaa17 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 17).Sum(x => x.Beginning_balance);
            decimal totalPrexc1 = totalSaa17 + CO20 + PS25;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Locally-Funded Project(s)";
            var rangeAtoU14 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU14.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU14.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalPrexc1);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
          
            foreach (var suballot in subAllotments) //National Immunization 310303100001000 // SAA 2023-02-000497
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 17)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalPrexc1);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                  if(PS25 == 0)
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", PS25);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        }
                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services ");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa17);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                        paptitle = suballot.prexc.pap_title;
                        papcode = suballot.prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa17);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            if(CO20 == 0)
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", "-");
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else
            {
                worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", CO20);
                worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
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

            foreach(var uacs in _MyDbContext.Uacs.ToList())
            {
                 if(uacs.UacsId == 24)
                {
                    var rangeAtoU31 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                    rangeAtoU31.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCC3FF"); //pink
                    rangeAtoU31.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                    worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                    worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 2).Style.Font.FontName = "Arial Narrow";
                    worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                    currentRow++;
                }
                
            }
            currentRow++;
            foreach (var funsorce in funsources1)
            {
                //FOR Retirement and Life Insurance Premium
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 4)
                {
                    if (funsorce.FundSourceId == 60)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                        foreach (var uacs in funsorce.FundSourceAmounts.ToList())
                        {
                            var rangeAtoU31 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
                            rangeAtoU31.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCC3FF"); //pink
                            rangeAtoU31.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 2).Style.Font.FontName = "Arial Narrow";
                            worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", uacs.beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }
                    }
                }
                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 15)
                {
                    if (funsorce.FundSourceId == 62)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
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

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                        }
                    }
                }


                if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 29)
                {
                    if (funsorce.FundSourceId == 61)
                    {
                        SubAllotTitleRed(worksheet, ref currentRow, funsorce.FundSourceTitle);
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Font.SetBold();
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
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

                            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", funsorce.Beginning_balance);
                            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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
            bool totalSaa18Displyed = false;
            var totalSaa18 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 1).Sum(x => x.Beginning_balance);
            foreach (var conapsub in subAllotments)  //CONAP SAA 2022-11-5344 //100000100001000
            {
                if (conapsub.AllotmentClassId == 2 && conapsub.AppropriationId == 1 && conapsub.BudgetAllotmentId == 1 && conapsub.prexcId == 1)
                {
                    if (conapsub.prexc.pap_title != paptitle || conapsub.prexc.pap_code1 != papcode)
                    {

                        Prexc_papTitle(worksheet, ref currentRow, conapsub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, conapsub.prexc.pap_code1);
                   
                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa18);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                        paptitle = conapsub.prexc.pap_title;
                        papcode = conapsub.prexc.pap_code1;
                    }

                    SubAllotTitleRed(worksheet, ref currentRow, conapsub.Suballotment_title);
                    currentRow++;
                    foreach (var uacs in conapsub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalSaa18);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "II. SUPPORT TO OPERATIONS";
            currentRow++;

            //Health Information Technolog / 200000100001000
            //CONAP SAA 2022-10-4920
            var totalconapSaa = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 35).Sum(x => x.Beginning_balance);
            foreach (var conapSub in subAllotments) //Procurement and Supply Chain Management Service // CONAP SAA 2022-03-1500
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 1 && conapSub.BudgetAllotmentId == 1 && conapSub.prexcId == 35)
                {
                    if (conapSub.prexc.pap_title != paptitle || conapSub.prexc.pap_code1 != papcode)
                    {
                        Prexc_papTitle(worksheet, ref currentRow, conapSub.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, conapSub.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, conapSub.AllotmentClass.Desc);
                        paptitle = conapSub.prexc.pap_title;
                        papcode = conapSub.prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", conapSub.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            }//end of foreach 

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapSaa1 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 35).Sum(x => x.Beginning_balance);
            foreach (var conapSub in subAllotments) //CONAP SAA 23-05-00000704 // CONAP SAA 23-02-00000323
            {
                if (conapSub.AllotmentClassId == 2 && conapSub.AppropriationId == 2 && conapSub.BudgetAllotmentId == 3 && conapSub.prexcId == 35)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, conapSub.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", conapSub.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in conapSub.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }


            }// end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa1);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
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
            var rangeAtoU17A12 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU17A12.Style.Fill.BackgroundColor = XLColor.FromHtml("C8E0F4");
            rangeAtoU17A12.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
            var rangeAtoU27 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU27.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU27.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Arial Narrow";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "SERVICE DELIVERY SUB - PROGRAM";
            currentRow++;
            foreach (var prexcData in subAllotments)// Health Facilities Enhancement Program  // 310201100004000
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
            var totalconapSaa2 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 11).Sum(x => x.Beginning_balance);


            foreach (var suballot in subAllotments) //Pharmaceutical Management //  310201100004000
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 11)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {
                        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa2);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                        paptitle = suballot.prexc.pap_title;
                        papcode = suballot.prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize =10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa2);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH HUMAN RESOURCE SUB - PROGRAM";
            var rangeAtoU29 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU29.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU29.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapSaa3 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments)//310202100002000  // HEALTH HUMAN RESOURCE SUB - PROGRAM
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 13)
                {
                    if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                    {

                        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa3);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                        paptitle = suballot.prexc.pap_title;
                        papcode = suballot.prexc.pap_code1;
                    }
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }//end of if statement

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa3);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapSaa4 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 23-02-00000429  CONAP SAA 23-06-00000824
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa4);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            var totalconapSaa5 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 32).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) 
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 32)
                {
                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa5);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            }//end of foreach

            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa5);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            var totalconapSaa6 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 14).Sum(x => x.Beginning_balance);

            foreach (var SubAllot in subAllotments) //CONAP SAA 2022-05-2210
            {
                if (SubAllot.AllotmentClassId == 2 && SubAllot.AppropriationId == 1 && SubAllot.BudgetAllotmentId == 1 && SubAllot.prexcId == 14)
                {
                    Prexc_papTitle(worksheet, ref currentRow, SubAllot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, SubAllot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa6);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, SubAllot.AllotmentClass.Desc);
                    // No Data CONAP 2022 HEALTH PROMOTION
                    SubAllotTitleRed(worksheet, ref currentRow, SubAllot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", SubAllot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in SubAllot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa6);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            //Public Health Emergency Benefits and Allowances for Health Care - 310300200003000 , CONAP SAA 2022-02-0791

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH PROGRAM";
            var rangeAtoU106 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU106.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU106.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Locally-Funded Project(s)";
            var rangeAtoU107 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU107.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU107.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapSaa7 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 40).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //CONAP SAA 23-03-00000472
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 40)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId).Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa7);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            var totalconapSaa8 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 38).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //CONAP SAA 2022-05-2462 // COVID-19 Laboratory Network Commodities
            {

                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 38)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa8);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }


            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa8);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            currentRow++;
            currentRow++;
            var totalconapSaa9 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 38).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) // CONAP SAA 23-02-00000291 // CONAP SAA 23-01-00000202
            {


                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 38)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }
                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa9);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapSaa10 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 31).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) // CONAP SAA 2022-12-6483 // COVID-19 Human Resources for Health Emergency Hiring
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 31)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa10);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa10);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapSaa12 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //CONAP SAA 23-02-00000429 // CONAP SAA 23-01-00000008
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 13)
                {
                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;


                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }
            }//end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa12);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            var totalconapSaa13 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 18).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 2022-06-3144
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 18)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa13);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa13);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU33 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU33.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU33.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            var totalconapSaa14 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 37).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 2022-06-3080
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 37)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa14);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa14);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapSaa15 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 37).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) // CONAP SAA 23-03-00000509
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 37)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa15);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Epidemiology and SurveillancePROGRAM";
            var rangeAtoU36 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU36.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU36.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapSaa16 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 24).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 23-03-00000538 // Epidemiology and Surveillance
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 24)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa16);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            //SAA # 2023 - No data in database
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa16);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            //CONAP SAA 2022-05-2673 - No data in database

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH EMERGENCY MANAGEMENT PROGRAM";
            var rangeAtoU34 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU34.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU34.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            var totalconapSaa17 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 24).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 2022-02-0941 // Health Emergency Preparedness and Response
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 25)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa17);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapSaa17);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;
            var totalconapsaa18 = subAllotments.Where( x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 26).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //CONAP SAA 2022-03-1405 // Operations of Blood Centers and National Voluntary Blood Services Program
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 26)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa18);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            rangeAtoU836.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU836.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "CURATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU37 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU37.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU37.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapsaa19 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 27).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments)
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 27)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa19);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa19);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            var totalconapsaa20 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 34).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) // CONAP SAA 2022-05-2730 // CONAP SAA 2022-03-0974
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 34)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa20);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa20);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "REHABILITATIVE HEALTH CARE SUB - PROGRAM";
            var rangeAtoU38 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU38.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU38.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapsaa21 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 28).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //CONAP SAA 2022-05-2178 // 320102100001000
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 28)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}",totalconapsaa21);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa21);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            rangeAtoU39.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU39.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH FACILITIES AND SERVICES REGULATION SUB - PROGRAM";
            var rangeAtoU40 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU40.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU40.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            var totalconapsaa22 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 41).Sum(x => x.Beginning_balance);
            foreach (var suballot in subAllotments) //CONAP SAA 2022-04-1874 // 330101100001000
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 41)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa22);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa22);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
            rangeAtoUE.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoUE.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;


            var totalconapsaa23 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 30).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) // 340100100001000 // CONAP SAA 2022-12-6899 // CONAP SAA 2022-12-6556
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 1 && suballot.BudgetAllotmentId == 1 && suballot.prexcId == 30)
                {

                    Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                    Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa23);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2022");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa23);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            var totalconapsaa24 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == 3 && x.prexcId == 30).Sum(x => x.Beginning_balance);

            foreach (var suballot in subAllotments) //  CONAP SAA 23-02-00000348 //  CONAP SAA 23-02-00000230
            {
                if (suballot.AllotmentClassId == 2 && suballot.AppropriationId == 2 && suballot.BudgetAllotmentId == 3 && suballot.prexcId == 30)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, suballot.Suballotment_title);
                    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", suballot.Beginning_balance);
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.Red;
                    currentRow++;
                    foreach (var uacs in suballot.SubAllotmentAmounts.ToList())
                    {
                        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Account_title;

                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == uacs.UacsId)?.Expense_code;

                        worksheet.Cell(currentRow, 3).Style.Font.FontSize = 9;
                        worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", uacs.beginning_balance);
                        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                }

            } //end of foreach
            TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa24);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
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
