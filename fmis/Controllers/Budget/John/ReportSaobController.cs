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
                            .OrderBy(x => x.AllotmentClass.Id)
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

            var subAllotments = _MyDbContext.SubAllotment
                .Include(X => X.AllotmentClass)
                .Include(x => x.SubAllotmentAmounts)
                     .ThenInclude(x => x.Uacs)
                .Include(x => x.Obligations.Where(o => o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday))
                .Include(x => x.prexc)
                .OrderBy(x => x.prexc.pap_title)
                .ThenByDescending(x => x.Suballotment_title)
                .OrderBy(x => x.AllotmentClass.Id)
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


            // var allotmentClassIdsToDisplay = new List<int> { 1, 2, 3 };
            var AllotmentClassId = _MyDbContext.AllotmentClass
                .Select(x => x.Id)
                .ToList();

            var budgetallotmentId = _MyDbContext.Budget_allotments
                                    .Select(x => x.BudgetAllotmentId)
                                    .ToList();
            var appropiationId = _MyDbContext.Appropriation
                                .Select(x => x.AppropriationId)
                                .ToList();

            var SaaPS2 = subAllotments.Where(prex => prex.AllotmentClassId == 1 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);
            var SaaMOOE1 = subAllotments.Where(prex => prex.AllotmentClassId == 2 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);
            var SaaCO1 = subAllotments.Where(prex => prex.AllotmentClassId == 3 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 1).Sum(prex => prex.Beginning_balance);

            var FundsourcePS1 = funsources1.Where(prex => prex.AllotmentClassId == 1 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);
            var FundsourceMOOE1 = funsources1.Where(prex => prex.AllotmentClassId == 2 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);
            var FundsourceCO1 = funsources1.Where(prex => prex.AllotmentClassId == 3 && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.PrexcId == 1).Sum(prex => prex.Beginning_balance);

            decimal totalPSGeneral = SaaPS2 + FundsourcePS1;
            decimal totalMOOEGeneral = SaaMOOE1 + FundsourceMOOE1;
            decimal totalCOGeneral = SaaCO1 + FundsourceCO1;
            decimal totalPrexcGeneral = totalPSGeneral + totalMOOEGeneral + totalCOGeneral;

            if (funsources1.Any(x => (x.AllotmentClassId == 2 || x.AllotmentClassId == 1 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 1)) //General Management and Supervision :: for Funsorce
            {

                foreach (var allotmentClass in AllotmentClassId)
                {
                    var funsorceAllotClass = funsources1.Where(f => f.AllotmentClassId == allotmentClass && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 1).ToList();
                    if (funsorceAllotClass.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllotClass)
                        {

                            if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                            {

                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexcGeneral));
                                Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                paptitle = fundsorce.Prexc.pap_title;
                                papcode = fundsorce.Prexc.pap_code1;
                            }
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
                    }
                }//end of foreach
            }//end of if Statement to check if there is data
            else
            {
                foreach (var prex in subAllotments)
                {
                    if ((prex.AllotmentClassId == 2 || prex.AllotmentClassId == 1 || prex.AllotmentClassId == 3) && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 1)
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
                                if (prex.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }

                            }


                            if (totalMOOEGeneral == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOEGeneral));
                                if (prex.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                                }

                            }

                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }
                    }
                }
            } // end of else statement
            foreach (var prex in subAllotments)//General Management and Supervision //100000100001000 // SAA 2023-03-001317
            {
                if ((prex.AllotmentClassId == 1 || prex.AllotmentClassId == 2 || prex.AllotmentClassId == 3) && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 1)
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


            var SaaPS1 = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 2).Sum(x => x.Beginning_balance);
            var SaaMOOE = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 2).Sum(x => x.Beginning_balance);
            var SaaCO = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 2).Sum(x => x.Beginning_balance);

            var FundsourcePS = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 2).Sum(x => x.Beginning_balance);
            var FundsourceMOOE = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 2).Sum(x => x.Beginning_balance);
            var FundsourceCO = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 2).Sum(x => x.Beginning_balance);

            decimal totalPS_Adminis = SaaPS1 + FundsourcePS;
            decimal totalMOOE_Adminis = SaaMOOE + FundsourceMOOE;
            decimal totalCO_Adminis = SaaCO + FundsourceCO;
            decimal totalPrexc_Adminis = totalPS_Adminis + totalMOOE_Adminis + SaaCO + FundsourceCO;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 2)) // check this data if naa
            {
                foreach (var allotmentclass in AllotmentClassId)//Administration of Personnel Benefits// 100000100002000  Fundsorces1
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentclass && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 2).ToList();
                    //if (funsorce.AllotmentClassId == 1 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 2)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {
                            if (funsorce.Prexc.pap_title != paptitle && funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Adminis));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                                if (totalPS_Adminis == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Adminis));
                                    ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                                }

                                if (totalMOOE_Adminis == 0)
                                {

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Adminis));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
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
                            // }
                        }
                    }//end of foreach
                }//end of foreach classAllotment
            }
            else
            {
                foreach (var prex in subAllotments) //Administration of Personnel Benefits// 100000100002000 SubAllotment
                {
                    if ((prex.AllotmentClassId == 1 || prex.AllotmentClassId == 2 || prex.AllotmentClassId == 3) && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 2)
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
                                if (prex.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                }
                            }

                            paptitle = prex.prexc.pap_title;
                            papcode = prex.prexc.pap_code1;
                        }
                    }
                }// end of foreach
            }//end of else statement
            currentRow++;
            bool staticDataAdded = false;
            foreach (var prex in subAllotments)//Administration of Personnel Benefits// 100000100002000 //SAA 2023-07-003501
            {
                if ((prex.AllotmentClassId == 1 || prex.AllotmentClassId == 2 || prex.AllotmentClassId == 3) && appropiationId.Contains(prex.AppropriationId) && budgetallotmentId.Contains(prex.BudgetAllotmentId.GetValueOrDefault()) && prex.prexcId == 2)
                {

                    SubAllotTitleRed(worksheet, ref currentRow, prex.Suballotment_title);
                    currentRow++;


                    foreach (var uacs in prex.SubAllotmentAmounts.ToList())
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
            if (totalCO_Adminis == 0)
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

            var SaaPS4 = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 4).Sum(x => x.Beginning_balance);
            var SaaMOOE3 = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 4).Sum(x => x.Beginning_balance);
            var SaaCO3 = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 4).Sum(x => x.Beginning_balance);

            var FundsourcePS3 = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var FundsourceMOOE3 = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var FundsourceCO3 = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 4).Sum(x => x.Beginning_balance);

            decimal totalPS_Operation = SaaPS4 + FundsourcePS3;
            decimal totalMOOE_Operation = SaaMOOE3 + FundsourceMOOE3;
            decimal totalCO_Operation = SaaCO3 + FundsourceCO3;
            decimal totalPrexc_Operation = totalPS_Operation + totalMOOE_Operation + totalCO_Operation;

            var STO_Oro_Beginning_balanced = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            var Sto_Oro_Ps_BeginningBalance = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 4).Sum(x => x.Beginning_balance);
            decimal totalSumSto_Oro = Sto_Oro_Ps_BeginningBalance + STO_Oro_Beginning_balanced; //Operations of Regional Offices 200000100002000 //2023 STO-ORO-PS

            bool sto_oro_displayed = false;

            var SaaPS3 = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 3).Sum(x => x.Beginning_balance);
            var SaaMOOE2 = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 3).Sum(x => x.Beginning_balance);
            var SaaCO2 = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 3).Sum(x => x.Beginning_balance);

            var FundsourcePS2 = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 3).Sum(x => x.Beginning_balance);
            var FundsourceMOOE2 = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 3).Sum(x => x.Beginning_balance);
            var FundsourceCO2 = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 3).Sum(x => x.Beginning_balance);

            decimal totalPS_Health = SaaPS3 + FundsourcePS2;
            decimal totalMOOE_Health = SaaMOOE2 + FundsourceMOOE2;
            decimal totalCO_Health = SaaCO2 + FundsourceCO2;
            decimal totalPrexc_Health = totalPS_Health + totalMOOE_Health + totalCO_Health;

            if (funsources1.Any(x => (x.AllotmentClassId == 2 || x.AllotmentClassId == 1 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 3))
            {
                foreach (var allotmentClass in AllotmentClassId)// Health Information Technology 200000100001000
                {
                    //if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 3)
                    //{
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentClass && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 3).ToList();
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {
                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {

                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);


                                if (totalPS_Health == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health));
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                                if (totalMOOE_Health == 0)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                                    }
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

                            //}
                        }

                    }

                }//end of foreach
            }
            else
            {
                foreach (var suballot in subAllotments)
                {
                    if ((suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 3)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {

                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health));
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                            }
                            if (totalMOOE_Health == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }
            }// end of else statement


            foreach (var subaalot in subAllotments)// Health Information Technology 200000100001000 continue::
            {
                if ((subaalot.AllotmentClassId == 2 || subaalot.AllotmentClassId == 1 || subaalot.AllotmentClassId == 3) && appropiationId.Contains(subaalot.AppropriationId) && budgetallotmentId.Contains(subaalot.BudgetAllotmentId.GetValueOrDefault()) && subaalot.prexcId == 3)
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
            if (SaaMOOE2 == 0)
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



            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 4))
            {
                foreach (var allotmentClassId in AllotmentClassId) //Operations of Regional Offices 2023 STO-ORO-PS
                {
                    var funsourcesForAllotmentClass = funsources1
                        .Where(f => f.AllotmentClassId == allotmentClassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 4)
                        .ToList();

                    if (funsourcesForAllotmentClass.Count > 0)
                    {

                        foreach (var funsource in funsourcesForAllotmentClass)
                        {
                            // Check if AllotmentClassId is 1, 2, or 3
                            if (allotmentClassId == 1 || allotmentClassId == 2 || allotmentClassId == 3)
                            {

                                if (funsource.Prexc.pap_title != paptitle || funsource.Prexc.pap_code1 != papcode)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Operation));
                                    Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                                    Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);

                                    paptitle = funsource.Prexc.pap_title;
                                    papcode = funsource.Prexc.pap_code1;
                                }

                                if (totalPS_Operation == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Operation));
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }

                                if (totalMOOE_Operation == 0)
                                {

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Operation));
                                    if (allotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                }

                                // Display FundSourceTitle
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
                    } // end of if statement
                } // end of foreach

            }//End of if statment check if any
            else
            {
                foreach (var suballot in subAllotments) // Operations of Regional Offices
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 4)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {

                            if (totalPS_Operation == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }

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
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                }
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }//end of foreach
            } // end of else

            foreach (var suballot in subAllotments)//Operations of Regional Offices 200000100002000 //suballotments
            {
                if ((suballot.AllotmentClassId == 1 && suballot.AllotmentClassId == 2 && suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 4)
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

            if (totalCO_Operation == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Operation));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            }
            string duplicate1 = null;
            string duplicate2 = null;
            string duplicate3 = null;

            var SaaPS_Chain = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 35).Sum(x => x.Beginning_balance);
            var SaaMOOE_Chain = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 35).Sum(x => x.Beginning_balance);
            var SaaCO_Chain = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 35).Sum(x => x.Beginning_balance);

            var funsourcePS_Chain = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 35).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Chain = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 35).Sum(x => x.Beginning_balance);
            var funsorceCO_Chain = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 35).Sum(x => x.Beginning_balance);
            decimal totalPS_Chain = SaaPS_Chain + funsourcePS_Chain;
            decimal totalMOOE_Chain = funsorceMOOE_Chain + SaaMOOE_Chain;
            decimal totalCO_Chain = SaaCO_Chain + funsorceCO_Chain;
            decimal totalPrexc_Chain = totalPS_Chain + totalMOOE_Chain + totalCO_Chain;
            currentRow++;
            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 35))
            {
                foreach (var allotmentclassId in AllotmentClassId) //Procurement and Supply Chain Management Service
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentclassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 35).ToList();
                    // if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 35)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot) //Procurement and Supply Chain Management Service
                        {

                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Chain));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);


                                if (totalPS_Chain == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");

                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Chain));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }

                                if (totalMOOE_Chain == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");

                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Chain));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
                                }

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

                            // }
                        }
                    }
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //Procurement and Supply Chain Management Service ::
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 35)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Chain));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                           
                                if (totalPS_Chain == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");

                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                     SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Chain));
                                     if (suballot.AllotmentClassId == 1)
                                     {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                     }
                               }
                           
                                if (totalMOOE_Chain == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");

                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Chain));
                                   if (suballot.AllotmentClassId == 2)
                                    {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
                                }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }

                }

            }//end of else


            foreach (var suballot in subAllotments)//Procurement and Supply Chain Management Service ::
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 35)
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

                   
                }//end of if statement

            }//end of foreach
            if (SaaMOOE_Chain == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Chain));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }

            currentRow++;

            if (totalCO_Chain == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Chain));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            }
            currentRow++;


            TOTALSaa(worksheet, ref currentRow, "TOTAL, STO");
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalSumSto_Oro);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;
            currentRow++;

            var SaaPS_HSRD = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 7).Sum(x => x.Beginning_balance);
            var SaaMOOE_HSRD = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 7).Sum(x => x.Beginning_balance);
            var SaaCO_HSRD = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 7).Sum(x => x.Beginning_balance);

            var funsourcePS_HSRD = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            var funsorceMOOE_HSRD = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            var funsorceCO_HSRD = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 7).Sum(x => x.Beginning_balance);
            decimal totalPS_HSRD = SaaPS_HSRD + funsourcePS_HSRD;
            decimal totalMOOE_HSRD = funsorceMOOE_HSRD + SaaMOOE_HSRD;
            decimal totalCO_HSRD = SaaCO_HSRD + funsorceCO_HSRD;

            decimal totalprexc_HSRD = totalPS_HSRD + totalMOOE_HSRD + totalCO_HSRD; //Health Sector Research Development Health Sector Research Development //    2023 HSRD




            string suballotUacs1 = null;
            string suballotUacs2 = null;
            var SaaPS_health = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 6).Sum(x => x.Beginning_balance);
            var SaaMOOE_health = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 6).Sum(x => x.Beginning_balance);
            var SaaCO_health = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 6).Sum(x => x.Beginning_balance);
            //Health Sector Policy and Plan Development  310100100001000 Continue::

            var funsorcePS_health = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 6).Sum(x => x.Remaining_balance);
            var funsorceMOOE_health = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 6).Sum(x => x.Remaining_balance);
            var funsorceCO_health = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 6).Sum(x => x.Remaining_balance);

            decimal totalPS1_heath = SaaPS_health + funsorcePS_health;
            decimal totalMOOE1_health = SaaMOOE_health + funsorceMOOE_health;
            decimal totalCO1_health = SaaCO_health + funsorceCO_health;
            decimal totalPrexc1_Health = totalPS1_heath + totalMOOE1_health + totalCO1_health; //Health Sector Policy and Plan Development  310100100001000

            var SaaPS10 = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 5).Sum(x => x.Beginning_balance);
            var SaaMOOE4 = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 5).Sum(x => x.Beginning_balance);
            var SaaCO5 = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 5).Sum(x => x.Beginning_balance);
            //International Health Policy Development and Cooperation  310100100001000 continue

            var funsorcePS10 = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 5).Sum(x => x.Remaining_balance);
            var funsorceMOOE = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 5).Sum(x => x.Remaining_balance);
            var funsorceCO3 = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 5).Sum(x => x.Remaining_balance);

            decimal totalPS_Inter = SaaPS10 + funsorcePS10;
            decimal totalMOOE_Inter = SaaMOOE4 + funsorceMOOE;
            decimal totalCO_Inter = SaaCO5 + funsorceCO3;
            decimal totalPrexc_Inter = totalPS_Inter + totalMOOE_Inter + totalCO_Inter; //International Health Policy Development and Cooperation  310100100001000

            decimal health_and_Standard_Dev = totalPrexc_Inter + totalPrexc1_Health + totalprexc_HSRD;
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
            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 5))
            {
                foreach (var allotmentClass in AllotmentClassId) //International Health Policy Development and Cooperation  310100100001000
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentClass && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 5).ToList();
                    // if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 5)
                    //  {
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {
                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Inter));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                                if (totalPS_Inter == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Inter));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }
                                if (totalMOOE_Inter == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Inter));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
                                }
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

                            // }
                        }
                    }
                }// end of foreach
            }
            else
            {
                foreach (var suballot in subAllotments)
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 5)
                    {
                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Inter));
                        Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                        Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                        if (totalPS_Inter == 0)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, "-");
                            ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                        }
                        else
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Inter));
                            if (suballot.AllotmentClassId == 1)
                            {
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                        }
                        if (totalMOOE_Inter == 0)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                            if (suballot.AllotmentClassId == 2)
                            {
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                            }
                        }
                        else
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Inter));
                            if (suballot.AllotmentClassId == 2)
                            {
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                            }
                        }

                    }
                } //end of foreach


            }//end of else


            foreach (var suballot in subAllotments)//International Health Policy Development and Cooperation  310100100001000 continue
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 5)
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

            if (SaaMOOE4 == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");

            }
            else
            {

                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE4));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");

            }

            if (totalCO_Inter == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Inter));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            }


            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 6))
            {
                foreach (var allotmentclassId in AllotmentClassId) //Health Sector Policy and Plan Development  310100100001000
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentclassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 6).ToList();
                    // if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 6)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {

                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc1_Health));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                                if (totalPS1_heath == 0)
                                {

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS1_heath));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }
                                if (totalMOOE1_health == 0)
                                {

                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE1_health));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
                                }

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

                            // }
                        }
                    }
                }
            }
            else
            {
                foreach (var suballot in subAllotments)
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 6)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc1_Health));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS1_heath == 0)
                            {

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS1_heath));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                            }
                            if (totalMOOE1_health == 0)
                            {

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE1_health));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                }
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }

                }

            }//end of else


            foreach (var suballot in subAllotments)//Health Sector Policy and Plan Development  310100100001000 Continue::
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 6)
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

                    if (SaaMOOE_health == 0)
                    {
                        SaaGaaBalance(worksheet, ref currentRow, "-");
                        TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
                    }
                    else
                    {
                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_health));
                        TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
                    }

                    currentRow++;

                    if (totalCO1_health == 0)
                    {
                        SaaGaaBalance(worksheet, ref currentRow, "-");
                        ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                    }
                    else
                    {
                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO1_health));
                        ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
                        currentRow++;
                    }
                }//end of if statement

            }

            currentRow++;
            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 7))
            {
                foreach (var allotmentClass in AllotmentClassId) //Health Sector Research Development Health Sector Research Development // 2023 HSRD
                {
                    var funsorceAllot = funsources1.Where(x => x.AllotmentClassId == allotmentClass && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 7).ToList();
                    // if (funsorce.AllotmentClassId == 2 && funsorce.AppropriationId == 1 && funsorce.BudgetAllotmentId == 3 && funsorce.PrexcId == 7)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {
                            if (funsorce.Prexc.pap_title != duplicate1 && funsorce.Prexc.pap_code1 != duplicate2 && funsorce.AllotmentClass.Desc != duplicate3)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalprexc_HSRD));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                                if (totalPS_HSRD == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_HSRD));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }
                                if (totalMOOE_HSRD == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                                    }
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_HSRD));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                                    }
                                }

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

                            // }
                        }
                    }//end of foreach 
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //Health Sector Research Development Health Sector Research Development //    2023 HSRD::
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 7)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalprexc_HSRD));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_HSRD == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_HSRD));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                            }
                            if (totalMOOE_HSRD == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_HSRD));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }

                }

            }//end of else
            currentRow++;
            foreach (var suballot in subAllotments)//Health Sector Research Development Health Sector Research Development //    2023 HSRD::
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 7)
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

            if (SaaMOOE_HSRD == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "total saa's 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:n2}", SaaMOOE_HSRD));
                TOTALSaa(worksheet, ref currentRow, "total saa's 2023");
            }
            currentRow++;
            if (totalCO_HSRD == 0)
            {

                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_HSRD));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }

            currentRow++;


            var uniqueValues = new HashSet<string>();
            var uniqueValues1 = new HashSet<string>();
            bool SaaDiplayed = false;
            bool saaDisplayed = false;

            var SaaPS_Health = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 14).Sum(x => x.Beginning_balance);
            var SaaMOOE_Health = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 14).Sum(x => x.Beginning_balance);
            var SaaCO_Health =     subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 14).Sum(x => x.Beginning_balance);

            var funsorcePS_Health = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Health = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            var funsorceCO_Health = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 14).Sum(x => x.Beginning_balance);
            
            decimal totalPS_Health1 = SaaPS_Health + funsorcePS_Health;
            decimal totalMOOE_Health1 = SaaMOOE_Health + funsorceMOOE_Health;
            decimal totalCO_Health1 = SaaCO_Health + funsorceCO_Health;
            decimal totalPrexc_Health1 = totalPS_Health1 + totalMOOE_Health1 + totalCO_Health1; ///2023 HEALTH PROMOTION 310203100001000 for Health PromotionSUB - PROGRAM


            var SaaPS_Enhanced = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 9).Sum(x => x.Beginning_balance); // SAA 2023 - 02 - 000162 - INFRA //SAA 2023-02-000680-INFRA
            var SaaMOOE_Enhanced = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 9).Sum(x => x.Beginning_balance);
            var SAACO_Enhanced = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 9).Sum(x => x.Beginning_balance);

            var funsorcePS_Enhanced = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 9).Sum(x => x.Beginning_balance); // SAA 2023 - 02 - 000162 - INFRA //SAA 2023-02-000680-INFRA
            var funsorceMOOE_Enhanced = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 9).Sum(x => x.Beginning_balance);
            var funsorceCO_Enhanced = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 9).Sum(x => x.Beginning_balance);

            decimal totalPS_Enhanced = SaaPS_Enhanced + funsorcePS_Enhanced;
            decimal totalMOOE_Enhanced = SaaMOOE_Enhanced + funsorceMOOE_Enhanced;
            decimal totalCO_Enhanced = SAACO_Enhanced + funsorceCO_Enhanced;
            decimal totalPrexc_Enhanced = totalPS_Enhanced + totalMOOE_Enhanced + totalCO_Enhanced; //Health Facilities Enhancement Program  310201100002000 /SAA 2023-02-000451

            var SaaPS_Local = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 10).Sum(x => x.Beginning_balance);
            var SaaMOOE_Local = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 10).Sum(x => x.Beginning_balance);
            var SaaCO_Local = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 10).Sum(x => x.Beginning_balance);

            var fundsourcePS_Local = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 10).Sum(x => x.Beginning_balance);
            var fundsourceMOOE_Local = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 10).Sum(x => x.Beginning_balance);
            var fundsourceCO_Local = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 10).Sum(x => x.Beginning_balance);

            decimal totalPS_Local = SaaPS_Local + fundsourcePS_Local;
            decimal totalMOOE_Local = SaaMOOE_Local + fundsourceMOOE_Local;
            decimal totalCO_Local = SaaCO_Local + fundsourceCO_Local;
            decimal totalPrexc_Local = totalPS_Local + totalMOOE_Local + totalCO_Local; //Local Health Systems Development and Assistance  // 310201100003000 

            var SaaPS_Pharmaceu = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 11).Sum(x => x.Beginning_balance);
            var SaaMOOE_Pharmaceu = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 11).Sum(x => x.Beginning_balance);
            var SaaCO_Pharmaceu = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 11).Sum(x => x.Beginning_balance);

            var funsorcePS_Pharmaceu = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 11).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Pharmaceu = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 11).Sum(x => x.Beginning_balance);
            var funsorceCO_Pharnaceu = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 11).Sum(x => x.Beginning_balance);

            decimal totalPS_Pharmaceu = SaaPS_Pharmaceu + funsorcePS_Pharmaceu;
            decimal totalMOOE_Parmaceu = SaaMOOE_Pharmaceu + funsorceMOOE_Pharmaceu;
            decimal totalCO_Pharmaceu = SaaCO_Pharmaceu + funsorceCO_Pharnaceu;
            decimal totalPrexc_Pharmaceu = totalPS_Pharmaceu + totalMOOE_Parmaceu + totalCO_Pharmaceu; //Pharmaceutical Management


            var Saa_SAA_000199 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.prexcId == 13).Sum(x => x.Beginning_balance);
            var totalfunsorce = funsources1.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var NHWSS = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 32).Sum(x => x.Beginning_balance);

            var SaaFunsourceTotal = funsources1.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 3 && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            decimal SaaFunsourceTotal_SaaTotal1_NHWSS = SaaFunsourceTotal + Saa_SAA_000199 + totalfunsorce + NHWSS; //NHWSS/

            var SaaPS_Service = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 8).Sum(x => x.Beginning_balance);
            var SaaMOOE_Service = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 8).Sum(x => x.Beginning_balance);
            var SaaCO_Service = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 8).Sum(x => x.Beginning_balance);

            var funsorcePS_Service = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 8).Sum(x => x.Beginning_balance);
            var funsorcesMOOE_Service = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 8).Sum(x => x.Beginning_balance);
            var funsorceCO_Service = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 8).Sum(x => x.Beginning_balance);

            decimal totalPS_Service = SaaPS_Service + funsorcePS_Service;
            decimal totalMOOE_Service = SaaMOOE_Service + funsorcesMOOE_Service;
            decimal totalCO_Service = SaaCO_Service + funsorceCO_Service;
            decimal totalPrexc_Service = totalPS_Service + totalMOOE_Service + totalCO_Service; //HEALTH SYSTEMS STRENGTHENING PROGRAM

            var SaaPS_NHWSS = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 32).Sum(x => x.Beginning_balance);
            var SaaMOOE_NHWSS = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 32).Sum(x => x.Beginning_balance);
            var SaaCO_NHWSS = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 32).Sum(x => x.Beginning_balance);

            var fundsorcePS_NHWSS = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var fundsorceMOOE_NHWSS = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 32).Sum(x => x.Beginning_balance);
            var fundsorceCO_NHWSS = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 32).Sum(x => x.Beginning_balance);

            decimal totalPS_NHWSS = SaaPS_NHWSS + fundsorcePS_NHWSS;
            decimal totalMOOE_NHWSS = SaaMOOE_NHWSS + fundsorceMOOE_NHWSS;
            decimal totalCO_NHWSS = SaaCO_NHWSS + fundsorceCO_NHWSS;
            decimal totalPrexc_NHWSS = totalPS_NHWSS + totalMOOE_NHWSS + totalCO_NHWSS;//total of  National Health Workforce Support System(NHWSS)



            var SaaPS_HRH = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 13).Sum(x => x.Beginning_balance);
            var SaaMOOE_HRH = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 13).Sum(x => x.Beginning_balance);
            var SaaCO_HRH = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 13).Sum(x => x.Beginning_balance);

            var funsourcePS_HRH = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            var funsourceMOOE_HRH = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            var funsourceCO_HRH = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 13).Sum(x => x.Beginning_balance);
            decimal totalPS_HRH = SaaPS_HRH + funsourcePS_HRH;
            decimal totalMOOE_HRH = SaaMOOE_HRH + funsourceMOOE_HRH;
            decimal totalCO_HRH = SaaCO_HRH + funsourceCO_HRH;

            decimal totalPrexc_HRH = totalPS_HRH + totalMOOE_HRH + totalCO_HRH;
            decimal total_Health_HumanSub = totalPrexc_HRH + totalPrexc_NHWSS; //NHWSSGRandTotal - National Health Workforce Support System ( NHWSS ) // 310202100002000 // 2023 HRHICM

            decimal service_delivery_sub_Prexc = totalPrexc_Pharmaceu + totalPrexc_Service + totalPrexc_Local + totalPrexc_Enhanced;//prexcTotal SERVICE DELIVERY SUB - PROGRAM

            decimal TotalHealthSystem_STRENGTHENING = service_delivery_sub_Prexc + totalPrexc_Health1 + total_Health_HumanSub;


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Value = "HEALTH SYSTEMS STRENGTHENING PROGRAM";
            var rangeAtoU101 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU101.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU101.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", TotalHealthSystem_STRENGTHENING));
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 9;
            worksheet.Cell(currentRow, 1).Value = "SERVICE DELIVERY SUB - PROGRAM";
            var rangeAtoU102 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU102.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU102.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", service_delivery_sub_Prexc));
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 8))
            {
                // if (saaSub.AllotmentClassId == 2 && saaSub.AppropriationId == 1 && saaSub.BudgetAllotmentId == 3 && saaSub.prexcId == 8)
                // {  
                foreach (var allotmentClassId in AllotmentClassId) //Health Facility Policy and Plan Development  310201100001000 HEALTH SYSTEMS STRENGTHENING PROGRAM
                {
                    var funsorceAllotClass = funsources1.Where(f => f.AllotmentClassId == allotmentClassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 8).ToList();
                    if (funsorceAllotClass.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllotClass)
                        {
                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {

                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Service));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);

                                if (totalPS_Service == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Service));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }
                                if (totalMOOE_Service == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Service));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, funsorce.AllotmentClass.Desc);
                                    }
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

                            }//list of Item
                             // }
                        }
                    } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //Health Facility Policy and Plan Development  310201100001000 ::  HEALTH SYSTEMS STRENGTHENING PROGRAM
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 8)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Service));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Service == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Service));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                            }
                            if (totalMOOE_Service == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);

                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Service));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }
                            }

                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }


                }

            }//end of else


            foreach (var suballot in subAllotments) //Health Facility Policy and Plan Development  310201100001000 ::  HEALTH SYSTEMS STRENGTHENING PROGRAM
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 8)
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

            if (SaaMOOE_Service == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Service));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;
            if (totalCO_Service == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");

            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Service));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 9))
            {
                foreach (var allotmentClassid in AllotmentClassId)//Health Facilities Enhancement Program  310201100002000 /SAA 2023-02-000451
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 9).ToList();

                    // if (subUacs.AllotmentClassId == 2 && subUacs.AppropriationId == 1 && subUacs.BudgetAllotmentId == 3 && subUacs.prexcId == 9)
                    // {
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {
                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Enhanced));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                                if (totalPS_Enhanced == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");

                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Enhanced));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                }
                                if (totalMOOE_Enhanced == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Enhanced));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
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

                            }
                            //} //end year
                        }

                    }
                }// end of foreach

            }
            else
            {
                foreach (var suballot in subAllotments) //Health Facilities Enhancement Program  310201100002000 SAA 2023-03-001503 //SAA 2023-04-002179
                {
                    if ((suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 9)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Enhanced));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Enhanced == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Enhanced));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                            }

                            if (totalMOOE_Enhanced == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Enhanced));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                }
                            }
                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }//end of foreach
            }//end of else


            foreach (var suballot in subAllotments) //Health Facilities Enhancement Program  310201100002000 SAA 2023-03-001503 //SAA 2023-04-002179
            {
                if ((suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 9)
                {

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
            if (SaaMOOE_Enhanced == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Enhanced));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }

            currentRow++;
            if (totalCO_Enhanced == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Enhanced));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 10))
            {
                foreach (var allotmentClassid in AllotmentClassId) // Local Health Systems Development and Assistance  // 310201100003000 // 2023 LHSDA
                {
                    // if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 10)
                    //{
                    var funsorceAllotClass = funsources1.Where(f => f.AllotmentClassId == allotmentClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 10).ToList();
                    if (funsorceAllotClass.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllotClass)
                        {

                            if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Local));
                                Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                if (totalPS_Local == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Local));
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                                if (totalMOOE_Local == 0)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Local));
                                    if (fundsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                }
                                paptitle = fundsorce.Prexc.pap_title;
                                papcode = fundsorce.Prexc.pap_code1;
                            }

                            SubAllotTitleRed(worksheet, ref currentRow, fundsorce.FundSourceTitle);
                            //duplicate1 = fundsorce.FundSourceTitle;

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
                            //}
                        }
                    }
                }//end of foreach
            }
            else
            {
                foreach (var suballot in subAllotments)  // Local Health Systems Development and Assistance  // 310201100003000 // 2023 LHSDA
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 10)
                    {

                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Local));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Local == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Local));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                            }
                            if (totalMOOE_Local == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Local));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                                }
                            }
                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }
                } //end of foreach

            }

            foreach (var suballot in subAllotments) // Local Health Systems Development and Assistance  // 310201100003000 // 2023 LHSDA ::
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 10)
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
            if (SaaMOOE_Local == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "total saa's 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:n2}", SaaMOOE_Local));
                TOTALSaa(worksheet, ref currentRow, "total saa's 2023");
            }
            currentRow++;
            if (totalCO_Local == 0)
            {

                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Local));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            currentRow++;
            bool saadisplayed = false; bool saadisplayed1 = false;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 11))
            {
                foreach (var allotmentclassId in AllotmentClassId) //Pharmaceutical Management // 310201100004000 // SAA 2023-03-000952
                {
                    // if (subaalot.AllotmentClassId == 2 && subaalot.AppropriationId == 1 && subaalot.BudgetAllotmentId == 3 && subaalot.prexcId == 11)
                    //{
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotmentclassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 11).ToList();
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var funsorce in funsorceAllot)
                        {

                            if (funsorce.Prexc.pap_title != paptitle || funsorce.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Pharmaceu));
                                Prexc_papTitle(worksheet, ref currentRow, funsorce.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsorce.Prexc.pap_code1);
                                if (totalPS_Pharmaceu == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Pharmaceu));
                                    if (funsorce.AllotmentClassId == 1)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                    }
                                }
                                if (totalMOOE_Parmaceu == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Parmaceu));
                                    if (funsorce.AllotmentClassId == 2)
                                    {
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                    }
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

                            }
                        }
                        // } // year
                    } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)//Pharmaceutical Management // 310201100004000 // SAA 2023-03-000952
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 11)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Pharmaceu));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Pharmaceu == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Pharmaceu));
                                if (suballot.AllotmentClassId == 1)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                }
                            }
                            if (totalMOOE_Parmaceu == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Parmaceu));
                                if (suballot.AllotmentClassId == 2)
                                {
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance & Other Operating Expenses ");
                                }
                            }
                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }


                }
            }//end else

            foreach (var suballot in subAllotments)//Pharmaceutical Management // 310201100004000 // SAA 2023-03-000952
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 11)
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


            if (SaaMOOE_Pharmaceu == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Pharmaceu));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            }
            currentRow++;
            if (totalCO_Pharmaceu == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Pharmaceu));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold();
            worksheet.Cell(currentRow, 1).Value = "HEALTH HUMAN RESOURCE SUB - PROGRAM";
            var rangeAtoU103 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU103.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU103.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", total_Health_HumanSub));
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 13))
            {
                foreach (var AllotmentclassId in AllotmentClassId) // 310202100002000 // 2023 HRHICM 
                {
                    var funsorceAllotClass = funsources1.Where(f => f.AllotmentClassId == AllotmentclassId && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 13).ToList();
                    //if (funsource.AllotmentClassId == 2 && funsource.AppropriationId == 1 && funsource.BudgetAllotmentId == 3 && funsource.PrexcId == 13)
                    //{
                    if (funsorceAllotClass.Count > 0)
                    {
                        foreach (var funsource in funsorceAllotClass)
                        {

                            if (funsource.Prexc.pap_title != paptitle || funsource.Prexc.pap_code1 != papcode)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_HRH));
                                Prexc_papTitle(worksheet, ref currentRow, funsource.Prexc.pap_title);
                                Prexc_papcode(worksheet, ref currentRow, funsource.Prexc.pap_code1);

                                if (totalPS_HRH == 0)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_HRH));
                                }
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                                if (totalMOOE_HRH == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                                }
                                else
                                {
                                    if (funsource.AllotmentClassId == 2)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_HRH));
                                    }
                                }
                                ItemSubPrexc(worksheet, ref currentRow, funsource.AllotmentClass.Desc);

                                paptitle = funsource.Prexc.pap_title;
                                papcode = funsource.Prexc.pap_code1;
                            }

                            SubAllotTitleRed(worksheet, ref currentRow, funsource.FundSourceTitle);
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

                        }

                        // } //year and prexc
                    } // end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments) // 310202100002000 // 2023 HRHICM ::
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 13)
                    {

                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_HRH));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_HRH == 0)
                            {

                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_HRH));
                            }
                            if (suballot.AllotmentClassId == 1)
                            {
                                ItemSubPrexc(worksheet, ref currentRow, "Personal Services");
                            }

                            if (totalMOOE_HRH == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_HRH));
                            }
                            if (suballot.AllotmentClassId == 2)
                            {
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }


                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }

                    }

                }


            }//end of else


            foreach (var suballot1 in subAllotments) // 310202100002000 // 2023 HRHICM continue::
            {
                if ((suballot1.AllotmentClassId == 2 || suballot1.AllotmentClassId == 1 || suballot1.AllotmentClassId == 3) && appropiationId.Contains(suballot1.AppropriationId) && budgetallotmentId.Contains(suballot1.BudgetAllotmentId.GetValueOrDefault()) && suballot1.prexcId == 13)
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

            if (SaaMOOE_HRH == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_HRH));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;
            if (totalCO_HRH == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", "-"));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_HRH));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }

            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 32))
            {
                foreach (var allotClassid in AllotmentClassId) //National Health Workforce Support System (NHWSS) // 310202100003000 //2023 NHWSS-PS
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 32).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 32)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                         if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                         {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                            {

                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_NHWSS));
                                Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                                Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                if (totalPS_NHWSS == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_NHWSS));
                                    ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                }
                                if (totalMOOE_NHWSS == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                }
                                else       
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_NHWSS));
                                  
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                       
                                    
                                }


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
                      }
                    }
                    // } //end of foreach
                }
            }
            else
            {
              foreach(var suballot in subAllotments)
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 32)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_NHWSS));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_NHWSS == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_NHWSS));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_NHWSS == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_NHWSS));
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }


                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments) // part -SAA 2023-03-000988 2023 NHWSS-PS
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 32)
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

            if (SaaMOOE_NHWSS == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_NHWSS));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S");
            }
            currentRow++;

            if (totalCO_NHWSS == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_NHWSS));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Health PromotionSUB - PROGRAM";
            var rangeAtoU104 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU104.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU104.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health1));


            currentRow++;
            //start

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 14))
            {
                foreach (var allotClassid in AllotmentClassId) //2023 HEALTH PROMOTION 310203100001000
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 14).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 14)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health1));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                    if (totalPS_Health1 == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health1));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Health1 == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health1));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");


                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //2023 HEALTH PROMOTION 310203100001000 contenue:: SAA 2023-03-001447
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 14)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Health1));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Health1 == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Health1));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Health1 == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Health1));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");


                            }


                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)  //2023 HEALTH PROMOTION 310203100001000 contenue:: SAA 2023-03-001447
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 14)
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

            if (SaaMOOE_Health == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Health));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Health1 == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Health1));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;
            var SaaPS_SubProgram = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 23).Sum(x => x.Beginning_balance);
            var SaaMOOE_SubProgram = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 23).Sum(x => x.Beginning_balance);
            var SaaCO_SubProgram = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 23).Sum(x => x.Beginning_balance);

            var funsorcePS_SubProgram = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            var funsorceMOOE_SubProgram = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            var funsorceCO_SubProgram = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 23).Sum(x => x.Beginning_balance);
            decimal totalPS_SubProgram = SaaPS_SubProgram + funsorcePS_SubProgram;
            decimal totalMOOE_SubProgram = funsorceMOOE_SubProgram + SaaMOOE_SubProgram;
            decimal totalCO_Subprogram = funsorceCO_SubProgram + SaaCO_SubProgram;
            decimal totalPrexc_SubProgram = totalPS_SubProgram + totalMOOE_SubProgram + totalCO_Subprogram;  //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM

            var SaaPS_Control = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 37).Sum(x => x.Beginning_balance);
            var SaaMOOE_Control = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 37).Sum(x => x.Beginning_balance);
            var SaaCO_Control = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 37).Sum(x => x.Beginning_balance);

            var funsorcePS_Control = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 37).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Control = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 37).Sum(x => x.Beginning_balance);
            var funsorceCO_Control = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 37).Sum(x => x.Beginning_balance);

            decimal totalPS_Control = SaaPS_Control + funsorcePS_Control;
            decimal totalMOOE_Control = funsorceMOOE_Control + SaaMOOE_Control;
            decimal totalCO_Control = SaaCO_Control + funsorceCO_Control;
            decimal totalPrexc_Control = totalPS_Control + totalMOOE_Control + totalCO_Control;//Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD

            var SaaPS_Family = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 18).Sum(x => x.Beginning_balance);
            var SaaMOOE_Family = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 18).Sum(x => x.Beginning_balance);
            var SaaCO_Family = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 18).Sum(x => x.Beginning_balance);

            var funsorcePS_Family = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 18).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Family = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 18).Sum(x => x.Beginning_balance);
            var funsorceCO_Family = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 18).Sum(x => x.Beginning_balance);

            decimal totalPS_Family = SaaPS_Family + funsorcePS_Family;
            decimal totalMOOE_Family = SaaMOOE_Family + funsorceMOOE_Family;
            decimal totalCO_Family = SaaCO_Family + funsorceCO_Family;
            decimal totalPrexc_Family = totalPS_Family + totalMOOE_Family + totalCO_Family; //Family Health, Nutrition and Responsible Parenting // 2023 FHINRP

            var SaaPS_Environ = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 16).Sum(x => x.Beginning_balance);
            var SaaMOOE_Environ = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 16).Sum(x => x.Beginning_balance);
            var SaaCO_Environ = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 16).Sum(x => x.Beginning_balance);

            var funsourcePS_Environ = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 16).Sum(x => x.Beginning_balance);
            var funsourceMOOE_Environ = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 16).Sum(x => x.Beginning_balance);
            var funsourceCO_Environ = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 16).Sum(x => x.Beginning_balance);
            decimal totalPS_Environ = SaaPS_Environ + funsourcePS_Environ;
            decimal totalMOOE_Environ = funsourceMOOE_Environ + SaaMOOE_Environ;
            decimal totalCO_Environ = SaaCO_Environ + funsourceCO_Environ;
            decimal totalPrexc_Environ = totalPS_Environ + totalMOOE_Environ + totalCO_Environ;  //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044

            var SaaPS_Manage = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS
            var SaaMOOE_Manage = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS
            var SaaCO_Manage = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 15).Sum(x => x.Beginning_balance);//2023 PHM-HFDS

            var fundSorcePS_Manage = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179
            var fundSorceMOOE_Manage = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179
            var fundSorceCO_Manage = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 15).Sum(x => x.Beginning_balance); //SAA 2023-04-002179


            decimal totalPS_Manage = SaaPS_Manage + fundSorcePS_Manage;
            decimal totalMOOE_Manage  = SaaMOOE_Manage + fundSorceMOOE_Manage;
            decimal totalCO_Manage = fundSorceCO_Manage + SaaCO_Manage;
            decimal totalPrexc_Manage = totalPS_Manage + totalMOOE_Manage + totalCO_Manage;  //Public Health Management // 310301100001000 // 2023 PHM-PS

            var SaaPS_Public = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 40).Sum(x => x.Beginning_balance);
            var SaaMOOE_Public = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 40).Sum(x => x.Beginning_balance);
            var SaaCO_Public = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 40).Sum(x => x.Beginning_balance);
            //Public Health Emergency Benefits and Allowances // 310300200003000 // SAA 2023-07-003434
            var fundsorcePS_Public = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 40).Sum(x => x.Beginning_balance);
            var fundsorceMOOE_Public = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 40).Sum(x => x.Beginning_balance);
            var fundsorceCO_Public = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 40).Sum(x => x.Beginning_balance);
           
            decimal totalPS_Public = SaaPS_Public + fundsorcePS_Public;
            decimal totalMOOE_Public = fundsorceMOOE_Public + SaaMOOE_Public;
            decimal totalCO_Public = SaaCO_Public + fundsorceCO_Public;
            decimal totalPrexc_Public = totalCO_Public + totalMOOE_Public + totalPS_Public;



            decimal total_PUBLIC_HEALTHPROGRAM = totalPrexc_Public + totalPrexc_SubProgram + totalPrexc_Control + totalPrexc_Family + totalPrexc_Environ + totalPrexc_Manage;

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
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrexc_Public);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            bool totalSaaDisplayed = false;


            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 40))
            {
                foreach (var allotClassid in AllotmentClassId)  //Public Health Emergency Benefits and Allowances // 310300200003000 // SAA 2023-07-003434
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 14).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 14)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Public));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                    if (totalPS_Public == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Public));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Public == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Public));
                                        
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)  //Public Health Emergency Benefits and Allowances // 310300200003000 // SAA 2023-07-003434
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 40)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Public));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Public == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Public));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if(suballot.AllotmentClassId == 2)
                            {
                                if (totalMOOE_Public == 0)
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, "-");
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                }
                                else
                                {
                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Public));
                                    ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                }
                            }
                           


                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)  //Public Health Emergency Benefits and Allowances // 310300200003000 // SAA 2023-07-003434
            { 
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 40)
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

            if (SaaMOOE_Public == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Public));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Public == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Public));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            HashSet<string> uniqueAccountTitles5 = new HashSet<string>(); HashSet<string> uniqueExpenseCodes5 = new HashSet<string>();


            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PUBLIC HEALTH MANAGEMENT SUB - PROGRAM";
            var rangeAtoU2 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU2.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.SetBold();
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrexc_Manage);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 15))
            {
                foreach (var allotClassid in AllotmentClassId)  ////Public Health Management // 310301100001000 // 2023 PHM-PS
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 15).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 14)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Manage));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                    worksheet.Cell(currentRow, 1).Value = "GAA";
                                    currentRow++;
                                    if (totalPS_Manage == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Manage));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Manage == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Manage));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)  //Public Health Management // 310301100001000 // 2023 PHM-PS
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 15)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Public));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Manage == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Manage));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Manage == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Manage));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)  //Public Health Management // 310301100001000 // 2023 PHM-PS
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 15)
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

            if (totalMOOE_Manage == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Manage));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Manage == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Manage));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "ENVIRONMENTAL AND OCCUPATIONAL HEALTH SUB - PROGRAM";
            var rangeAtoU3 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU3.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrexc_Environ);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;


            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 16))
            {
                foreach (var allotClassid in AllotmentClassId)   //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 16).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 16)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Environ));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); //National Health Workforce Support System (NHWSS)
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);

                                    worksheet.Cell(currentRow, 1).Value = "GAA";
                                    currentRow++;
                                    if (totalPS_Environ == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Environ));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Environ == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Environ));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)   //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 16)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Environ));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Environ == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Environ));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Environ == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Environ));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)   //Environmental and Occupational Health // 310302100001000 // SAA 2023-04-002044
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 16)
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

            if (totalMOOE_Environ == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Environ));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Environ == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Environ));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "FAMILY HEALTH SUB - PROGRAM";
            var rangeAtoU4 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU4.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = string.Format("{0:N2}", totalPrexc_Family);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 18))
            {
                foreach (var allotClassid in AllotmentClassId)   // Prevention and Control of Communicable Diseases// 310308100001000 // 2023 FHINRP
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 18).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 14)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Family));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title); 
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                    if (totalPS_Family == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Family));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Family == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Family));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)   // Prevention and Control of Communicable Diseases// 310308100001000 // 2023 FHINRP
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 18)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Family));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);

                            if (totalPS_Family == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Family));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Family == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Family));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)  // Prevention and Control of Communicable Diseases// 310308100001000 // 2023 FHINRP
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 18)
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

            if (totalMOOE_Family == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Family));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Family == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Family));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU5 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU5.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalPrexc_Control);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 37))
            {
                foreach (var allotClassid in AllotmentClassId)  //Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 37).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 14)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Control));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                    if (totalPS_Control == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Control));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Control == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Control));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)   //Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 37)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Control));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Control == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Control));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Control == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Control));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments)  //Prevention and Control of Communicable Disea // 310308100001000 // 2023 PCCD
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 37)
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

            if (totalMOOE_Control == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Control));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Control == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Control));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM";
            var rangeAtoU24 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU24.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FBE8"); //yellow
            rangeAtoU24.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            if (funsources1.Any(x => (x.AllotmentClassId == 1 || x.AllotmentClassId == 2 || x.AllotmentClassId == 3) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 23))
            {
                foreach (var allotClassid in AllotmentClassId) //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 23).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 23)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_SubProgram));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                    if (totalPS_SubProgram == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_SubProgram));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_SubProgram == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_SubProgram));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments)  //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 23)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_SubProgram));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_SubProgram == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_SubProgram));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_SubProgram == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_SubProgram));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments) //PREVENTION AND CONTROL OF NON - COMMUNICABLE DISEASES SUB - PROGRAM
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 23)
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

            if (totalMOOE_SubProgram == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Control));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Subprogram == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Subprogram));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }

            currentRow++;
           
            var SaaPS_Epide = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 24).Sum(x => x.Beginning_balance);
            var SaaMOOE_Epide = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 24).Sum(x => x.Beginning_balance);
            var SaaCO_Epide = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 24).Sum(x => x.Beginning_balance);
            
            var funsorcePS_Epide = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Epide = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            var funsorceCO_Epide = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 24).Sum(x => x.Beginning_balance);
            decimal totalPS_Epide = SaaPS_Epide + funsorcePS_Epide;
            decimal totalMOOE_Epide = SaaMOOE_Epide + funsorceMOOE_Epide;
            decimal totalCO_Epide = funsorceCO_Epide + SaaCO_Epide;
            decimal totalPrexc_Epide = totalPS_Epide + totalMOOE_Epide + totalCO_Epide;//2023 ES // Epidemiology and Surveillance // 310400100001000

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "Epidemiology and Surveillance PROGRAM";
            var rangeAtoU6 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU6.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU6.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            SaaGaaBalance(worksheet, ref currentRow, String.Format("{0:n2}", totalPrexc_Epide));
            currentRow++;

            if (funsources1.Any(x =>(new List<int> { 1, 2, 3 }).Contains(x.AllotmentClassId) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 24))
            {
                foreach (var allotClassid in AllotmentClassId) //2023 ES // Epidemiology and Surveillance // 310400100001000
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 24).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 24)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Epide));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                    if (totalPS_Epide == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Epide));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Epide == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Epide));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //2023 ES // Epidemiology and Surveillance // 310400100001000
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 24)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {
                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Epide));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Epide == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Epide));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Epide == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Epide));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments) //2023 ES // Epidemiology and Surveillance // 310400100001000
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 24)
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

            if (totalMOOE_Epide == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Epide));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Epide == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Epide));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            currentRow++;

            var SaaPS_Emerge = subAllotments.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 25).Sum(x => x.Beginning_balance);
            var SaaMOOE_Emerge = subAllotments.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 25).Sum(x => x.Beginning_balance);
            var SaaCO_Emerge = subAllotments.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.prexcId == 25).Sum(x => x.Beginning_balance);

            var funSorcePS_Emerge = funsources1.Where(x => x.AllotmentClassId == 1 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            var funsorceMOOE_Emerge = funsources1.Where(x => x.AllotmentClassId == 2 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            var funsorceCO_Emerge = funsources1.Where(x => x.AllotmentClassId == 3 && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 25).Sum(x => x.Beginning_balance);
            decimal totalPS_Emerge = SaaPS_Emerge + funSorcePS_Emerge;
            decimal totalMOOE_Emerge = SaaMOOE_Emerge + funsorceMOOE_Emerge;
            decimal totalCO_Emerge = funsorceCO_Emerge + SaaCO_Emerge;
            decimal totalPrexc_Emerge = totalPS_Emerge + totalMOOE_Emerge + totalCO_Emerge;

            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 1).Value = "HEALTH EMERGENCY MANAGEMENT PROGRAM";
            var rangeAtoU7 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 21));
            rangeAtoU7.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff80b3");
            rangeAtoU7.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 10.5;
            worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalPrexc_Emerge);
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            currentRow++;


            if (funsources1.Any(x => (new List<int> { 1, 2, 3 }).Contains(x.AllotmentClassId) && appropiationId.Contains(x.AppropriationId) && budgetallotmentId.Contains(x.BudgetAllotmentId.GetValueOrDefault()) && x.PrexcId == 25))
            {
                foreach (var allotClassid in AllotmentClassId) //HEALTH EMERGENCY MANAGEMENT PROGRAM 2023 HEPR0
                {
                    var funsorceAllot = funsources1.Where(f => f.AllotmentClassId == allotClassid && appropiationId.Contains(f.AppropriationId) && budgetallotmentId.Contains(f.BudgetAllotmentId.GetValueOrDefault()) && f.PrexcId == 25).ToList();
                    //if (fundsorce.AllotmentClassId == 1 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 25)
                    //{
                    if (funsorceAllot.Count > 0)
                    {
                        foreach (var fundsorce in funsorceAllot)
                        {
                            if (allotClassid == 1 || allotClassid == 2 || allotClassid == 3)
                            {
                                if (fundsorce.Prexc.pap_title != paptitle || fundsorce.Prexc.pap_code1 != papcode)
                                {

                                    SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Emerge));
                                    Prexc_papTitle(worksheet, ref currentRow, fundsorce.Prexc.pap_title);
                                    Prexc_papcode(worksheet, ref currentRow, fundsorce.Prexc.pap_code1);
                                    if (totalPS_Emerge == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Emerge));
                                        ItemSubPrexc(worksheet, ref currentRow, fundsorce.AllotmentClass.Desc);
                                    }
                                    if (totalMOOE_Emerge == 0)
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, "-");
                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }
                                    else
                                    {
                                        SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Emerge));

                                        ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                                    }


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
                        }
                    }
                    // } //end of foreach
                }
            }
            else
            {
                foreach (var suballot in subAllotments) //HEALTH EMERGENCY MANAGEMENT PROGRAM 2023 HEPR
                {
                    if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 25)
                    {
                        if (suballot.prexc.pap_title != paptitle || suballot.prexc.pap_code1 != papcode)
                        {

                            SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPrexc_Emerge));
                            Prexc_papTitle(worksheet, ref currentRow, suballot.prexc.pap_title);
                            Prexc_papcode(worksheet, ref currentRow, suballot.prexc.pap_code1);
                            if (totalPS_Emerge == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Personnel Services");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalPS_Emerge));
                                ItemSubPrexc(worksheet, ref currentRow, suballot.AllotmentClass.Desc);
                            }
                            if (totalMOOE_Emerge == 0)
                            {
                                SaaGaaBalance(worksheet, ref currentRow, "-");
                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }
                            else
                            {
                                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalMOOE_Emerge));

                                ItemSubPrexc(worksheet, ref currentRow, "Maintenance and Other Operating Expenses");
                            }



                            paptitle = suballot.prexc.pap_title;
                            papcode = suballot.prexc.pap_code1;
                        }
                    }
                }

            }//end of else
            foreach (var suballot in subAllotments) //HEALTH EMERGENCY MANAGEMENT PROGRAM 2023 HEPR
            {
                if ((suballot.AllotmentClassId == 1 || suballot.AllotmentClassId == 2 || suballot.AllotmentClassId == 3) && appropiationId.Contains(suballot.AppropriationId) && budgetallotmentId.Contains(suballot.BudgetAllotmentId.GetValueOrDefault()) && suballot.prexcId == 25)
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

            if (SaaMOOE_Emerge == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", SaaMOOE_Emerge));
                TOTALSaa(worksheet, ref currentRow, "TOTAL SAA'S 2023");
            }
            currentRow++;

            if (totalCO_Emerge == 0)
            {
                SaaGaaBalance(worksheet, ref currentRow, "-");
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }
            else
            {
                SaaGaaBalance(worksheet, ref currentRow, string.Format("{0:N2}", totalCO_Emerge));
                ItemSubPrexc(worksheet, ref currentRow, "Capital Outlays ");
            }

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
                if (fundsorce.AllotmentClassId == 2 && fundsorce.AppropriationId == 1 && fundsorce.BudgetAllotmentId == 3 && fundsorce.PrexcId == 26) // console.writeLine()
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
            if (MOOE7 == 0)
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
                        if (PS24 == 0)
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
                        if (PS25 == 0)
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
            if (CO20 == 0)
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

            foreach (var uacs in _MyDbContext.Uacs.ToList())
            {
                if (uacs.UacsId == 24)
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
            var totalconapsaa18 = subAllotments.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == 1 && x.prexcId == 26).Sum(x => x.Beginning_balance);
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
                    worksheet.Cell(currentRow, 3).Value = String.Format("{0:n2}", totalconapsaa21);
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