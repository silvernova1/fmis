using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using fmis.Data;
using fmis.DataHealpers;
using fmis.Filters;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using fmis.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace fmis.Controllers.Budget.John
{
    public class ReportsController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public ReportsController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass", "");
            return View(await _MyDbContext.FundSources.ToListAsync());
        }

        #region COOKIES

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        #endregion


        public IActionResult Export(string fn, string date_from, string date_to, int? post_yearly_reference)
        {
            const string yearly_reference = "_yearly_reference";

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

            date1.ToString("yyyy-MM-dd 00:00:00");
            date2.ToString("yyyy-MM-dd 23:59:59");
            DateTime dateTimeNow = date2;
            DateTime dateTomorrow = dateTimeNow.Date.AddDays(1);
            /*var lastDayOfMonth = DateTime.DaysInMonth(date1.Year, date1.Month);*/


            //LASTDAY OF THE MONTH
            var firstDayOfMonth = new DateTime(date2.Year, date2.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime lastday = Convert.ToDateTime(lastDayOfMonth);
            lastday.ToString("yyyy-MM-dd 23:59:59");

            var subs = (from sub in _MyDbContext.SubAllotment
                                join ba in _MyDbContext.Budget_allotments
                                on sub.BudgetAllotmentId equals ba.BudgetAllotmentId
                                join yr in _MyDbContext.Yearly_reference
                                on ba.YearlyReferenceId equals yr.YearlyReferenceId
                                where ba.BudgetAllotmentId == 2 && sub.AllotmentClassId == 2 && sub.AppropriationId == 2
                        select new
                                {
                                    AllotmentTitle = sub.Suballotment_title
                                }).ToList();


            var dateTime = _MyDbContext.FundSources.Where(x => x.CreatedAt >= date1 && x.CreatedAt <= dateTomorrow).Select(y => new { y.FundSourceTitle }).ToList();


            var fortheMonthTotalinTotalPS = (from oa in _MyDbContext.ObligationAmount
                                             join o in _MyDbContext.Obligation
                                             on oa.ObligationId equals o.Id
                                             join f in _MyDbContext.FundSources
                                             on o.FundSourceId equals f.FundSourceId
                                             where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                             select new
                                             {
                                                 amount = oa.Amount,
                                                 uacsId = oa.UacsId,
                                                 sourceId = o.FundSourceId,
                                                 sourceType = o.source_type,
                                                 date = o.Date,
                                                 status = o.status,
                                                 allotmentClassID = f.AllotmentClassId,
                                                 appropriationID = f.AppropriationId,
                                                 fundSourceTitle = f.FundSourceTitle
                                             }).ToList();

            var asAtTotalinTotalPS = (from oa in _MyDbContext.ObligationAmount
                                      join o in _MyDbContext.Obligation
                                      on oa.ObligationId equals o.Id
                                      join f in _MyDbContext.FundSources
                                      on o.FundSourceId equals f.FundSourceId
                                      where o.Date >= date1 && o.Date <= date2
                                      select new
                                      {
                                          amount = oa.Amount,
                                          sourceId = o.FundSourceId,
                                          sourceType = o.source_type,
                                          uacsId = oa.UacsId,
                                          status = o.status,
                                          allotmentClassID = f.AllotmentClassId,
                                          appropriationID = f.AppropriationId,
                                          fundSourceTitle = f.FundSourceTitle,
                                          fundSourceBudgetAllotmentId = f.BudgetAllotmentId
                                      }).ToList();
            var PsTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle != "AUTOMATIC APPROPRIATION" && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);




            var fortheMonthTotalinTotalPSConap = (from oa in _MyDbContext.ObligationAmount
                                                  join o in _MyDbContext.Obligation
                                                  on oa.ObligationId equals o.Id
                                                  join f in _MyDbContext.FundSources
                                                  on o.FundSourceId equals f.FundSourceId
                                                  where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                  select new
                                                  {
                                                      amount = oa.Amount,
                                                      uacsId = oa.UacsId,
                                                      sourceId = o.FundSourceId,
                                                      sourceType = o.source_type,
                                                      date = o.Date,
                                                      status = o.status,
                                                      allotmentClassID = f.AllotmentClassId,
                                                      appropriationID = f.AppropriationId
                                                  }).ToList();

            var asAtTotalinTotalPSConap = (from oa in _MyDbContext.ObligationAmount
                                           join o in _MyDbContext.Obligation
                                           on oa.ObligationId equals o.Id
                                           join f in _MyDbContext.FundSources
                                           on o.FundSourceId equals f.FundSourceId
                                           where o.Date >= date1 && o.Date <= date2
                                           select new
                                           {
                                               amount = oa.Amount,
                                               sourceId = o.FundSourceId,
                                               sourceType = o.source_type,
                                               uacsId = oa.UacsId,
                                               status = o.status,
                                               allotmentClassID = f.AllotmentClassId,
                                               appropriationID = f.AppropriationId
                                           }).ToList();


            var PsConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Sum(x => x.Beginning_balance);
            var unobligatedTotalinTotalPSConap = PsConapTotal - asAtTotalinTotalPSConap.Where(x => x.appropriationID == 2).Sum(x => x.amount);


            var fortheMonthTotalinTotalCoConap = (from oa in _MyDbContext.ObligationAmount
                                                  join o in _MyDbContext.Obligation
                                                  on oa.ObligationId equals o.Id
                                                  join f in _MyDbContext.FundSources
                                                  on o.FundSourceId equals f.FundSourceId
                                                  where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                  select new
                                                  {
                                                      amount = oa.Amount,
                                                      uacsId = oa.UacsId,
                                                      sourceId = o.FundSourceId,
                                                      sourceType = o.source_type,
                                                      date = o.Date,
                                                      status = o.status,
                                                      allotmentClassID = f.AllotmentClassId,
                                                      appropriationID = f.AppropriationId
                                                  }).ToList();

            var asAtTotalinTotalCoConap = (from oa in _MyDbContext.ObligationAmount
                                           join o in _MyDbContext.Obligation
                                           on oa.ObligationId equals o.Id
                                           join f in _MyDbContext.FundSources
                                           on o.FundSourceId equals f.FundSourceId
                                           where o.Date >= date1 && o.Date <= date2
                                           select new
                                           {
                                               amount = oa.Amount,
                                               sourceId = o.FundSourceId,
                                               sourceType = o.source_type,
                                               uacsId = oa.UacsId,
                                               status = o.status,
                                               allotmentClassID = f.AllotmentClassId,
                                               appropriationID = f.AppropriationId
                                           }).ToList();




            var CoConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
            var unobligatedTotalinTotalCoConap = CoConapTotal - asAtTotalinTotalCoConap.Where(x => x.appropriationID == 2 && x.allotmentClassID == 3).Sum(x => x.amount);


            var fortheMonthTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                               join o in _MyDbContext.Obligation
                                               on oa.ObligationId equals o.Id
                                               join f in _MyDbContext.FundSources
                                               on o.FundSourceId equals f.FundSourceId
                                               where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                               select new
                                               {
                                                   amount = oa.Amount,
                                                   uacsId = oa.UacsId,
                                                   sourceId = o.FundSourceId,
                                                   date = o.Date,
                                                   status = o.status,
                                                   allotmentClassID = f.AllotmentClassId
                                               }).ToList();

            var asAtTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                        join o in _MyDbContext.Obligation
                                        on oa.ObligationId equals o.Id
                                        join f in _MyDbContext.FundSources
                                        on o.FundSourceId equals f.FundSourceId
                                        where o.Date >= date1 && o.Date <= date2
                                        select new
                                        {
                                            amount = oa.Amount,
                                            sourceId = o.FundSourceId,
                                            uacsId = oa.UacsId,
                                            status = o.status,
                                            allotmentClassID = f.AllotmentClassId
                                        }).ToList();

            var fortheMonthTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                             join o in _MyDbContext.Obligation
                                             on oa.ObligationId equals o.Id
                                             join f in _MyDbContext.FundSources
                                             on o.FundSourceId equals f.FundSourceId
                                             where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                             select new
                                             {
                                                 amount = oa.Amount,
                                                 uacsId = oa.UacsId,
                                                 sourceId = o.FundSourceId,
                                                 date = o.Date,
                                                 status = o.status,
                                                 allotmentClassID = f.AllotmentClassId
                                             }).ToList();

            var asAtTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                      join o in _MyDbContext.Obligation
                                      on oa.ObligationId equals o.Id
                                      join f in _MyDbContext.FundSources
                                      on o.FundSourceId equals f.FundSourceId
                                      where o.Date >= date1 && o.Date <= date2
                                      select new
                                      {
                                          amount = oa.Amount,
                                          sourceId = o.FundSourceId,
                                          uacsId = oa.UacsId,
                                          status = o.status,
                                          allotmentClassID = f.AllotmentClassId
                                      }).ToList();





            DataTable dt = new DataTable("Saob Report");

            var ballots = from ballot in _MyDbContext.Budget_allotments.ToList()
                          select ballot;



            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Column(7).AdjustToContents();
                ws.Worksheet.SheetView.FreezeColumns(12);
                ws.Worksheet.SheetView.FreezeRows(19);

                //IXLRange range = ws.Range(ws.Cell(2, 1).Address, ws.Cell(13, 11).Address);

                //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                var currentRow = 20;
                var currentColumn = 1;
                Double total = 0.00;
                Double allotment_total = 0;
                Double allotment_totalSaa = 0;
                Double suballotment_total = 0;
                Double GrandTotal = 0;


                //PS SAA
                var PsTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                var unobligatedTotalinTotalPSSaa = PsTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                var totalPercentPSSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;


                var wsFreeze = ws.Worksheet.Cell("Freeze View");

                ws.Worksheet.SheetView.FreezeColumns(12);
                ws.Worksheet.SheetView.FreezeRows(19);


                //range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                ws.Cell("A7").Style.Font.FontSize = 10;
                ws.Cell("A7").Style.Font.FontName = "Calibri Light";
                ws.Range("A7").Value = "Department:";
                ws.Range("A7").Style.Alignment.WrapText = false;

                ws.Cell("A8").Style.Font.FontSize = 10;
                ws.Cell("A8").Style.Font.FontName = "Calibri Light";
                ws.Range("A8").Value = "Agency /OU:";
                ws.Range("A8").Style.Alignment.WrapText = false;

                ws.Cell("A9").Style.Font.FontSize = 10;
                ws.Cell("A9").Style.Font.FontName = "Calibri Light";
                ws.Range("A9").Value = "Fund";
                ws.Range("A9").Style.Alignment.WrapText = false;

                ws.Cell("E7").Style.Font.SetBold();
                ws.Cell("E7").Style.Font.FontSize = 10;
                ws.Cell("E7").Style.Font.FontName = "Lucida Bright";
                ws.Range("E7").Value = "HEALTH";
                ws.Range("E7").Style.Alignment.WrapText = false;

                ws.Cell("E8").Style.Font.SetBold();
                ws.Cell("E8").Style.Font.FontSize = 10;
                ws.Cell("E8").Style.Font.FontName = "Lucida Bright";
                ws.Range("E8").Value = "CENTRAL VISAYAS CENTER FOR HEALTH DEVELOPMENT";
                ws.Range("E8").Style.Alignment.WrapText = false;

                ws.Cell("E9").Style.Font.SetBold();
                ws.Cell("E9").Style.Font.FontSize = 10;
                ws.Cell("E9").Style.Font.FontName = "Lucida Bright";
                ws.Range("E9").Value = "GAA";
                ws.Range("E9").Style.Alignment.WrapText = false;


                ws.Cell("A4").Style.Font.SetBold();
                ws.Cell("A4").Style.Font.FontSize = 10;
                ws.Cell("A4").Style.Font.FontName = "Lucida Bright";
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("A4").RichText.AddText("STATEMENT OF ALLOTMENTS, OBLIGATIONS, DISBURSEMENTS AND BALANCES");
                ws.Range("A4:W4").Merge();
                ws.Cell("A4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("A5").Style.Font.FontSize = 10;
                ws.Cell("A5").Style.Font.FontName = "Lucida Bright";
                ws.Cell("A5").Value =  "As at" + " " + date2.ToString("MMMM dd, yyyy");
                ws.Range("A5:W5").Merge();
                ws.Cell("A5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("A6").Style.Font.FontSize = 10;
                ws.Cell("A6").Style.Font.FontName = "Lucida Bright";
                ws.Cell("A6").Value = "(In Pesos)";
                ws.Range("A6:W6").Merge();
                ws.Cell("A6").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                //ws.Columns(11, 13).Hide();
                ws.Rows(11, 13).Hide();
                ws.Rows(1, 3).Hide();
                ws.Row(18).Hide();

                // Merge a range
                ws.Cell(14, 1).Style.Font.SetBold();
                ws.Cell(14, 1).Style.Font.FontSize = 10;
                ws.Cell(14, 1).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 1).Value = "P/A/P/ ALLOTMENT CLASS/ \n OBJECT OF EXPENDITURE";
                ws.Cell(14, 1).Style.Alignment.WrapText = true;
                ws.Cell(14, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 1), ws.Cell(17, 11)).Merge();
                ws.Rows(14, 19).Height = 16;
                ws.Columns(1, 11).Width = 3.1;

                // Merge a range
                ws.Cell(14, 12).Style.Font.SetBold();
                ws.Cell(14, 12).Style.Font.FontSize = 10;
                ws.Cell(14, 12).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 12).Value = "EXPENSES \n CODE";
                ws.Cell(14, 12).Style.Alignment.WrapText = true;
                ws.Cell(14, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 12).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 12), ws.Cell(17, 12)).Merge();
                ws.Column(12).Width = 12;

                // Merge a range
                ws.Cell(14, 13).Style.Font.SetBold();
                ws.Cell(14, 13).Style.Font.FontSize = 10;
                ws.Cell(14, 13).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 13).Value = "ALLOTMENT \n RECEIVED";
                ws.Cell(14, 13).Style.Alignment.WrapText = true;
                ws.Cell(14, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 13).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 13), ws.Cell(17, 13)).Merge();
                ws.Column(13).Width = 20;

                // Merge a range
                ws.Cell(14, 14).Style.Font.SetBold();
                ws.Cell(14, 14).Style.Font.FontSize = 10;
                ws.Cell(14, 14).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 14).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(14, 14).Value = "TRANSFER FROM \n CENTRAL OFFICE \n (2022)";
                ws.Cell(14, 14).Style.Alignment.WrapText = true;
                ws.Cell(14, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 14).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 14), ws.Cell(17, 14)).Merge();
                ws.Column(14).Width = 18;

                // Merge a range
                ws.Cell(14, 15).Style.Font.SetBold();
                ws.Cell(14, 15).Style.Font.FontSize = 10;
                ws.Cell(14, 15).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 15).Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Cell(14, 15).Value = "NEGATIVE SAA \n FROM CENTRAL \n OFFICE (2022)";
                ws.Cell(14, 15).Style.Alignment.WrapText = true;
                ws.Cell(14, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 15), ws.Cell(17, 15)).Merge();
                ws.Column(15).Width = 18;

                // Merge a range
                ws.Cell(14, 16).Style.Font.SetBold();
                ws.Cell(14, 16).Style.Font.FontSize = 10;
                ws.Cell(14, 16).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 16).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(14, 16).Value = "TOTAL SAA";
                ws.Cell(14, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 16).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 16), ws.Cell(17, 16)).Merge();
                ws.Column(16).Width = 18;

                // Merge a range
                ws.Cell(14, 17).Style.Font.SetBold();
                ws.Cell(14, 17).Style.Font.FontSize = 10;
                ws.Cell(14, 17).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 17).Value = "REALIGNMENT";
                ws.Cell(14, 17).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 17).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 17), ws.Cell(17, 17)).Merge();
                ws.Column(17).Width = 18;

                // Merge a range
                ws.Cell(14, 18).Style.Font.SetBold();
                ws.Cell(14, 18).Style.Font.FontSize = 10;
                ws.Cell(14, 18).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 18).Value = "TRANSFER \n TO \n RETAINED \n HOSPITAL";
                ws.Cell(14, 18).Style.Alignment.WrapText = true;
                ws.Cell(14, 18).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 18).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 18), ws.Cell(17, 18)).Merge();
                ws.Column(18).Width = 18;

                // Merge a range
                ws.Cell(14, 19).Style.Font.SetBold();
                ws.Cell(14, 19).Style.Font.FontSize = 10;
                ws.Cell(14, 19).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 19).Value = "TOTAL ADJUSTED \n ALLOTMENT";
                ws.Cell(14, 19).Style.Alignment.WrapText = true;
                ws.Cell(14, 19).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 19).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 19), ws.Cell(17, 19)).Merge();
                ws.Column(19).Width = 20;

                // Merge a range
                ws.Cell(14, 20).Style.Font.SetBold();
                ws.Cell(14, 20).Style.Font.FontSize = 10;
                ws.Cell(14, 20).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 20).Value = "OBLIGATIONS INCURRED";
                ws.Cell(14, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                ws.Cell(14, 20).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 20).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 20), ws.Cell(14, 21)).Merge();
                ws.Column(20).Width = 18;

                // Merge a range
                ws.Cell(15, 20).Style.Font.SetBold();
                ws.Cell(15, 20).Style.Font.FontSize = 10;
                ws.Cell(15, 20).Style.Font.FontName = "Lucida Bright";
                ws.Cell(15, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                ws.Cell(15, 20).Value = "FOR THE MONTH" + "\n" +  date2.ToString("MMMM").ToUpper();
                ws.Cell(15, 20).Style.Alignment.WrapText = true;
                ws.Cell(15, 20).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(15, 20).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(15, 20), ws.Cell(17, 20)).Merge();
                ws.Column(20).Width = 18;

                // Merge a range
                ws.Cell(15, 21).Style.Font.SetBold();
                ws.Cell(15, 21).Style.Font.FontSize = 10;
                ws.Cell(15, 21).Style.Font.FontName = "Lucida Bright";
                ws.Cell(15, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                ws.Cell(15, 21).Value = "AS AT" + " " + date2.ToString("MMMM").ToUpper();
                ws.Cell(15, 21).Style.Alignment.WrapText = true;
                ws.Cell(15, 21).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(15, 21).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(15, 21), ws.Cell(17, 21)).Merge();
                ws.Column(21).Width = 18;

                // Merge a range
                ws.Cell(14, 22).Style.Font.SetBold();
                ws.Cell(14, 22).Style.Font.FontSize = 10;
                ws.Cell(14, 22).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 22).Value = "UNOBLIGATED \n BALANCE OF \n ALLOTMENT";
                ws.Cell(14, 22).Style.Alignment.WrapText = true;
                ws.Cell(14, 22).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 22).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 22), ws.Cell(17, 22)).Merge();
                ws.Column(22).Width = 18;


                // Merge a range
                ws.Cell(14, 23).Style.Font.SetBold();
                ws.Cell(14, 23).Style.Font.FontSize = 10;
                ws.Cell(14, 23).Style.Font.FontName = "Lucida Bright";
                ws.Cell(14, 23).Value = "% \n OBLIGATIONS \n ALLOTMENT";
                ws.Cell(14, 23).Style.Alignment.WrapText = true;
                ws.Cell(14, 23).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(14, 23).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(14, 23), ws.Cell(17, 23)).Merge();
                ws.Column(23).Width = 18;

                // Merge a range
                ws.Cell(19, 1).Style.Font.SetBold();
                ws.Cell(19, 1).Style.Font.FontSize = 10;
                ws.Cell(19, 1).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 1).Value = "(1)";
                ws.Cell(19, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range(ws.Cell(19, 1), ws.Cell(19, 11)).Merge();

                // Merge a range
                ws.Cell(19, 12).Style.Font.SetBold();
                ws.Cell(19, 12).Style.Font.FontSize = 10;
                ws.Cell(19, 12).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 12).Value = "(2)";
                ws.Cell(19, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 12).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 13).Style.Font.SetBold();
                ws.Cell(19, 13).Style.Font.FontSize = 10;
                ws.Cell(19, 13).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 13).Value = "(3)";
                ws.Cell(19, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 13).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 14).Style.Font.SetBold();
                ws.Cell(19, 14).Style.Font.FontSize = 10;
                ws.Cell(19, 14).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 14).Value = "(4)";
                ws.Cell(19, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 14).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 15).Style.Font.SetBold();
                ws.Cell(19, 15).Style.Font.FontSize = 10;
                ws.Cell(19, 15).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 15).Value = "(5)";
                ws.Cell(19, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 16).Style.Font.SetBold();
                ws.Cell(19, 16).Style.Font.FontSize = 10;
                ws.Cell(19, 16).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 16).Value = "(7) =  (5) + (6)";
                ws.Cell(19, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 16).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 17).Style.Font.SetBold();
                ws.Cell(19, 17).Style.Font.FontSize = 10;
                ws.Cell(19, 17).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 17).Value = "(8)";
                ws.Cell(19, 17).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 17).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 18).Style.Font.SetBold();
                ws.Cell(19, 18).Style.Font.FontSize = 10;
                ws.Cell(19, 18).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 18).Value = "(9)";
                ws.Cell(19, 18).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 18).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Merge a range
                ws.Cell(19, 19).Style.Font.SetBold();
                ws.Cell(19, 19).Style.Font.FontSize = 10;
                ws.Cell(19, 19).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 19).Value = "(10) = (3) + (7) + (8) + (9)";

                // Merge a range
                ws.Cell(19, 20).Style.Font.SetBold();
                ws.Cell(19, 20).Style.Font.FontSize = 10;
                ws.Cell(19, 20).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 20).Value = "(11)";
                ws.Cell(19, 20).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 20).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cell(19, 21).Style.Font.SetBold();
                ws.Cell(19, 21).Style.Font.FontSize = 10;
                ws.Cell(19, 21).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 21).Value = "(12)";
                ws.Cell(19, 21).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 21).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cell(19, 22).Style.Font.SetBold();
                ws.Cell(19, 22).Style.Font.FontSize = 10;
                ws.Cell(19, 22).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 22).Value = "(13) = (10) - (12)";
                ws.Cell(19, 22).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 22).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cell(19, 23).Style.Font.SetBold();
                ws.Cell(19, 23).Style.Font.FontSize = 10;
                ws.Cell(19, 23).Style.Font.FontName = "Lucida Bright";
                ws.Cell(19, 23).Value = "(14) = (12) / (10)";
                ws.Cell(19, 23).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(19, 23).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == id).YearlyReference;
                DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
                var res = next_year.AddYears(-1);
                var result = res.Year.ToString();


                var budget_allotments = _MyDbContext.Budget_allotments
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(Allotment_class => Allotment_class.AllotmentClass)
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(a => a.Appropriation)
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                            .ThenInclude(uacs => uacs.Uacs)
                    .Include(sub_allotment => sub_allotment.SubAllotment)
                    .Where(x => x.YearlyReferenceId == id/* || x.Yearly_reference.YearlyReference == result*/).ToList();

                // var realignment_amount = 50;

                var allotmentClassId = (from f in _MyDbContext.FundSources
                                        join a in _MyDbContext.AllotmentClass
                                        on f.AllotmentClassId equals a.Id
                                        select new
                                        {
                                            AllotmentClassID = a.Id,
                                            AppropriationID = f.AppropriationId
                                        }).FirstOrDefault();




                foreach (BudgetAllotment budget_allotment in budget_allotments)
                {

                    ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 16;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 1).Value = "CURRENT APPROPRIATION";
                    currentRow++;


                    if (_MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.BudgetAllotmentId == id).Any())
                    {

                        ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 1).Value = "Personnel Services".ToUpper();
                        currentRow++;
                    }



                    //START PS LOOP

                    foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.FundSourceTitle != "AUTOMATIC APPROPRIATION" && x.BudgetAllotmentId == id).ToList())
                    {
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                        currentRow++;

                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.OrderBy(x => x.UacsId).Where(x => x.status == "activated").ToList())
                        {
                            var uacsID = from fa in _MyDbContext.FundSourceAmount
                                         join u in _MyDbContext.Uacs
                                         on fa.UacsId equals u.UacsId
                                         select fa.UacsId;


                            var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                               join o in _MyDbContext.Obligation
                                               on oa.ObligationId equals o.Id
                                               where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                               select new
                                               {
                                                   amount = oa.Amount,
                                                   uacsId = oa.UacsId,
                                                   date = o.Date,
                                                   sourceId = o.FundSourceId,
                                                   status = o.status

                                               }).ToList();

                            var fundsourceID = (from f in _MyDbContext.FundSources
                                                join fa in _MyDbContext.FundSourceAmount
                                                on f.FundSourceId equals fa.FundSourceId
                                                where f.FundSourceId == fa.FundSourceId
                                                select new
                                                {
                                                    faId = f.FundSourceId,
                                                    faBeginningBalance = fa.beginning_balance,
                                                    uacsID = fa.UacsId
                                                }).ToList();

                            var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                      join fa in _MyDbContext.FundSourceAmount
                                                      on f.FundSourceId equals fa.FundSourceId
                                                      where f.FundSourceId == fa.FundSourceId
                                                      select new
                                                      {
                                                          faAmountId = fa.FundSourceId
                                                      }).ToList();




                            var asAt = (from oa in _MyDbContext.ObligationAmount
                                        join o in _MyDbContext.Obligation
                                        on oa.ObligationId equals o.Id
                                        where o.Date >= date1 && o.Date <= date2
                                        select new
                                        {
                                            amount = oa.Amount,
                                            uacsId = oa.UacsId,
                                            sourceId = o.FundSourceId,
                                            status = o.status
                                        }).ToList();
                            var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                            total = 0;

                            ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                            //ws.Cell(currentRow, 3).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            if (fundsource_amount.beginning_balance != 0)
                            {
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Value = fundsource_amount.beginning_balance;
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Value = "-";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }



                            if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                            {
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId &&x.FundSourceId == fundsource_amount.FundSourceId).Any())
                            {
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Value = "-";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "-";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                            var afterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                            if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || afterrealignment_amount != 0)
                            {
                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Value = afterrealignment_amount;
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                            {
                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Value = afterrealignment_amountadd;
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (afterrealignment_amount ==0 || afterrealignment_amountadd == 0)
                            {
                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Value = fundsource_amount.beginning_balance;
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (fundsource_amount.beginning_balance == 0)
                            {
                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Value = "-";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }

                            if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                            {
                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Value = "-";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                            {
                                //OBLIGATED (AS AT)
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                //OBLIGATED (AS AT)
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Value = "-";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }


                            var addunobligated = afterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                            var deductunobligated = afterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                            double zero = 0.00;

                            if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || addunobligated != 0)
                            {
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Value = addunobligated;
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                            {
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Value = deductunobligated;
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (addunobligated == 0 || deductunobligated == 0)
                            {
                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Value = unobligated_amount;
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            if (unobligated_amount == 0)
                            {
                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Value = "-";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }


                            //PERCENT OF UTILIZATION
                            if (/*asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) == 0 || */afterrealignment_amount == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "-";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {

                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                var percentTotal = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) / afterrealignment_amount;
                                ws.Cell(currentRow, 23).Value = percentTotal;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }

                            //REALIGNMENT DATA
                            var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                            foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data).ToList())
                            {
                                currentRow++;
                                Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                if (_MyDbContext.FundSourceAmount.Where(x => x.UacsId == realignment.Realignment_to).FirstOrDefault()?.beginning_balance == null)
                                {
                                    ws.Cell(currentRow, 3).Value = "-";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 3).Value = _MyDbContext.FundSourceAmount.FirstOrDefault(x => x.UacsId == realignment.Realignment_to)?.beginning_balance;
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TRANSFER TO AMOUNT
                                ws.Cell(currentRow, 5).Value = "-";
                                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //ADJUSTED ALLOTMENT
                                if (_MyDbContext.FundSourceAmount.Where(x => x.UacsId == realignment.Realignment_to).FirstOrDefault()?.beginning_balance == null)
                                {
                                    ws.Cell(currentRow, 6).Value = "-";
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 6).Value = _MyDbContext.FundSourceAmount.FirstOrDefault(x => x.UacsId == realignment.Realignment_to)?.beginning_balance;
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                //REALIGNMENT - FOR THE MONTH
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = /*fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);*/"-";

                                //REALIGNMENT - AS AT
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = /*fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);*/"-";

                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = /*fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);*/"-";

                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 10).Value = /*fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);*/"-";
                            }
                            currentRow++;
                            total = (double)fundsource_amount.beginning_balance;
                        }

                        var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    date = o.Date,
                                                    status = o.status
                                                }).ToList();

                        var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                 join fa in _MyDbContext.FundSourceAmount
                                                 on f.FundSourceId equals fa.FundSourceId
                                                 select new
                                                 {
                                                     Id = f.FundSourceId
                                                 }).ToList();

                        var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                         join o in _MyDbContext.Obligation
                                         on oa.ObligationId equals o.Id
                                         where o.Date >= date1 && o.Date <= date2
                                         select new
                                         {
                                             amount = oa.Amount,
                                             sourceId = o.FundSourceId,
                                             uacsId = oa.UacsId,
                                             status = o.status
                                         }).ToList();

                        var sub6 = fundSource.Beginning_balance - fundSource.FundsRealignment?.Sum(x => x.Realignment_amount) + fundSource.FundsRealignment?.Sum(x => x.Realignment_amount);
                        var sub9 = sub6 - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts?.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                        ws.Cell(currentRow, 4).Style.Font.SetBold();
                        ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 4).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper();

                        ws.Cell(currentRow, 13).Style.Font.SetBold();
                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 13).Value = fundSource.Beginning_balance;

                        //REALIGNMENT SUBTOTAL
                        var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                        if (realignment_subtotal == null)
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Value = "0.00";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        }
                        else
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 17).Value = realignment_subtotal;
                        }


                        ws.Cell(currentRow, 18).Style.Font.SetBold();
                        ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 18).Value = "0.00";
                        ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        if (string.IsNullOrEmpty(fundSource.FundsRealignment?.Sum(x => x.Realignment_amount).ToString()))
                        {
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = fundSource.Beginning_balance;
                        }
                        else
                        {
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = sub6;
                        }


                        ws.Cell(currentRow, 20).Style.Font.SetBold();
                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                        //AS AT TOTAL
                        ws.Cell(currentRow, 21).Style.Font.SetBold();
                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                        if (string.IsNullOrEmpty(fundSource.FundsRealignment?.Sum(x => x.Realignment_amount).ToString()))
                        {
                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotal;
                        }
                        else
                        {
                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = sub9;
                        }

                        if (string.IsNullOrEmpty(fundSource.FundsRealignment?.Sum(x => x.Realignment_amount).ToString()))
                        {
                            if(asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) == 0)
                            {
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Value = "-";
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            
                        }
                        else
                        {
                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / sub9;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        }


                        var subAllotmentTotal = _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance);

                        allotment_total += (double)fundSource.Beginning_balance + (double)subAllotmentTotal;
                        currentRow++;




                    }
                    if (_MyDbContext.FundSources.Where(x => x.AppropriationId ==1 && x.AllotmentClassId == 1 && x.BudgetAllotmentId == id).Any())
                    {
                        ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 11).Style.Font.SetBold();
                        ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 11).Value = "TOTAL PERSONNEL SERVICES";

                        var PTC = budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                        //var PsTotalCurrent = +(double)PTC;


                        ws.Cell(currentRow, 13).Style.Font.SetBold();
                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 13).Value = PsTotal;

                        //REALIGNMENT TOTAL
                        var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                        if (realignment_total == null)
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 17).Value = 0.00;
                        }
                        else
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 17).Value = realignment_total;
                        }


                        //TOTAL TRANSFER TO
                        ws.Cell(currentRow, 18).Style.Font.SetBold();
                        ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 18).Value = "0.00";
                        ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 19).Style.Font.SetBold();
                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 19).Value = PsTotal;

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 20).Style.Font.SetBold();
                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 21).Style.Font.SetBold();
                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalPS = PsTotal - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                        ws.Cell(currentRow, 22).Style.Font.SetBold();
                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPS;
                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";

                        //PERCENT OF UTILIZATION
                        var totalPercentPS = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;
                        ws.Cell(currentRow, 23).Value = totalPercentPS;
                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 23).Style.Font.SetBold();
                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";

                        currentRow++;
                        //END PS LOOP
                    }
                    //START AUTOMATIC APPROPRIATION PS LOOP
                    foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").ToList())
                    {



                        ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                        currentRow++;

                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.OrderByDescending(x => x.UacsId).Where(x => x.status == "activated").ToList())
                        {
                            var uacsID = from fa in _MyDbContext.FundSourceAmount
                                         join u in _MyDbContext.Uacs
                                         on fa.UacsId equals u.UacsId
                                         select fa.UacsId;


                            var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                               join o in _MyDbContext.Obligation
                                               on oa.ObligationId equals o.Id
                                               where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                               select new
                                               {
                                                   amount = oa.Amount,
                                                   uacsId = oa.UacsId,
                                                   date = o.Date,
                                                   sourceId = o.FundSourceId,
                                                   status = o.status

                                               }).ToList();

                            var fundsourceID = (from f in _MyDbContext.FundSources
                                                join fa in _MyDbContext.FundSourceAmount
                                                on f.FundSourceId equals fa.FundSourceId
                                                where f.FundSourceId == fa.FundSourceId
                                                select new
                                                {
                                                    faId = f.FundSourceId
                                                }).ToList();

                            var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                      join fa in _MyDbContext.FundSourceAmount
                                                      on f.FundSourceId equals fa.FundSourceId
                                                      where f.FundSourceId == fa.FundSourceId
                                                      select new
                                                      {
                                                          faAmountId = fa.FundSourceId
                                                      }).ToList();




                            var asAt = (from oa in _MyDbContext.ObligationAmount
                                        join o in _MyDbContext.Obligation
                                        on oa.ObligationId equals o.Id
                                        where o.Date >= date1 && o.Date <= date2
                                        select new
                                        {
                                            amount = oa.Amount,
                                            uacsId = oa.UacsId,
                                            sourceId = o.FundSourceId,
                                            status = o.status
                                        }).ToList();

                            var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                            total = 0;
                            var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                            ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance;
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            if (fundsource_amount.realignment_amount == 0)
                            {
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "-";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "-";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL ADJUSTED ALLOTMENT
                            ws.Cell(currentRow, 6).Value = afterrealignment_amount;
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (FOR THE MONTH)
                            ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (AS AT)
                            //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount;
                            ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Value = unobligated_amount;
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                            //REALIGNMENT DATA
                            foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).ToList())
                            //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                            {




                                currentRow++;
                                Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = "#,##0.00";
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount;
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                            }
                            currentRow++;
                            total = (double)fundsource_amount.beginning_balance;
                        }

                        var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    date = o.Date,
                                                    status = o.status
                                                }).ToList();

                        var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                 join fa in _MyDbContext.FundSourceAmount
                                                 on f.FundSourceId equals fa.FundSourceId
                                                 select new
                                                 {
                                                     Id = f.FundSourceId
                                                 }).ToList();

                        var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                         join o in _MyDbContext.Obligation
                                         on oa.ObligationId equals o.Id
                                         where o.Date >= date1 && o.Date <= date2
                                         select new
                                         {
                                             amount = oa.Amount,
                                             sourceId = o.FundSourceId,
                                             uacsId = oa.UacsId,
                                             status = o.status
                                         }).ToList();

                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance;

                        //REALIGNMENT TOTAL
                        var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                        if (realignment_total == null)
                        {
                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 4).Value = 0.00;
                        }
                        else
                        {
                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 4).Value = realignment_total;
                        }
                        //TOTAL TRANSFER TO
                        ws.Cell(currentRow, 5).Style.Font.SetBold();
                        ws.Cell(currentRow, 5).Value = "0.00";
                        ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance;

                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                        //AS AT TOTAL
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                        var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                        //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotal;

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        var subAllotmentTotal = _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance);

                        allotment_total += (double)fundSource.Beginning_balance + (double)subAllotmentTotal;
                        currentRow++;
                    }

                    if (_MyDbContext.FundSources.Where(x => x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Any())
                    {
                        var PsTotalAP = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION" && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                        ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF1DE");
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 1).Value = "TOTAL AUTOMATIC APPROPRIATIONS";

                        var PTCAP = budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                        //var PsTotalCurrentAP = +(double)PTC;


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = PsTotalAP;

                        //REALIGNMENT TOTAL
                        var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                        if (realignment_total == null)
                        {
                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 4).Value = 0.00;
                        }
                        else
                        {
                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 4).Value = realignment_total;
                        }
                        //TOTAL TRANSFER TO
                        ws.Cell(currentRow, 5).Style.Font.SetBold();
                        ws.Cell(currentRow, 5).Value = "0.00";
                        ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = PsTotalAP;

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalPSAP = PsTotalAP - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSAP;

                        //PERCENT OF UTILIZATION
                        var totalPercentPSAP = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;
                        ws.Cell(currentRow, 10).Value = totalPercentPSAP;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }
                    //END AUTOMATIC APPROPRIATION PS LOOP

                    var saa = _MyDbContext.Budget_allotments
                        .Include(sub_allotment => sub_allotment.SubAllotment)
                        .ThenInclude(suballotment_amount => suballotment_amount.SubAllotmentAmounts)
                        .ThenInclude(uacs => uacs.Uacs).Where(x => x.YearlyReferenceId == id);


                    foreach (BudgetAllotment b in saa)
                    {

                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "Current Saa Personnel Services".ToUpper();
                            currentRow++;
                        }

                        //START SAA PS LOOP
                        foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1))
                        {
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                            currentRow++;

                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title?.ToUpper().ToString();

                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                            ws.Column(11).Width = 15;
                            currentRow++;

                            ws.Cell(currentRow, 2).Style.Font.SetItalic();
                            ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 2).Value = subAllotment.Description?.ToString();
                            ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                            currentRow++;


                            foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.OrderBy(x => x.UacsId).Where(x => x.status == "activated"))
                            {
                                var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                             join u in _MyDbContext.Uacs
                                             on Suballotment.UacsId equals u.UacsId
                                             select Suballotment.UacsId;


                                var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                   join o in _MyDbContext.Obligation
                                                   on oa.ObligationId equals o.Id
                                                   join f in _MyDbContext.FundSources
                                                   on o.FundSourceId equals f.FundSourceId
                                                   where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                   select new
                                                   {
                                                       amount = oa.Amount,
                                                       uacsId = oa.UacsId,
                                                       date = o.Date,
                                                       sourceId = o.FundSourceId,
                                                       sourceType = o.source_type,
                                                       status = o.status,
                                                       allotmentClassID = f.AllotmentClassId

                                                   }).ToList();

                                var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                    join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                    on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                    where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                    select new
                                                    {
                                                        saId = Suballotment.SubAllotmentId
                                                    }).ToList();

                                var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                          join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                          on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                          where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                          select new
                                                          {
                                                              saAmountId = Suballotment_amount.SubAllotmentId
                                                          }).ToList();

                                var asAt = (from oa in _MyDbContext.ObligationAmount
                                            join o in _MyDbContext.Obligation
                                            on oa.ObligationId equals o.Id
                                            where o.Date >= date1 && o.Date <= date2
                                            select new
                                            {
                                                amount = oa.Amount,
                                                uacsId = oa.UacsId,
                                                sourceId = o.SubAllotmentId,
                                                sourceType = o.source_type,
                                                status = o.status
                                            }).ToList();

                                var SAAunobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                total = 0;
                                var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 4).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                if (suballotment_amount.beginning_balance != 0)
                                {
                                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 13).Value = suballotment_amount.beginning_balance;
                                    ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 13).Value = "-";
                                    ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }


                                //REALIGNMENT SAA
                                if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                {
                                    //REALIGNMENT SAA AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                {
                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "-";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "-";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                var SAAafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                var SAAafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || afterrealignment_amount != 0)
                                {
                                    //TOTAL ADJUSTED ALLOTMENT SAA
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = SAAafterrealignment_amount;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                {
                                    //TOTAL ADJUSTED ALLOTMENT SAA
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = SAAafterrealignment_amountadd;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (SAAafterrealignment_amount == 0 || SAAafterrealignment_amountadd == 0)
                                {
                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (suballotment_amount.beginning_balance == 0)
                                {
                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = "-";
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                {
                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                    ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 20).Value = "-";
                                    ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                {
                                    //OBLIGATED (AS AT)
                                    ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                    ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                    ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    //OBLIGATED (AS AT)
                                    ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                    ws.Cell(currentRow, 21).Value = "-";
                                    ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                var SAAaddunobligated = SAAafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                var SAAdeductunobligated = SAAafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                double zero = 0.00;

                                if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || SAAaddunobligated != 0)
                                {
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = SAAaddunobligated;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                {
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = SAAdeductunobligated;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (SAAaddunobligated == 0 || SAAdeductunobligated == 0)
                                {
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = SAAunobligated_amount;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (SAAunobligated_amount == 0)
                                {
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = "-";
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                //PERCENT OF UTILIZATION
                                if (SAAafterrealignment_amount == 0)
                                {
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = "-";
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) / SAAafterrealignment_amount;
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                //REALIGNMENT DATA SUB ALLOTMENT
                                var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                {
                                    currentRow++;
                                    Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                    //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 6).Value = "";
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                currentRow++;
                                total = (double)suballotment_amount.beginning_balance;
                            }

                            var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                    join o in _MyDbContext.Obligation
                                                    on oa.ObligationId equals o.Id
                                                    where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                    select new
                                                    {
                                                        amount = oa.Amount,
                                                        uacsId = oa.UacsId,
                                                        sourceId = o.SubAllotmentId,
                                                        sourceType = o.source_type,
                                                        date = o.Date,
                                                        status = o.status
                                                    }).ToList();

                            var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                     join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                     on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                     select new
                                                     {
                                                         Id = Suballotment.SubAllotmentId
                                                     }).ToList();

                            var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                             join o in _MyDbContext.Obligation
                                             on oa.ObligationId equals o.Id
                                             where o.Date >= date1 && o.Date <= date2
                                             select new
                                             {
                                                 amount = oa.Amount,
                                                 sourceId = o.SubAllotmentId,
                                                 uacsId = oa.UacsId,
                                                 sourceType = o.source_type,
                                                 status = o.status
                                             }).ToList();


                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title?.ToUpper();

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = subAllotment.Beginning_balance;

                            //REALIGNMENT SUBTOTAL
                            var SAArealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                            var SAAsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                            //var SAAsub9 = SAAsub6 - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                            if (SAArealignment_subtotal == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Value = "0.00";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = SAArealignment_subtotal;
                            }

                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            if (string.IsNullOrEmpty(subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount).ToString()))
                            {
                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;
                            }
                            else
                            {
                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = SAAsub6;
                            }

                            if(fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment") == null)
                            {
                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = "-";
                            }
                            else
                            {
                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            }
                            

                            //AS AT TOTAL
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                            var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotal;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault()?.FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            allotment_totalSaa += (double)subAllotment.Beginning_balance;

                            currentRow++;

                        }



                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId ==1 && x.AllotmentClassId == 1 && x.BudgetAllotmentId == id).Any())
                        {
                            //START SAA PS LOOP
                            ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#C4D79B");
                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL PERSONNEL SERVICES SAA";


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = PsTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }


                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = PsTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPSSaa;

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotal = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentPSSaaTotal;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            //END SAA PS LOOP
                        }


                        //

                        //START MOOE LOOP
                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "Maintenance and Other Operating Expenses".ToUpper();
                            currentRow++;

                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1).ToList())
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                                currentRow++;

                                foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x => x.status == "activated").ToList())
                                {
                                    var uacsID = from fa in _MyDbContext.FundSourceAmount
                                                 join u in _MyDbContext.Uacs
                                                 on fa.UacsId equals u.UacsId
                                                 select fa.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           status = o.status

                                                       }).ToList();

                                    var fundsourceID = (from f in _MyDbContext.FundSources
                                                        join fa in _MyDbContext.FundSourceAmount
                                                        on f.FundSourceId equals fa.FundSourceId
                                                        where f.FundSourceId == fa.FundSourceId
                                                        select new
                                                        {
                                                            faId = f.FundSourceId
                                                        }).ToList();

                                    var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                              join fa in _MyDbContext.FundSourceAmount
                                                              on f.FundSourceId equals fa.FundSourceId
                                                              where f.FundSourceId == fa.FundSourceId
                                                              select new
                                                              {
                                                                  faAmountId = fa.FundSourceId
                                                              }).ToList();




                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (fundsource_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var MOOEafterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    var MOOEafterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || MOOEafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = MOOEafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = MOOEafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (MOOEafterrealignment_amount == 0 || MOOEafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (fundsource_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //OBLIGATED (FOR THE MONTH)
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //OBLIGATED (AS AT)
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    var MOOEaddunobligated = MOOEafterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    var MOOEdeductunobligated = MOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || MOOEaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = MOOEaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = MOOEdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (MOOEaddunobligated == 0 || MOOEdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //PERCENT OF UTILIZATION
                                    if (afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //REALIGNMENT DATA
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)fundsource_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.FundSourceId,
                                                            date = o.Date,
                                                            status = o.status

                                                        }).ToList();

                                var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                         join fa in _MyDbContext.FundSourceAmount
                                                         on f.FundSourceId equals fa.FundSourceId
                                                         select new
                                                         {
                                                             Id = f.FundSourceId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.FundSourceId,
                                                     uacsId = oa.UacsId,
                                                     status = o.status
                                                 }).ToList();

                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper();
                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = fundSource.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = realignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = fundSource.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_total += (double)fundSource.Beginning_balance;

                                currentRow++;




                            }

                            if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                            {
                                ws.Cell(currentRow, 11).Style.Font.SetBold();
                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 11).Value = "TOTAL MOOE";

                                /*var fortheMonthTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                                                   join o in _MyDbContext.Obligation
                                                                   on oa.ObligationId equals o.Id
                                                                   join f in _MyDbContext.FundSources
                                                                   on o.FundSourceId equals f.FundSourceId
                                                                   where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                                   select new
                                                                   {
                                                                       amount = oa.Amount,
                                                                       uacsId = oa.UacsId,
                                                                       sourceId = o.FundSourceId,
                                                                       date = o.Date,
                                                                       status = o.status,
                                                                       allotmentClassID = f.AllotmentClassId
                                                                   }).ToList();

                                var asAtTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                                            join o in _MyDbContext.Obligation
                                                            on oa.ObligationId equals o.Id
                                                            join f in _MyDbContext.FundSources
                                                            on o.FundSourceId equals f.FundSourceId
                                                            where o.Date >= date1 && o.Date <= date2
                                                            select new
                                                            {
                                                                amount = oa.Amount,
                                                                sourceId = o.FundSourceId,
                                                                uacsId = oa.UacsId,
                                                                status = o.status,
                                                                allotmentClassID = f.AllotmentClassId
                                                            }).ToList();*/

                                var MooeTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                                var allotment_totalMOOE = +MooeTotal;

                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = MooeTotal;

                                //REALIGNMENT TOTAL
                                var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_total == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = 0.00;
                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = realignment_total;
                                }

                                //TOTAL TRANSFER TO
                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL - TOTAL AFTER REALIGNMENT
                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = MooeTotal;

                                //TOTAL - FOR THE MONTH
                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.status == "activated").Sum(x => x.amount);

                                //TOTAL - AS AT
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2 && x.status == "activated").Sum(x => x.amount);

                                //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                                var unobligatedTotalinTotalMOOE = MooeTotal - asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount);
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOE;


                                //PERCENT OF UTILIZATION
                                if (MooeTotal == 0 && asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) == 0)
                                {
                                    ws.Cell(currentRow, 23).Style.Font.SetBold();
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = "";
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    currentRow++;
                                }
                                else
                                {
                                    var totalPercentMOOE = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) / MooeTotal;
                                    ws.Cell(currentRow, 23).Style.Font.SetBold();
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = totalPercentMOOE;
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    currentRow++;
                                }
                                //END MOOE LOOP
                            }

                        }

                        //START SAA MOOE LOOP
                        if (budget_allotment.SubAllotment.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 2).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CURRENT MOOE SUB-ALLOTMENT";
                            currentRow++;

                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title?.ToUpper().ToString();

                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 4).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var SAAMOOEafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var SAAMOOEafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || afterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = SAAMOOEafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = SAAMOOEafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (SAAMOOEafterrealignment_amount == 0 || SAAMOOEafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        //OBLIGATED (FOR THE MONTH)
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //OBLIGATED (FOR THE MONTH)
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        //OBLIGATED (AS AT)
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //OBLIGATED (AS AT)
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2DCDB");
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    var SAAMOOEaddunobligated = SAAMOOEafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var SAAMOOEdeductunobligated = SAAMOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || SAAMOOEaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = SAAMOOEaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = SAAMOOEdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (SAAMOOEaddunobligated == 0 || SAAMOOEdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL" + " " + subAllotment.Suballotment_title?.ToUpper();


                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var SAAMOOErealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var SAAMOOEsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var SAAMOOEsub9 = SAAMOOEsub6 - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (SAAMOOErealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = SAAMOOErealignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL MOOE SAA";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }


                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END SAA MOOE LOOP
                        }


                        //
                        //START CO LOOP
                        if (_MyDbContext.FundSources.Where(x => x.AllotmentClassId == 3).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "Capital Outlay";
                            currentRow++;

                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                                currentRow++;

                                foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from fa in _MyDbContext.FundSourceAmount
                                                 join u in _MyDbContext.Uacs
                                                 on fa.UacsId equals u.UacsId
                                                 select fa.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           status = o.status

                                                       }).ToList();

                                    var fundsourceID = (from f in _MyDbContext.FundSources
                                                        join fa in _MyDbContext.FundSourceAmount
                                                        on f.FundSourceId equals fa.FundSourceId
                                                        where f.FundSourceId == fa.FundSourceId
                                                        select new
                                                        {
                                                            faId = f.FundSourceId
                                                        }).ToList();

                                    var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                              join fa in _MyDbContext.FundSourceAmount
                                                              on f.FundSourceId equals fa.FundSourceId
                                                              where f.FundSourceId == fa.FundSourceId
                                                              select new
                                                              {
                                                                  faAmountId = fa.FundSourceId
                                                              }).ToList();




                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 4).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    if (fundsource_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var COafterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    var COafterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || COafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = COafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = COafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (COafterrealignment_amount == 0 || COafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (fundsource_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    var COaddunobligated = COafterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    var COdeductunobligated = COafterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || COaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = COaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = COdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (COaddunobligated == 0 || COdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) != 0 || afterrealignment_amount != 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT DATA REGULAR
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)fundsource_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.FundSourceId,
                                                            date = o.Date,
                                                            status = o.status

                                                        }).ToList();

                                var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                         join fa in _MyDbContext.FundSourceAmount
                                                         on f.FundSourceId equals fa.FundSourceId
                                                         select new
                                                         {
                                                             Id = f.FundSourceId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.FundSourceId,
                                                     uacsId = oa.UacsId,
                                                     status = o.status
                                                 }).ToList();

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper();
                                ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Value = "0.00";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 4).Value = realignment_subtotal;
                                }
                                ws.Cell(currentRow, 5).Style.Font.SetBold();
                                ws.Cell(currentRow, 5).Value = "0.00";
                                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance;

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_total += (double)fundSource.Beginning_balance;

                                currentRow++;


                            }

                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CO";


                            /*var fortheMonthTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                                             join o in _MyDbContext.Obligation
                                                             on oa.ObligationId equals o.Id
                                                             join f in _MyDbContext.FundSources
                                                             on o.FundSourceId equals f.FundSourceId
                                                             where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                             select new
                                                             {
                                                                 amount = oa.Amount,
                                                                 uacsId = oa.UacsId,
                                                                 sourceId = o.FundSourceId,
                                                                 date = o.Date,
                                                                 status = o.status,
                                                                 allotmentClassID = f.AllotmentClassId
                                                             }).ToList();

                            var asAtTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                                      join o in _MyDbContext.Obligation
                                                      on oa.ObligationId equals o.Id
                                                      join f in _MyDbContext.FundSources
                                                      on o.FundSourceId equals f.FundSourceId
                                                      where o.Date >= date1 && o.Date <= date2
                                                      select new
                                                      {
                                                          amount = oa.Amount,
                                                          sourceId = o.FundSourceId,
                                                          uacsId = oa.UacsId,
                                                          status = o.status,
                                                          allotmentClassID = f.AllotmentClassId
                                                      }).ToList();*/

                            var CoTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                            var allotment_totalCO = +CoTotal;

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }


                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalCO = CoTotal - asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCO;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) == 0 && CoTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            {
                                var totalPercentCO = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / CoTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentCO;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                                currentRow++;
                            }

                            //END CO LOOP
                        }
                        //
                        //START SAA CO LOOP
                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 3 && x.BudgetAllotmentId == id).Any())
                        {
                            //START SAA CO LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CURRENT CO SUB-ALLOTMENT";
                            currentRow++;

                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();

                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //TRANSFER TO
                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var SAACOafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var SAACOafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || afterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = SAACOafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = SAACOafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (SAACOafterrealignment_amount == 0 || SAACOafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var SAACOaddunobligated = SAACOafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var SAACOdeductunobligated = SAACOafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || SAACOaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = SAACOaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = SAACOdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (SAACOaddunobligated == 0 || SAACOdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) != 0 || afterrealignment_amount != 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var SAACOrealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var SAACOsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var SAACOsub9 = SAACOsub6 - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (SAACOrealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = SAACOrealignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.BudgetAllotmentId == id).Any())
                            {

                                var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3  && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                                var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                                var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                                ws.Cell(currentRow, 11).Style.Font.SetBold();
                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 11).Value = "TOTAL CO SAA";

                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = CoTotalSaa;

                                //REALIGNMENT TOTAL
                                var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_total == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = 0.00;
                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = realignment_total;
                                }
                                //TOTAL TRANSFER TO
                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL - TOTAL AFTER REALIGNMENT
                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = CoTotalSaa;

                                //TOTAL - FOR THE MONTH
                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //TOTAL - AS AT
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCOSaa;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentCOSaa;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                currentRow++;
                                //END SAA CO LOOP
                            }
                        }

                        //CONAP HEADER
                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2).Any() || _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 16;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CONTINUING APPROPRIATION";
                            currentRow++;
                        }

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "Personnel Services";
                            currentRow++;

                            //START CONAP PS LOOP
                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                                currentRow++;

                                foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from fa in _MyDbContext.FundSourceAmount
                                                 join u in _MyDbContext.Uacs
                                                 on fa.UacsId equals u.UacsId
                                                 select fa.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           status = o.status

                                                       }).ToList();

                                    var fundsourceID = (from f in _MyDbContext.FundSources
                                                        join fa in _MyDbContext.FundSourceAmount
                                                        on f.FundSourceId equals fa.FundSourceId
                                                        where f.FundSourceId == fa.FundSourceId
                                                        select new
                                                        {
                                                            faId = f.FundSourceId
                                                        }).ToList();

                                    var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                              join fa in _MyDbContext.FundSourceAmount
                                                              on f.FundSourceId equals fa.FundSourceId
                                                              where f.FundSourceId == fa.FundSourceId
                                                              select new
                                                              {
                                                                  faAmountId = fa.FundSourceId
                                                              }).ToList();




                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 4).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (fundsource_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var CONAPafterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    var CONAPafterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPafterrealignment_amount == 0 || CONAPafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (fundsource_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPaddunobligated = CONAPafterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPdeductunobligated = CONAPafterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPaddunobligated == 0 || CONAPdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) != 0 || afterrealignment_amount != 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //REALIGNMENT DATA REGULAR
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)fundsource_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.FundSourceId,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                         join fa in _MyDbContext.FundSourceAmount
                                                         on f.FundSourceId equals fa.FundSourceId
                                                         select new
                                                         {
                                                             Id = f.FundSourceId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.FundSourceId,
                                                     uacsId = oa.UacsId,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper();


                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = fundSource.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = realignment_subtotal;
                                }


                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = fundSource.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_total += (double)fundSource.Beginning_balance;

                                currentRow++;
                            }

                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP PERSONNEL SERVICES";


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = PsConapTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = PsConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPSConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                var totalPercentPSConaps = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 23).Value = totalPercentPSConaps;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        //END CONAP PS LOOP

                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId ==2 && x.AllotmentClassId == 1).Any())
                        {//START CONAP SAA PS LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CONAP PS SUB-ALLOTMENT";
                            currentRow++;

                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();

                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.Description?.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 3).Value = "-";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = "-";
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 5).Value = "-";
                                    ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var CONAPSAAafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var CONAPSAAafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 6).Value = CONAPSAAafterrealignment_amount;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 6).Value = CONAPSAAafterrealignment_amountadd;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAafterrealignment_amount == 0 || CONAPSAAafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = "-";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 7).Value = "-";
                                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 8).Value = "-";
                                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPSAAaddunobligated = CONAPSAAafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPSAAdeductunobligated = CONAPSAAafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 9).Value = CONAPSAAaddunobligated;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 9).Value = CONAPSAAdeductunobligated;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAaddunobligated == 0 || CONAPSAAdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 9).Value = unobligated_amount;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 9).Value = "-";
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) != 0 || afterrealignment_amount != 0)
                                    {
                                        ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 10).Value = "-";
                                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var CONAPSAAMOOErealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub9 = CONAPSAAMOOEsub6 - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (CONAPSAAMOOErealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Value = "0.00";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 4).Value = CONAPSAAMOOErealignment_subtotal;
                                }

                                ws.Cell(currentRow, 5).Style.Font.SetBold();
                                ws.Cell(currentRow, 5).Value = "0.00";
                                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP SAA PERSONNEL SERVICES";


                            var PsTotalSaaConapPS = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalPSSaaConap = PsTotalSaaConapPS - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentPSSaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaaConapPS;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaaConapPS;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaaConap;

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotals = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = totalPercentPSSaaTotals;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                                //END CONAP SAA PS LOOP
                            }
                        }


                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "Maintenance and Other Operating Expenses".ToUpper();
                            currentRow++;
                        }
                        //START CONAP MOOE LOOP

                        foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2))
                        {

                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                            currentRow++;

                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                            currentRow++;

                            foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x => x.status == "activated"))
                            {
                                var uacsID = from fa in _MyDbContext.FundSourceAmount
                                             join u in _MyDbContext.Uacs
                                             on fa.UacsId equals u.UacsId
                                             select fa.UacsId;


                                var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                   join o in _MyDbContext.Obligation
                                                   on oa.ObligationId equals o.Id
                                                   where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                   select new
                                                   {
                                                       amount = oa.Amount,
                                                       uacsId = oa.UacsId,
                                                       date = o.Date,
                                                       sourceId = o.FundSourceId,
                                                       status = o.status

                                                   }).ToList();

                                var fundsourceID = (from f in _MyDbContext.FundSources
                                                    join fa in _MyDbContext.FundSourceAmount
                                                    on f.FundSourceId equals fa.FundSourceId
                                                    where f.FundSourceId == fa.FundSourceId
                                                    select new
                                                    {
                                                        faId = f.FundSourceId
                                                    }).ToList();

                                var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                          join fa in _MyDbContext.FundSourceAmount
                                                          on f.FundSourceId equals fa.FundSourceId
                                                          where f.FundSourceId == fa.FundSourceId
                                                          select new
                                                          {
                                                              faAmountId = fa.FundSourceId
                                                          }).ToList();




                                var asAt = (from oa in _MyDbContext.ObligationAmount
                                            join o in _MyDbContext.Obligation
                                            on oa.ObligationId equals o.Id
                                            where o.Date >= date1 && o.Date <= date2
                                            select new
                                            {
                                                amount = oa.Amount,
                                                uacsId = oa.UacsId,
                                                sourceId = o.FundSourceId,
                                                status = o.status
                                            }).ToList();

                                var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                total = 0;
                                var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;

                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();

                                ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                if (fundsource_amount.beginning_balance != 0)
                                {
                                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 13).Value = fundsource_amount.beginning_balance;
                                    ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 13).Value = "-";
                                    ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                {
                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                {
                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "-";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "-";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL ADJUSTED ALLOTMENT
                                var CONAPMOOEafterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                var CONAPMOOEafterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                                //TOTAL ADJUSTED ALLOTMENT
                                if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPMOOEafterrealignment_amount != 0)
                                {
                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = CONAPMOOEafterrealignment_amount;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                {
                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = CONAPMOOEafterrealignment_amountadd;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (CONAPMOOEafterrealignment_amount == 0 || CONAPMOOEafterrealignment_amountadd == 0)
                                {
                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = fundsource_amount.beginning_balance;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Value = afterrealignment_amount;
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (FOR THE MONTH)
                                if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                {
                                    ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 20).Value = "-";
                                    ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                //OBLIGATED (AS AT)
                                if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                {
                                    ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 21).Value = "-";
                                    ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }


                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                var CONAPMOOEaddunobligated = CONAPMOOEafterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                var CONAPMOOEdeductunobligated = CONAPMOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPMOOEaddunobligated != 0)
                                {
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = CONAPMOOEaddunobligated;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                {
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = CONAPMOOEdeductunobligated;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (CONAPMOOEaddunobligated == 0 || CONAPMOOEdeductunobligated == 0)
                                {
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = unobligated_amount;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                if (unobligated_amount == 0)
                                {
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = "-";
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                //PERCENT OF UTILIZATION
                                if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                {
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = "-";
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }

                                //REALIGNMENT DATA REGULAR
                                var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data))
                                {
                                    currentRow++;
                                    Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                    //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 6).Value = "";
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                currentRow++;
                                total = (double)fundsource_amount.beginning_balance;
                            }

                            var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                    join o in _MyDbContext.Obligation
                                                    on oa.ObligationId equals o.Id
                                                    where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                    select new
                                                    {
                                                        amount = oa.Amount,
                                                        uacsId = oa.UacsId,
                                                        sourceId = o.FundSourceId,
                                                        date = o.Date,
                                                        status = o.status
                                                    }).ToList();

                            var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                     join fa in _MyDbContext.FundSourceAmount
                                                     on f.FundSourceId equals fa.FundSourceId
                                                     select new
                                                     {
                                                         Id = f.FundSourceId
                                                     }).ToList();

                            var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                             join o in _MyDbContext.Obligation
                                             on oa.ObligationId equals o.Id
                                             where o.Date >= date1 && o.Date <= date2
                                             select new
                                             {
                                                 amount = oa.Amount,
                                                 sourceId = o.FundSourceId,
                                                 uacsId = oa.UacsId,
                                                 status = o.status
                                             }).ToList();


                            ws.Cell(currentRow, 4).Style.Font.SetBold();
                            ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 4).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = fundSource.Beginning_balance;

                            //REALIGNMENT SUBTOTAL
                            var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_subtotal == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Value = "0.00";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_subtotal;
                            }

                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = fundSource.Beginning_balance;

                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                            //AS AT TOTAL
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotal;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            allotment_total += (double)fundSource.Beginning_balance;

                            currentRow++;




                        }

                        var fortheMonthTotalinTotalMooeConap = (from oa in _MyDbContext.ObligationAmount
                                                                join o in _MyDbContext.Obligation
                                                                on oa.ObligationId equals o.Id
                                                                join f in _MyDbContext.FundSources
                                                                on o.FundSourceId equals f.FundSourceId
                                                                where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                                select new
                                                                {
                                                                    amount = oa.Amount,
                                                                    uacsId = oa.UacsId,
                                                                    sourceId = o.FundSourceId,
                                                                    sourceType = o.source_type,
                                                                    date = o.Date,
                                                                    status = o.status,
                                                                    allotmentClassID = f.AllotmentClassId,
                                                                    appropriationID = f.AppropriationId
                                                                }).ToList();

                        var asAtTotalinTotalMooeConap = (from oa in _MyDbContext.ObligationAmount
                                                         join o in _MyDbContext.Obligation
                                                         on oa.ObligationId equals o.Id
                                                         join f in _MyDbContext.FundSources
                                                         on o.FundSourceId equals f.FundSourceId
                                                         where o.Date >= date1 && o.Date <= date2
                                                         select new
                                                         {
                                                             amount = oa.Amount,
                                                             sourceId = o.FundSourceId,
                                                             sourceType = o.source_type,
                                                             uacsId = oa.UacsId,
                                                             status = o.status,
                                                             allotmentClassID = f.AllotmentClassId,
                                                             appropriationID = f.AppropriationId,
                                                             fundSourceBudgetAllotmentId = f.BudgetAllotmentId
                                                         }).ToList();


                        var MooeConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                        var unobligatedTotalinTotalMooeConap = MooeConapTotal - asAtTotalinTotalMooeConap.Where(x => x.appropriationID == 2 && x.allotmentClassID == 2 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP MOOE";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeConapTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMooeConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentPSConap;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            //END CONAP MOOE LOOP
                        }

                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId ==2 && x.AllotmentClassId == 2).Any())
                        {
                            //START CONAP SAA MOOE LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CONAP MOOE SUB-ALLOTMENT";
                            currentRow++;
                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title?.ToUpper().ToString();

                                

                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description?.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.Where(x=>x.status == "activated"))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var CONAPSAAMOOEafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var CONAPSAAMOOEafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEafterrealignment_amount == 0 || CONAPSAAMOOEafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPSAAMOOEaddunobligated = CONAPSAAMOOEafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPSAAMOOEdeductunobligated = CONAPSAAMOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEaddunobligated == 0 || CONAPSAAMOOEdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //PERCENT OF UTILIZATION
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title?.ToUpper();

                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var CONAPSAAMOOErealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub9 = CONAPSAAMOOEsub6 - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (CONAPSAAMOOErealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = CONAPSAAMOOErealignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault()?.FundSourceAmounts.FirstOrDefault()?.FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaaConap = MooeTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP SAA MOOE";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaaConap;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaaConap;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaaConap;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaaConap;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END CONAP SAA MOOE LOOP
                        }

                        //START IF CONAP MOOE PREVIOUS YEAR WAS CHECKED
                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId ==2 && x.AllotmentClassId == 2 && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.IsAddToNextAllotment == true).Any())
                        {
                            //START CONAP SAA MOOE LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CONAP MOOE SUB-ALLOTMENT FOR CY " + result;
                            currentRow++;
                            foreach (var subAllotment in _MyDbContext.SubAllotment.Where(x=>x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.AppropriationId ==2 && x.AllotmentClassId == 2))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title?.ToUpper().ToString();



                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description?.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (var suballotment_amount in _MyDbContext.Suballotment_amount.Where(x => x.status == "activated" && x.SubAllotment.AppropriationId == 2 && x.SubAllotment.AllotmentClassId == 2 && x.SubAllotment.IsAddToNextAllotment == true && x.SubAllotmentId == subAllotment.SubAllotmentId))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 14).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var CONAPSAAMOOEafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var CONAPSAAMOOEafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEafterrealignment_amount == 0 || CONAPSAAMOOEafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPSAAMOOEaddunobligated = CONAPSAAMOOEafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPSAAMOOEdeductunobligated = CONAPSAAMOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEaddunobligated == 0 || CONAPSAAMOOEdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //PERCENT OF UTILIZATION
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title?.ToUpper() + " FOR CY " + result;

                                ws.Cell(currentRow, 14).Style.Font.SetBold();
                                ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 14).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var CONAPSAAMOOErealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub9 = CONAPSAAMOOEsub6 - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (CONAPSAAMOOErealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = CONAPSAAMOOErealignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault()?.FundSourceAmounts.FirstOrDefault()?.FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AppropriationId == 2 && x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaaConap = MooeTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP SAA MOOE FOR CY " + result;

                            ws.Cell(currentRow, 14).Style.Font.SetBold();
                            ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 14).Value = MooeTotalSaaConap;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaaConap;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaaConap;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaaConap;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END CONAP SAA MOOE LOOP FOR CY RESULT
                        }
                        //END IF CONAP MOOE PREVIOUS YEAR WAS CHECKED

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 3).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "Capital Outlay".ToUpper();
                            currentRow++;
                        }
                        //START CONAP CO LOOP
                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3).Any())
                        {
                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                                //ws.Cell(currentRow, 1).Value = dateTime.FirstOrDefault().FundSourceTitle.ToUpper().ToString();
                                currentRow++;

                                foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from fa in _MyDbContext.FundSourceAmount
                                                 join u in _MyDbContext.Uacs
                                                 on fa.UacsId equals u.UacsId
                                                 select fa.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           status = o.status

                                                       }).ToList();

                                    var fundsourceID = (from f in _MyDbContext.FundSources
                                                        join fa in _MyDbContext.FundSourceAmount
                                                        on f.FundSourceId equals fa.FundSourceId
                                                        where f.FundSourceId == fa.FundSourceId
                                                        select new
                                                        {
                                                            faId = f.FundSourceId
                                                        }).ToList();

                                    var fundsourceamountID = (from f in _MyDbContext.FundSources
                                                              join fa in _MyDbContext.FundSourceAmount
                                                              on f.FundSourceId equals fa.FundSourceId
                                                              where f.FundSourceId == fa.FundSourceId
                                                              select new
                                                              {
                                                                  faAmountId = fa.FundSourceId
                                                              }).ToList();




                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.FundSourceId,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (fundsource_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 3).Value = "-";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = "(" + _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId).FirstOrDefault()?.Realignment_amount + ")";
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = _MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = "-";
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    ws.Cell(currentRow, 5).Value = "-";
                                    ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    var CONAPCOafterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    var CONAPCOafterrealignment_amountadd = fundsource_amount.beginning_balance + _MyDbContext.FundsRealignment.FirstOrDefault(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId)?.Realignment_amount;
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPCOafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = CONAPCOafterrealignment_amount;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = CONAPCOafterrealignment_amountadd;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPCOafterrealignment_amount == 0 || CONAPCOafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = fundsource_amount.beginning_balance;
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (fundsource_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 6).Value = "-";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 7).Value = "-";
                                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 8).Value = "-";
                                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPCOaddunobligated = CONAPCOafterrealignment_amount - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPCOdeductunobligated = CONAPCOafterrealignment_amountadd - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId).Any() || CONAPCOaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 9).Value = CONAPCOaddunobligated;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.FundsRealignment.Where(x => x.Realignment_to == fundsource_amount.UacsId && x.FundSourceId == fundsource_amount.FundSourceId).Any())
                                    {
                                        ws.Cell(currentRow, 9).Value = CONAPCOdeductunobligated;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPCOaddunobligated == 0 || CONAPCOdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 9).Value = unobligated_amount;
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 9).Value = "-";
                                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //PERCENT OF UTILIZATION
                                    if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) != 0 || afterrealignment_amount != 0)
                                    {
                                        ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 10).Value = "-";
                                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT DATA REGULAR
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)fundsource_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.FundSourceId,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from f in _MyDbContext.FundSources
                                                         join fa in _MyDbContext.FundSourceAmount
                                                         on f.FundSourceId equals fa.FundSourceId
                                                         select new
                                                         {
                                                             Id = f.FundSourceId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.FundSourceId,
                                                     uacsId = oa.UacsId,
                                                     status = o.status
                                                 }).ToList();

                                //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var realignment_subtotal = budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault().FundsRealignment?.Sum(x => x.Realignment_amount);
                                if (realignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Value = "0.00";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else
                                {
                                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 4).Value = realignment_subtotal;
                                }
                                ws.Cell(currentRow, 5).Style.Font.SetBold();
                                ws.Cell(currentRow, 5).Value = "0.00";
                                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance;

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount);


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_total += (double)fundSource.Beginning_balance;

                                currentRow++;




                            }

                            /*var fortheMonthTotalinTotalCoConap = (from oa in _MyDbContext.ObligationAmount
                                                                  join o in _MyDbContext.Obligation
                                                                  on oa.ObligationId equals o.Id
                                                                  join f in _MyDbContext.FundSources
                                                                  on o.FundSourceId equals f.FundSourceId
                                                                  where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                                  select new
                                                                  {
                                                                      amount = oa.Amount,
                                                                      uacsId = oa.UacsId,
                                                                      sourceId = o.FundSourceId,
                                                                      sourceType = o.source_type,
                                                                      date = o.Date,
                                                                      status = o.status,
                                                                      allotmentClassID = f.AllotmentClassId,
                                                                      appropriationID = f.AppropriationId
                                                                  }).ToList();

                            var asAtTotalinTotalCoConap = (from oa in _MyDbContext.ObligationAmount
                                                           join o in _MyDbContext.Obligation
                                                           on oa.ObligationId equals o.Id
                                                           join f in _MyDbContext.FundSources
                                                           on o.FundSourceId equals f.FundSourceId
                                                           where o.Date >= date1 && o.Date <= date2
                                                           select new
                                                           {
                                                               amount = oa.Amount,
                                                               sourceId = o.FundSourceId,
                                                               sourceType = o.source_type,
                                                               uacsId = oa.UacsId,
                                                               status = o.status,
                                                               allotmentClassID = f.AllotmentClassId,
                                                               appropriationID = f.AppropriationId
                                                           }).ToList();


                            var CoConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalCoConap = CoConapTotal - asAtTotalinTotalCoConap.Where(x => x.appropriationID == 2 && x.allotmentClassID == 3).Sum(x => x.amount);*/
                            //var totalPercentPSConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;

                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP CO";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoConapTotal;
                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCoConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 || CoConapTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentCoConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / CoConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentCoConap;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        //END CONAP CO LOOP

                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3 && x.BudgetAllotmentId == id).Any())
                        {
                            //START CONAP SAA CO LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CONAP CO SUB-ALLOTMENT";
                            currentRow++;
                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2 && x.BudgetAllotmentId == id))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();

                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description?.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (Suballotment_amount suballotment_amount in subAllotment.SubAllotmentAmounts.Where(x => x.status == "activated"))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 13).Value = suballotment_amount.beginning_balance;
                                    ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT SAA
                                    if (suballotment_amount.realignment_amount == 0)
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 19).Value = afterrealignment_amount;
                                    ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                    ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                    ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 22).Value = unobligated_amount;
                                    ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //PERCENT OF UTILIZATION
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }


                                    //REALIGNMENT DATA
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == suballotment_amount.SubAllotmentAmountId && x.FundSourceId == suballotment_amount.SubAllotmentId))
                                    //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper();

                                ws.Cell(currentRow, 13).Style.Font.SetBold();
                                ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 13).Value = subAllotment.Beginning_balance;

                                //TOTAL TRANSFER TO
                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                //ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources?.FirstOrDefault().FundSourceAmounts?.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources?.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var CoTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalCoSaaConap = CoTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentCoSaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP SAA MOOE";


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = CoTotalSaaConap;

                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = CoTotalSaaConap;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCoSaaConap;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentCoSaaConap;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                            //END CONAP SAA CO LOOP
                        }


                        //START IF CONAP CO PREVIOUS YEAR WAS CHECKED
                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId ==2 && x.AllotmentClassId == 3 && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.IsAddToNextAllotment == true).Any())
                        {
                            //START CONAP SAA MOOE LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Value = "CONAP CO SUB-ALLOTMENT FOR CY " + result;
                            currentRow++;
                            foreach (var subAllotment in _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result && x.AppropriationId ==2 && x.AllotmentClassId == 3))
                            {
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Range(ws.Cell(currentRow, 1), ws.Cell(currentRow, 11)).Merge();
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title?.ToUpper().ToString();



                                ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 11).Value = subAllotment.Date.ToShortDateString();
                                ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                                currentRow++;

                                ws.Cell(currentRow, 2).Style.Font.SetItalic();
                                ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 2).Value = subAllotment.Description?.ToString();
                                ws.Range(ws.Cell(currentRow, 2), ws.Cell(currentRow, 18)).Merge();
                                currentRow++;

                                foreach (var suballotment_amount in _MyDbContext.Suballotment_amount.Where(x => x.status == "activated" && x.SubAllotment.AppropriationId == 2 && x.SubAllotment.AllotmentClassId == 3 && x.SubAllotment.IsAddToNextAllotment == true && x.SubAllotmentId == subAllotment.SubAllotmentId))
                                {
                                    var uacsID = from Suballotment in _MyDbContext.Suballotment_amount
                                                 join u in _MyDbContext.Uacs
                                                 on Suballotment.UacsId equals u.UacsId
                                                 select Suballotment.UacsId;


                                    var fortheMonth = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           uacsId = oa.UacsId,
                                                           date = o.Date,
                                                           sourceId = o.FundSourceId,
                                                           sourceType = o.source_type,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId

                                                       }).ToList();

                                    var fundsourceID = (from Suballotment in _MyDbContext.SubAllotment
                                                        join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                        on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                        where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                        select new
                                                        {
                                                            saId = Suballotment.SubAllotmentId
                                                        }).ToList();

                                    var fundsourceamountID = (from Suballotment in _MyDbContext.SubAllotment
                                                              join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                              on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                              where Suballotment.SubAllotmentId == Suballotment_amount.SubAllotmentId
                                                              select new
                                                              {
                                                                  saAmountId = Suballotment_amount.SubAllotmentId
                                                              }).ToList();

                                    var asAt = (from oa in _MyDbContext.ObligationAmount
                                                join o in _MyDbContext.Obligation
                                                on oa.ObligationId equals o.Id
                                                where o.Date >= date1 && o.Date <= date2
                                                select new
                                                {
                                                    amount = oa.Amount,
                                                    uacsId = oa.UacsId,
                                                    sourceId = o.SubAllotmentId,
                                                    sourceType = o.source_type,
                                                    status = o.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 4).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();

                                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 12).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                                    if (suballotment_amount.beginning_balance != 0)
                                    {
                                        ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 14).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 13).Value = "-";
                                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT SAA
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT SAA AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "(" + suballotment_amount.realignment_amount + ")";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = _MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId).FirstOrDefault().Realignment_amount;
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 17).Value = "-";
                                        ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 18).Value = "-";
                                    ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    var CONAPSAAMOOEafterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    var CONAPSAAMOOEafterrealignment_amountadd = suballotment_amount.beginning_balance + _MyDbContext.SubAllotment_Realignment.FirstOrDefault(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId)?.Realignment_amount;
                                    //TOTAL ADJUSTED ALLOTMENT
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEafterrealignment_amount != 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amount;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT SAA
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = CONAPSAAMOOEafterrealignment_amountadd;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEafterrealignment_amount == 0 || CONAPSAAMOOEafterrealignment_amountadd == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = suballotment_amount.beginning_balance;
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (suballotment_amount.beginning_balance == 0)
                                    {
                                        //TOTAL ADJUSTED ALLOTMENT
                                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 19).Value = "-";
                                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    //OBLIGATED (FOR THE MONTH)
                                    if (fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 20).Value = "-";
                                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //OBLIGATED (AS AT)
                                    if (asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) != 0)
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 21).Value = "-";
                                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    var CONAPSAAMOOEaddunobligated = CONAPSAAMOOEafterrealignment_amount - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    var CONAPSAAMOOEdeductunobligated = CONAPSAAMOOEafterrealignment_amountadd - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any() || CONAPSAAMOOEaddunobligated != 0)
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEaddunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (_MyDbContext.SubAllotment_Realignment.Where(x => x.Realignment_to == suballotment_amount.UacsId && x.SubAllotmentId == suballotment_amount.SubAllotmentId).Any())
                                    {
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = CONAPSAAMOOEdeductunobligated;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (CONAPSAAMOOEaddunobligated == 0 || CONAPSAAMOOEdeductunobligated == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = unobligated_amount;
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    if (unobligated_amount == 0)
                                    {
                                        //UNOBLIGATED BALANCE OF ALLOTMENT
                                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 22).Value = "-";
                                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }

                                    if (asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) == 0 || afterrealignment_amount == 0)
                                    {
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = "-";
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    else
                                    {
                                        //PERCENT OF UTILIZATION
                                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                        ws.Cell(currentRow, 23).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    //REALIGNMENT DATA SUB ALLOTMENT
                                    var data = _MyDbContext.Uacs.Where(c => !_MyDbContext.Suballotment_amount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().UacsId;
                                    foreach (var realignment in _MyDbContext.SubAllotment_Realignment.Where(x => x.SubAllotmentAmountId == suballotment_amount.SubAllotmentAmountId && x.SubAllotmentId == suballotment_amount.SubAllotmentId && x.Realignment_to == data))
                                    {
                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {suballotment_amount.SubAllotmentAmountId}\nfundsrc_id {suballotment_amount}");
                                        //ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.Where(c => !_MyDbContext.FundSourceAmount.Select(b => b.UacsId).Contains(c.UacsId)).FirstOrDefault().Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper().ToString();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = realignment.Realignment_amount;
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 6).Value = "";
                                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    }
                                    currentRow++;
                                    total = (double)suballotment_amount.beginning_balance;
                                }

                                var fortheMonthTotal = (from oa in _MyDbContext.ObligationAmount
                                                        join o in _MyDbContext.Obligation
                                                        on oa.ObligationId equals o.Id
                                                        where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                        select new
                                                        {
                                                            amount = oa.Amount,
                                                            uacsId = oa.UacsId,
                                                            sourceId = o.SubAllotmentId,
                                                            sourceType = o.source_type,
                                                            date = o.Date,
                                                            status = o.status
                                                        }).ToList();

                                var funds_filterTotal = (from Suballotment in _MyDbContext.SubAllotment
                                                         join Suballotment_amount in _MyDbContext.Suballotment_amount
                                                         on Suballotment.SubAllotmentId equals Suballotment_amount.SubAllotmentId
                                                         select new
                                                         {
                                                             Id = Suballotment.SubAllotmentId
                                                         }).ToList();

                                var asAtTotal = (from oa in _MyDbContext.ObligationAmount
                                                 join o in _MyDbContext.Obligation
                                                 on oa.ObligationId equals o.Id
                                                 where o.Date >= date1 && o.Date <= date2
                                                 select new
                                                 {
                                                     amount = oa.Amount,
                                                     sourceId = o.SubAllotmentId,
                                                     uacsId = oa.UacsId,
                                                     sourceType = o.source_type,
                                                     status = o.status
                                                 }).ToList();


                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 4).Value = "SUBTOTAL " + subAllotment.Suballotment_title?.ToUpper() + " FOR CY " + result;

                                ws.Cell(currentRow, 14).Style.Font.SetBold();
                                ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 14).Value = subAllotment.Beginning_balance;

                                //REALIGNMENT SUBTOTAL
                                var CONAPSAAMOOErealignment_subtotal = budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.SubAllotment.FirstOrDefault().SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub6 = subAllotment.Beginning_balance - subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount) + subAllotment.SubAllotmentRealignment?.Sum(x => x.Realignment_amount);
                                var CONAPSAAMOOEsub9 = CONAPSAAMOOEsub6 - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated").Sum(x => x.amount);
                                if (CONAPSAAMOOErealignment_subtotal == null)
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Value = "0.00";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                }
                                else
                                {
                                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    ws.Cell(currentRow, 17).Value = CONAPSAAMOOErealignment_subtotal;
                                }

                                ws.Cell(currentRow, 18).Style.Font.SetBold();
                                ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 18).Value = "0.00";
                                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                ws.Cell(currentRow, 19).Style.Font.SetBold();
                                ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 19).Value = subAllotment.Beginning_balance;

                                ws.Cell(currentRow, 20).Style.Font.SetBold();
                                ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 20).Value = fortheMonthTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                //AS AT TOTAL
                                ws.Cell(currentRow, 21).Style.Font.SetBold();
                                ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 21).Value = asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == _MyDbContext.Suballotment_amount.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 22).Style.Font.SetBold();
                                ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 22).Value = unobligatedTotal;

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault()?.FundSourceAmounts.FirstOrDefault()?.FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AppropriationId == 2 && x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaaConap = MooeTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 11).Style.Font.SetBold();
                            ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 11).Value = "TOTAL CONAP SAA CO FOR CY " + result;

                            ws.Cell(currentRow, 14).Style.Font.SetBold();
                            ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 14).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 14).Value = MooeTotalSaaConap;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaaConap;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaaConap;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaaConap;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END CONAP SAA MOOE LOOP FOR CY RESULT
                        }
                        //END IF CONAP MOOE PREVIOUS YEAR WAS CHECKED



                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.BudgetAllotmentId == id).Any() || _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.BudgetAllotmentId == id).Any())
                        {
                            //CURRENT APPROPRIATION
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "CURRENT APPROPRIATION";
                            currentRow++;
                        }

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL PERSONNEL SERVICES";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = PsTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = PsTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalPSCurrent = PsTotal - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount);
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPSCurrent;

                            //PERCENT OF UTILIZATION
                            var totalPercentPSCurrent = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1).Sum(x => x.amount) / PsTotal;
                            ws.Cell(currentRow, 23).Value = totalPercentPSCurrent;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            currentRow++;
                        }

                        //START TOTAL AUTO APPRO

                        if (_MyDbContext.FundSources.Where(x => x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Any())
                        {

                            var PsTotalAPTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);
                            var PsTotalAP = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL AUTOMATIC APPROPRIATIONS";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = PsTotalAPTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = PsTotalAPTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalPSAPTotal = PsTotalAP - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPSAPTotal;

                            //PERCENT OF UTILIZATION
                            var totalPercentPSAPTotal = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentPSAPTotal;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                        }
                        else
                        {
                        }
                        //END TOTAL AUTO APPRO


                        /*var fortheMonthTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                                           join o in _MyDbContext.Obligation
                                                           on oa.ObligationId equals o.Id
                                                           join f in _MyDbContext.FundSources
                                                           on o.FundSourceId equals f.FundSourceId
                                                           where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                           select new
                                                           {
                                                               amount = oa.Amount,
                                                               uacsId = oa.UacsId,
                                                               sourceId = o.FundSourceId,
                                                               date = o.Date,
                                                               status = o.status,
                                                               allotmentClassID = f.AllotmentClassId
                                                           }).ToList();

                        var asAtTotalinTotalMOOE = (from oa in _MyDbContext.ObligationAmount
                                                    join o in _MyDbContext.Obligation
                                                    on oa.ObligationId equals o.Id
                                                    join f in _MyDbContext.FundSources
                                                    on o.FundSourceId equals f.FundSourceId
                                                    where o.Date >= date1 && o.Date <= date2
                                                    select new
                                                    {
                                                        amount = oa.Amount,
                                                        sourceId = o.FundSourceId,
                                                        uacsId = oa.UacsId,
                                                        status = o.status,
                                                        allotmentClassID = f.AllotmentClassId
                                                    }).ToList();*/

                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1).Any())
                        {

                            var MooeTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var allotment_totalMOOE = +MooeTotal;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL MOOE";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2 && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalMOOECurent = MooeTotal - asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount);

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOECurent;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) == 0 || MooeTotal == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "-";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentMOOECurrent = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) / MooeTotal;

                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentMOOECurrent;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }

                        }


                        //CO

                        /*var fortheMonthTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                                         join o in _MyDbContext.Obligation
                                                         on oa.ObligationId equals o.Id
                                                         join f in _MyDbContext.FundSources
                                                         on o.FundSourceId equals f.FundSourceId
                                                         where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                         select new
                                                         {
                                                             amount = oa.Amount,
                                                             uacsId = oa.UacsId,
                                                             sourceId = o.FundSourceId,
                                                             date = o.Date,
                                                             status = o.status,
                                                             allotmentClassID = f.AllotmentClassId
                                                         }).ToList();

                        var asAtTotalinTotalCO = (from oa in _MyDbContext.ObligationAmount
                                                  join o in _MyDbContext.Obligation
                                                  on oa.ObligationId equals o.Id
                                                  join f in _MyDbContext.FundSources
                                                  on o.FundSourceId equals f.FundSourceId
                                                  where o.Date >= date1 && o.Date <= date2
                                                  select new
                                                  {
                                                      amount = oa.Amount,
                                                      sourceId = o.FundSourceId,
                                                      uacsId = oa.UacsId,
                                                      status = o.status,
                                                      allotmentClassID = f.AllotmentClassId
                                                  }).ToList();*/

                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Any())
                        {

                            var CoTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                            var allotment_totalCO = +CoTotal;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CO";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = CoTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = CoTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalCOCurrent = CoTotal - asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCOCurrent;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) == 0 && CoTotal == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentCOCurrent = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / CoTotal;

                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentCOCurrent;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }


                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL PS SAA";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = PsTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = PsTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalPSSaa;

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotalinTotal = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentPSSaaTotalinTotal;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }

                        if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                        {

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL MOOE SAA";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment" && x.status == "activated").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }
                        //TOTAL CO SAA
                        if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.BudgetAllotmentId == id).Any())
                        {
                            var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.BudgetAllotmentId == id && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3  && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CO SAA";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = CoTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = CoTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCOSaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentCOSaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;

                        }



                        var TotalCA = _MyDbContext.FundSources.Where(x => x.AppropriationId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);

                        ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#B7DEE8");
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 3).Value = "TOTAL CURRENT APPROPRIATION";

                        ws.Cell(currentRow, 13).Style.Font.SetBold();
                        ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 13).Value = TotalCA;

                        //REALIGNMENT TOTAL
                        var REGULARrealignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                        if (REGULARrealignment_total == null)
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 17).Value = 0.00;
                        }
                        else
                        {
                            ws.Cell(currentRow, 17).Style.Font.SetBold();
                            ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 17).Value = REGULARrealignment_total;
                        }
                        //TOTAL TRANSFER TO
                        ws.Cell(currentRow, 18).Style.Font.SetBold();
                        ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 18).Value = "0.00";
                        ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        //TOTAL FUNDSOURCE - TOTAL AFTER REALIGMENT
                        ws.Cell(currentRow, 19).Style.Font.SetBold();
                        ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 19).Value = TotalCA;

                        var fortheMonthTotalinTotalCURRENT = (from oa in _MyDbContext.ObligationAmount
                                                              join o in _MyDbContext.Obligation
                                                              on oa.ObligationId equals o.Id
                                                              join f in _MyDbContext.FundSources
                                                              on o.FundSourceId equals f.FundSourceId
                                                              where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                              select new
                                                              {
                                                                  amount = oa.Amount,
                                                                  uacsId = oa.UacsId,
                                                                  sourceId = o.FundSourceId,
                                                                  date = o.Date,
                                                                  status = o.status,
                                                                  allotmentClassID = f.AllotmentClassId,
                                                                  appropriationID = f.AppropriationId
                                                              }).ToList();

                        var asAtTotalinTotalCURRENT = (from oa in _MyDbContext.ObligationAmount
                                                       join o in _MyDbContext.Obligation
                                                       on oa.ObligationId equals o.Id
                                                       join f in _MyDbContext.FundSources
                                                       on o.FundSourceId equals f.FundSourceId
                                                       where o.Date >= date1 && o.Date <= date2
                                                       select new
                                                       {
                                                           amount = oa.Amount,
                                                           sourceId = o.FundSourceId,
                                                           uacsId = oa.UacsId,
                                                           status = o.status,
                                                           allotmentClassID = f.AllotmentClassId,
                                                           appropriationID = f.AppropriationId
                                                       }).ToList();

                        var CurrentTotal = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);

                        //TOTAL - FOR THE MONTH
                        var CurrentApproForthemonth = fortheMonthTotalinTotalCURRENT.Where(x => x.appropriationID == 1 && x.status == "activated").Sum(x => x.amount) + fortheMonthTotalinTotalPS.Where(x => x.appropriationID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        var CurrentApproAsat = asAtTotalinTotalCURRENT.Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment" && x.status == "activated").Sum(x => x.amount);
                        ws.Cell(currentRow, 20).Style.Font.SetBold();
                        ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 20).Value = CurrentApproForthemonth;

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 21).Style.Font.SetBold();
                        ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 21).Value = CurrentApproAsat;

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalCURRENT = TotalCA - asAtTotalinTotalCURRENT.Sum(x => x.amount);
                        ws.Cell(currentRow, 22).Style.Font.SetBold();
                        ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                        ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCURRENT;

                        //PERCENT OF UTILIZATION
                        var totalPercentCURRENT = asAtTotalinTotalCURRENT.Sum(x => x.amount) / CurrentTotal;
                        ws.Cell(currentRow, 23).Style.Font.SetBold();
                        ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 23).Value = totalPercentCURRENT;
                        ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2).Any() || _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            //START CONAP SUMMARY
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 0;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "CONTINUING APPROPRIATION";
                            currentRow++;
                        }
                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP PERSONNEL SERVICES";


                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsConapTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSConapTotal = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentPSConapTotal;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONAP MOOE";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeConapTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMooeConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                            {
                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = "-";
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentMooeConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;

                                ws.Cell(currentRow, 23).Style.Font.SetBold();
                                ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 23).Value = totalPercentMooeConap;
                                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP CO";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoConapTotal;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoConapTotal;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCoConap;

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 || CoConapTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentCoConapTotal = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / CoConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentCoConapTotal;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP PS SAA";

                            var PsTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaaConap;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 4).Style.Font.SetBold();
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 4).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 5).Style.Font.SetBold();
                            ws.Cell(currentRow, 5).Value = "0.00";
                            ws.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaaConap;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalPSSaaConapTotal = PsTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaaConapTotal;

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotalinTotals = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = totalPercentPSSaaTotalinTotals;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }

                        if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Any())
                        {

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONAP MOOE SAA";


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }


                        //START IF PREVIOUS ALLOTMENT IS CHECKED TOTAL CONAP MOOE
                        if (_MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AllotmentClassId == 2 && x.AppropriationId == 2 && x.Budget_allotment.Yearly_reference.YearlyReference == result).Any())
                        {

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AllotmentClassId == 2 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONAP MOOE SAA FOR CY " + result;


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }
                        //END IF PREVIOUS ALLOTMENT IS CHECKED TOTAL CONAP MOOE

                        //TOTAL CO SAA
                        if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2).Any())
                        {

                            var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONAP CO SAA";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = CoTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = CoTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCOSaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentCOSaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }
                        //TOTAL CO SAA

                        //START IF PREVIOUS ALLOTMENT IS CHECKED TOTAL CONAP CO
                        if (_MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AllotmentClassId == 3 && x.AppropriationId == 2 && x.Budget_allotment.Yearly_reference.YearlyReference == result).Any())
                        {

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.AllotmentClassId == 3 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment" && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.fundSourceBudgetAllotmentId == id).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONAP CO SAA FOR CY " + result;


                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = MooeTotalSaa;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = MooeTotalSaa;

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalMOOESaa;

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;

                        }
                        //END IF PREVIOUS ALLOTMENT IS CHECKED TOTAL CONAP CO


                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Any() || _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Any())
                        {
                            var TotalCONAP = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2 && x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Beginning_balance);

                            ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#FABF8F");
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 3).Value = "TOTAL CONTINUING APPROPRIATION";

                            ws.Cell(currentRow, 13).Style.Font.SetBold();
                            ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 13).Value = TotalCONAP;

                            //REALIGNMENT TOTAL
                            var realignment_total = budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - budget_allotment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                            if (realignment_total == null)
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = 0.00;
                            }
                            else
                            {
                                ws.Cell(currentRow, 17).Style.Font.SetBold();
                                ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                                ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                                ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 17).Value = realignment_total;
                            }
                            //TOTAL TRANSFER TO
                            ws.Cell(currentRow, 18).Style.Font.SetBold();
                            ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 18).Value = "0.00";
                            ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL FUNDSOURCE - TOTAL AFTER REALIGMENT
                            ws.Cell(currentRow, 19).Style.Font.SetBold();
                            ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 19).Value = TotalCONAP;

                            var fortheMonthTotalinTotalCONAP = (from oa in _MyDbContext.ObligationAmount
                                                                join o in _MyDbContext.Obligation
                                                                on oa.ObligationId equals o.Id
                                                                join f in _MyDbContext.FundSources
                                                                on o.FundSourceId equals f.FundSourceId
                                                                where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                                select new
                                                                {
                                                                    amount = oa.Amount,
                                                                    uacsId = oa.UacsId,
                                                                    sourceId = o.FundSourceId,
                                                                    date = o.Date,
                                                                    status = o.status,
                                                                    allotmentClassID = f.AllotmentClassId,
                                                                    appropriationID = f.AppropriationId
                                                                }).ToList();

                            var asAtTotalinTotalCONAP = (from oa in _MyDbContext.ObligationAmount
                                                         join o in _MyDbContext.Obligation
                                                         on oa.ObligationId equals o.Id
                                                         join f in _MyDbContext.FundSources
                                                         on o.FundSourceId equals f.FundSourceId
                                                         where o.Date >= date1 && o.Date <= date2
                                                         select new
                                                         {
                                                             amount = oa.Amount,
                                                             sourceId = o.FundSourceId,
                                                             uacsId = oa.UacsId,
                                                             status = o.status,
                                                             allotmentClassID = f.AllotmentClassId,
                                                             appropriationID = f.AppropriationId
                                                         }).ToList();

                            var ConapTotal = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);

                            //TOTAL - FOR THE MONTH
                            var ConapApproForthemonth = fortheMonthTotalinTotalCONAP.Where(x => x.appropriationID == 2).Sum(x => x.amount) + fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var ConapApproAsat = asAtTotalinTotalCONAP.Where(x => x.appropriationID == 2).Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            ws.Cell(currentRow, 20).Style.Font.SetBold();
                            ws.Cell(currentRow, 20).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 20).Value = ConapApproForthemonth;

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 21).Style.Font.SetBold();
                            ws.Cell(currentRow, 21).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 21).Value = ConapApproAsat;

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalCONAP = TotalCONAP - ConapApproAsat;

                            ws.Cell(currentRow, 22).Style.Font.SetBold();
                            ws.Cell(currentRow, 22).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                            ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 22).Value = unobligatedTotalinTotalCONAP;

                            //PERCENT OF UTILIZATION
                            var totalPercentCONAP = ConapApproAsat / ConapTotal;

                            ws.Cell(currentRow, 23).Style.Font.SetBold();
                            ws.Cell(currentRow, 23).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                            ws.Cell(currentRow, 23).Value = totalPercentCONAP;
                            ws.Cell(currentRow, 23).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            //END CONAP SUMMARY
                        }

                    }

                    currentRow++;
                }

                var GrandTotals = _MyDbContext.FundSources.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.BudgetAllotmentId == id).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.IsAddToNextAllotment == true && x.Budget_allotment.Yearly_reference.YearlyReference == result).Sum(x => x.Beginning_balance);

                ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                ws.Cell(currentRow, 1).Style.Font.SetBold();
                ws.Cell(currentRow, 1).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 1).Value = "GRAND TOTAL";



                //currentRow++;
                ws.Cell(currentRow, 13).Style.Font.SetBold();
                ws.Cell(currentRow, 13).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 13).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 13).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 13).Value = GrandTotals;

                var GrandRealignment = _MyDbContext.Budget_allotments.Include(x => x.FundSources).ThenInclude(x => x.FundsRealignment).FirstOrDefault();

                //REALIGNMENT GRANDTOTAL
                var realignment_grandtotal = GrandRealignment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount) - GrandRealignment.FundSources.FirstOrDefault()?.FundsRealignment?.Sum(x => x.Realignment_amount);
                if (realignment_grandtotal == null)
                {
                    ws.Cell(currentRow, 17).Style.Font.FontColor = XLColor.White;
                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                    ws.Cell(currentRow, 17).Style.Font.FontSize = 12;
                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 17).Value = 0.00;
                }
                else
                {
                    ws.Cell(currentRow, 17).Style.Font.FontColor = XLColor.White;
                    ws.Cell(currentRow, 17).Style.Font.SetBold();
                    ws.Cell(currentRow, 17).Style.Font.FontSize = 12;
                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 17).Style.NumberFormat.Format = "#,##0.00";
                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 17).Value = realignment_grandtotal;
                }

                //TOTAL TRANSFER TO
                ws.Cell(currentRow, 18).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 18).Style.Font.SetBold();
                ws.Cell(currentRow, 18).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 18).Value = "0.00";
                ws.Cell(currentRow, 18).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                //GRANDTOTAL - TOTAL AFTER REALIGNMENT
                ws.Cell(currentRow, 19).Style.Font.SetBold();
                ws.Cell(currentRow, 19).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 19).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 19).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 19).Value = GrandTotals;

                var fortheMonthTotalinTotalCONAPGrand = (from oa in _MyDbContext.ObligationAmount
                                                         join o in _MyDbContext.Obligation
                                                         on oa.ObligationId equals o.Id
                                                         join f in _MyDbContext.FundSources
                                                         on o.FundSourceId equals f.FundSourceId
                                                         where o.Date >= date1 && o.Date <= lastday && o.Date >= firstDayOfMonth && o.Date <= lastday
                                                         select new
                                                         {
                                                             amount = oa.Amount,
                                                             uacsId = oa.UacsId,
                                                             sourceId = o.FundSourceId,
                                                             date = o.Date,
                                                             status = o.status,
                                                             allotmentClassID = f.AllotmentClassId,
                                                             appropriationID = f.AppropriationId
                                                         }).ToList();

                var asAtTotalinTotalCONAPGrand = (from oa in _MyDbContext.ObligationAmount
                                                  join o in _MyDbContext.Obligation
                                                  on oa.ObligationId equals o.Id
                                                  join f in _MyDbContext.FundSources
                                                  on o.FundSourceId equals f.FundSourceId
                                                  where o.Date >= date1 && o.Date <= date2
                                                  select new
                                                  {
                                                      amount = oa.Amount,
                                                      sourceId = o.FundSourceId,
                                                      uacsId = oa.UacsId,
                                                      status = o.status,
                                                      allotmentClassID = f.AllotmentClassId,
                                                      appropriationID = f.AppropriationId
                                                  }).ToList();

                var ConapTotalGrand = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);
                var ConapApproForthemonthGrand = fortheMonthTotalinTotalCONAPGrand.Where(x => x.appropriationID == 1 && x.status == "activated").Sum(x => x.amount) + fortheMonthTotalinTotalCONAPGrand.Where(x => x.appropriationID == 2 && x.status == "activated").Sum(x => x.amount);
                var ConapApproAsatGrand = asAtTotalinTotalCONAPGrand.Where(x => x.appropriationID == 1 && x.status == "activated").Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.appropriationID == 2).Sum(x => x.amount);
                ws.Cell(currentRow, 20).Style.Font.SetBold();
                ws.Cell(currentRow, 20).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 20).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 20).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 20).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 20).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 20).Value = ConapApproForthemonthGrand;

                ws.Cell(currentRow, 21).Style.Font.SetBold();
                ws.Cell(currentRow, 21).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 21).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 21).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 21).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 21).Value = ConapApproAsatGrand;

                var GrandTotalUnobligated_amount = GrandTotals - ConapApproAsatGrand;
                ws.Cell(currentRow, 22).Style.Font.SetBold();
                ws.Cell(currentRow, 22).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 22).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 22).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 22).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 22).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 22).Value = GrandTotalUnobligated_amount.ToString("N", new CultureInfo("en-US"));

                var GrandTotalpercentage = ConapApproAsatGrand / ConapTotalGrand;
                ws.Cell(currentRow, 23).Style.Font.SetBold();
                ws.Cell(currentRow, 23).Style.Font.FontSize = 12;
                ws.Cell(currentRow, 23).Style.Font.FontName = "Calibri Light";
                ws.Cell(currentRow, 23).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(currentRow, 23).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 23).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 23).Value = GrandTotalpercentage.ToString("N", new CultureInfo("en-US"));

                //ws.Columns().AdjustToContents();
                //ws.Rows().AdjustToContents();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //ws.Columns().AdjustToContents();
                    //ws.Rows().AdjustToContents();
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", date2.ToString("MMMM").ToUpper() + ".xlsx");
                }
            }
        }

        public IActionResult DownloadSaob()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            PopulatePrexcsDropDownList();
            return View();
        }


        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _MyDbContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.Conap = new SelectList((from s in _MyDbContext.SubAllotment.ToList()
                                              select new
                                              {
                                                  SuballotmentId = s.SubAllotmentId,
                                                  SuballotmentTitle = s.Suballotment_title,
                                              }),
                                       "SuballotmentId",
                                       "SuballotmentTitle",
                                       null);

        }

    }
}
//JOHNS UPDATE