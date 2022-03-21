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


        public IActionResult Export(string fn, string date_from, string date_to)
        {

            DateTime date1 = Convert.ToDateTime(date_from);
            DateTime date2 = Convert.ToDateTime(date_to);
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


            var dateTime = _MyDbContext.FundSources.Where(x => x.CreatedAt >= date1 && x.CreatedAt <= dateTomorrow).Select(y => new { y.FundSourceTitle });


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
                                                 status = oa.status,
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
                                          status = oa.status,
                                          allotmentClassID = f.AllotmentClassId,
                                          appropriationID = f.AppropriationId,
                                          fundSourceTitle = f.FundSourceTitle
                                      }).ToList();
            var PsTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);




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
                                                      status = oa.status,
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
                                               status = oa.status,
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
                                                      status = oa.status,
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
                                               status = oa.status,
                                               allotmentClassID = f.AllotmentClassId,
                                               appropriationID = f.AppropriationId
                                           }).ToList();


            var CoConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
            var unobligatedTotalinTotalCoConap = CoConapTotal - asAtTotalinTotalCoConap.Where(x => x.appropriationID == 2 && x.allotmentClassID == 3).Sum(x => x.amount);





            DataTable dt = new DataTable("Saob Report");

            var ballots = from ballot in _MyDbContext.Budget_allotments.ToList()
                          select ballot;



            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Column(7).AdjustToContents();
                ws.Worksheet.SheetView.FreezeColumns(2);
                ws.Worksheet.SheetView.FreezeRows(13);

                IXLRange range = ws.Range(ws.Cell(2, 1).Address, ws.Cell(13, 11).Address);

                range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                var currentRow = 14;
                Double total = 0.00;
                Double allotment_total = 0;
                Double allotment_totalSaa = 0;
                Double suballotment_total = 0;
                Double GrandTotal = 0;


                //PS SAA
                var PsTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                var unobligatedTotalinTotalPSSaa = PsTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                var totalPercentPSSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1).Sum(x => x.amount) / allotment_total;


                var wsFreeze = ws.Worksheet.Cell("Freeze View");



                ws.Worksheet.SheetView.FreezeColumns(2);
                ws.Worksheet.SheetView.FreezeRows(13);


                range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                //ws.Cell("A7").RichText.AddText("Department:");
                ws.Cell("A8").RichText.AddText("Agency/OU:");
                ws.Cell("A9").RichText.AddText("Fund");
                ws.Cell("B7").RichText.AddText("HEALTH");
                ws.Cell("B8").RichText.AddText("REGIONAL OFFICE VII");
                ws.Range("A7:D7").Merge();
                IXLRange titleRange = ws.Range("A7:D7");
                //ws.Range("A7:D7").Value = "Department:";
                titleRange.Cells().Style.Alignment.SetWrapText(true);
                titleRange.Value = "Department:";
                ws.Range("A8:D8").Merge();
                ws.Range("A8:D8").Value = "Agency /OU:";
                ws.Range("A9:D9").Merge();
                ws.Range("A9:D9").Value = "Fund";
                /*ws.Range("A7:C7").Merge();
                ws.Range("A8:C8").Merge();
                ws.Range("A9:C9").Merge();*/


                ws.Cell("F4").Style.Font.SetBold();
                ws.Cell("F4").Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("F4").RichText.AddText("STATEMENT OF ALLOTMENTS, OBLIGATIONS AND BALANCES");

                ws.Cell("F5").Style.Font.SetBold();
                ws.Cell("F5").Style.Font.FontSize = 10;
                ws.Cell("F5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                /*ws.Columns(11, 1).AdjustToContents();*/
                ws.Cell("F5").Style.DateFormat.Format = "AS AT" + " " + "MMMM dd, yyyy";
                ws.Cell("F5").Value = date2.ToString().ToUpper();


                ws.Cell("F6").Style.Font.FontSize = 10;
                ws.Cell("F6").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("F6").RichText.AddText("(In Pesos)");

                //FIRST ROW
                ws.Cell(11, 1).Style.Font.SetBold();
                ws.Cell(11, 1).RichText.AddText("P/A/P /ALLOTMENT CLASS/");
                //SECOND ROW
                ws.Cell(12, 1).Style.Font.SetBold();
                ws.Cell(12, 1).RichText.AddText("OBJECT OF EXPENDITURE");

                //FIRST ROW
                ws.Cell(11, 2).Style.Font.SetBold();
                ws.Cell(11, 2).Style.Font.FontSize = 12;
                ws.Cell(11, 2).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 2).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 2).AdjustToContents();
                ws.Cell(11, 2).RichText.AddText("EXPENSES");

                //SECOND ROW
                ws.Cell(12, 2).Style.Font.SetBold();
                ws.Cell(12, 2).RichText.AddText("CODE");

                //FIRST ROW
                ws.Cell(11, 3).Style.Font.SetBold();
                ws.Cell(11, 3).Style.Font.FontSize = 12;
                ws.Cell(11, 3).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 3).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 3).AdjustToContents();
                ws.Cell(11, 3).RichText.AddText("ALLOTMENT");

                //SECOND ROW
                ws.Cell(12, 3).Style.Font.SetBold();
                ws.Cell(12, 3).RichText.AddText("RECEIVED");

                //SECOND ROW
                ws.Cell(12, 4).Style.Font.SetBold();
                ws.Cell(11, 4).Style.Font.FontSize = 12;
                ws.Cell(11, 4).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 4).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 4).AdjustToContents();
                ws.Cell(12, 4).RichText.AddText("REALIGNMENT");

                //SECOND ROW
                ws.Cell(12, 5).Style.Font.SetBold();
                ws.Cell(1, 5).Style.Font.FontSize = 12;
                ws.Cell(1, 5).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 5).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 5).AdjustToContents();
                ws.Cell(12, 5).RichText.AddText("TRANSFER TO");

                //FIRST ROW
                ws.Cell(11, 6).Style.Font.SetBold();
                ws.Cell(1, 6).Style.Font.FontSize = 12;
                ws.Cell(1, 6).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 6).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(1, 6).AdjustToContents();
                ws.Cell(11, 6).RichText.AddText("TOTAL ADJUSTED");
                ws.Cell(11, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                //FIRST ROW
                ws.Cell(11, 7).Style.Font.SetBold();
                ws.Cell(1, 6).Style.Font.FontSize = 12;
                ws.Cell(1, 6).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 6).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(1, 6).AdjustToContents();
                ws.Cell(11, 7).RichText.AddText("FOR THE MONTH");
                ws.Cell(11, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                //SECOND ROW
                ws.Cell(12, 6).Style.Font.SetBold();
                ws.Cell(12, 6).RichText.AddText("ALLOTMENT");
                ws.Cell(12, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                //SECOND ROW
                ws.Cell(12, 7).Style.Font.SetBold();
                ws.Cell(1, 7).Style.Font.FontSize = 12;
                ws.Cell(1, 7).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 7).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 7).AdjustToContents();

                ws.Cell(12, 7).Value = date2.ToString("MMMM").ToUpper();
                //ws.Cell(12, 7).Value = DateTime.Now.ToString("MMMM yyyy");


                //SECOND ROW
                ws.Cell(12, 8).Style.Font.SetBold();
                ws.Cell(1, 8).Style.Font.FontSize = 12;
                ws.Cell(1, 8).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell(12, 8).WorksheetColumn().AdjustToContents();

                ws.Cell(12, 8).Value = "AS AT" + " " + date2.ToString("MMMM").ToUpper();


                //FIRST ROW
                ws.Cell(11, 9).Style.Font.SetBold();
                ws.Cell(1, 9).Style.Font.FontSize = 12;
                ws.Cell(1, 9).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 9).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 9).AdjustToContents();
                ws.Cell(11, 9).RichText.AddText("UNOBLIGATED");

                //SECOND ROW
                ws.Cell(12, 9).Style.Font.SetBold();
                ws.Cell(1, 9).Style.Font.FontSize = 12;
                ws.Cell(1, 9).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 9).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 9).AdjustToContents();
                ws.Cell(12, 9).RichText.AddText("BALANCE OF");

                //THIRD ROW
                ws.Cell(13, 9).Style.Font.SetBold();
                ws.Cell(1, 9).Style.Font.FontSize = 12;
                ws.Cell(1, 9).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 9).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(13, 9).AdjustToContents();
                ws.Cell(13, 9).RichText.AddText("ALLOTMENT");

                //FIRST ROW
                ws.Cell(11, 10).Style.Font.SetBold();
                ws.Cell(1, 10).Style.Font.FontSize = 12;
                ws.Cell(1, 10).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 10).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 10).AdjustToContents();
                ws.Cell(11, 10).RichText.AddText("%");

                //SECOND ROW
                ws.Cell(12, 10).Style.Font.SetBold();
                ws.Cell(1, 10).Style.Font.FontSize = 12;
                ws.Cell(1, 10).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 10).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 10).AdjustToContents();
                ws.Cell(12, 10).RichText.AddText("OF");

                //THIRD ROW
                ws.Cell(13, 10).Style.Font.SetBold();
                ws.Cell(1, 10).Style.Font.FontSize = 12;
                ws.Cell(1, 10).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 10).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(13, 10).AdjustToContents();
                ws.Cell(13, 10).RichText.AddText("UTILIZATION");

                //FIRST ROW
                ws.Cell(11, 11).Style.Font.SetBold();
                ws.Cell(1, 11).Style.Font.FontSize = 12;
                ws.Cell(1, 11).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 11).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 11).AdjustToContents();
                ws.Cell(11, 11).RichText.AddText("DISBURSEMENT");

                //SECOND ROW
                ws.Cell(12, 11).Style.Font.SetBold();
                ws.Cell(1, 11).Style.Font.FontSize = 12;
                ws.Cell(1, 11).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 11).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(12, 11).AdjustToContents();
                ws.Cell(12, 11).RichText.AddText("As of December");


                var budget_allotments = _MyDbContext.Budget_allotments
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(Allotment_class => Allotment_class.AllotmentClass)
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(a => a.Appropriation)
                    .Include(budget_allotment => budget_allotment.FundSources)
                        .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                        .ThenInclude(uacs => uacs.Uacs).ToList();

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

                    ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 12;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Value = "CURRENTAPPROPRIATION";
                    currentRow++;


                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Value = "Personnel Services";
                    currentRow++;


                    //START PS LOOP

                    foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.FundSourceTitle != "AUTOMATIC APPROPRIATION"))
                    {



                        ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.SetBold();
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
                                                   status = oa.status

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
                                            status = oa.status
                                        }).ToList();

                            var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                            total = 0;
                            var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                            ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //REALIGNMENT AMOUNT
                            ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL ADJUSTED ALLOTMENT
                            ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (FOR THE MONTH)
                            ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (AS AT)
                            //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //UNOBLIGATED BALANCE OF ALLOTMENT
                            //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //PERCENT OF UTILIZATION
                            if (asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) == 0 && afterrealignment_amount == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            else
                            { 
                            ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            }
                            //REALIGNMENT DATA
                            foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                            //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                            {

                                currentRow++;
                                Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = "0.00";
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 6).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
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
                                                    status = oa.status
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
                                             status = oa.status
                                         }).ToList();

                        //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                        //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                        ws.Cell(currentRow, 4).Style.Font.SetBold();
                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 4).Value = budget_allotment.FundSources.FirstOrDefault().FundsRealignment.Sum(x => x.Realignment_amount);

                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //AS AT TOTAL
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                        var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                        //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        var subAllotmentTotal = _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance);

                        allotment_total += (double)fundSource.Beginning_balance + (double)subAllotmentTotal;

                        //return Json(allotment_total);

                        currentRow++;




                    }

                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 1).Value = "TOTAL PERSONNEL SERVICES";

                    var PTC = budget_allotment.FundSources.FirstOrDefault()?.Beginning_balance;
                    var PsTotalCurrent = +(double)PTC;


                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = PsTotal.ToString("N", new CultureInfo("en-US"));

                    ws.Cell(currentRow, 4).Style.Font.SetBold();
                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 4).Value = budget_allotment.FundSources.FirstOrDefault().FundsRealignment.Sum(x => x.Realignment_amount);

                    //TOTAL - TOTAL AFTER REALIGNMENT
                    ws.Cell(currentRow, 6).Style.Font.SetBold();
                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 6).Value = PsTotal.ToString("N", new CultureInfo("en-US"));

                    //TOTAL - FOR THE MONTH
                    ws.Cell(currentRow, 7).Style.Font.SetBold();
                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                    //TOTAL - AS AT
                    ws.Cell(currentRow, 8).Style.Font.SetBold();
                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                    //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                    var unobligatedTotalinTotalPS = PsTotal - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                    ws.Cell(currentRow, 9).Style.Font.SetBold();
                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPS.ToString("N", new CultureInfo("en-US"));

                    //PERCENT OF UTILIZATION
                    var totalPercentPS = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle != "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;
                    ws.Cell(currentRow, 10).Value = totalPercentPS;
                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    currentRow++;
                    //END PS LOOP





                    //START AUTOMATIC APPROPRIATION PS LOOP

                    foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION"))
                    {



                        ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.SetBold();
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
                                                   status = oa.status

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
                                            status = oa.status
                                        }).ToList();

                            var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                            total = 0;
                            var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                            ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                            ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //REALIGNMENT AMOUNT
                            ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL ADJUSTED ALLOTMENT
                            ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (FOR THE MONTH)
                            ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (AS AT)
                            //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //UNOBLIGATED BALANCE OF ALLOTMENT
                            //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                            //REALIGNMENT DATA
                            foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                            //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                            {




                                currentRow++;
                                Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = "0.00";
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                    status = oa.status
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
                                             status = oa.status
                                         }).ToList();

                        //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                        //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //AS AT TOTAL
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                        var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                        //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        var subAllotmentTotal = _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance);

                        allotment_total += (double)fundSource.Beginning_balance + (double)subAllotmentTotal;

                        //return Json(allotment_total);

                        currentRow++;




                    }

                    if (_MyDbContext.FundSources.Where(x => x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Any())
                    {
                        var PsTotalAP = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 1).Value = "TOTAL AUTOMATIC APPROPRIATIONS";

                        var PTCAP = budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                        var PsTotalCurrentAP = +(double)PTC;


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = PsTotalAP.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = PsTotalAP.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalPSAP = PsTotalAP - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSAP.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        var totalPercentPSAP = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;
                        ws.Cell(currentRow, 10).Value = totalPercentPSAP;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;
                    }
                    //END AUTOMATIC APPROPRIATION PS LOOP



                    //START MOOE LOOP
                    if (budget_allotment.FundSources.Where(x=>x.AllotmentClassId == 2).Any())
                    {
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Value = "Maintenance and Other Operating Expenses";
                        currentRow++;

                        foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1))
                        {

                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
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
                                                       status = oa.status

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
                                                status = oa.status
                                            }).ToList();

                                var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                total = 0;
                                var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (AS AT)
                                //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                                //REALIGNMENT DATA
                                foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                                //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                {




                                    currentRow++;
                                    Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = "0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                        status = oa.status

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
                                                 status = oa.status
                                             }).ToList();

                            //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                            //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //AS AT TOTAL
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

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
                        ws.Cell(currentRow, 1).Value = "TOTAL MOOE";


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
                                                               status = oa.status,
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
                                                        status = oa.status,
                                                        allotmentClassID = f.AllotmentClassId
                                                    }).ToList();

                        var MooeTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                        var allotment_totalMOOE = +MooeTotal;

                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = MooeTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = MooeTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalMOOE = MooeTotal - asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOE.ToString("N", new CultureInfo("en-US"));


                        //PERCENT OF UTILIZATION
                        if (MooeTotal == 0 && asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) == 0)
                        {
                            ws.Cell(currentRow, 10).Value = "";
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }
                        else
                        {
                            var totalPercentMOOE = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) / MooeTotal;
                            ws.Cell(currentRow, 10).Value = totalPercentMOOE;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }
                        //END MOOE LOOP
                    }
                    else
                    {
                    }

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

                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
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
                                                       status = oa.status

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
                                                status = oa.status
                                            }).ToList();

                                var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                total = 0;
                                var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (AS AT)
                                //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                                //REALIGNMENT DATA
                                foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                                //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                {




                                    currentRow++;
                                    Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = "0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                        status = oa.status

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
                                                 status = oa.status
                                             }).ToList();

                            //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                            //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //AS AT TOTAL
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

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
                                                             status = oa.status,
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
                                                      status = oa.status,
                                                      allotmentClassID = f.AllotmentClassId
                                                  }).ToList();

                        var CoTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                        var allotment_totalCO = +CoTotal;

                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = CoTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = CoTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalCO = CoTotal - asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCO.ToString("N", new CultureInfo("en-US"));

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
                    else
                    {

                    }






                    var saa = _MyDbContext.Budget_allotments
                        .Include(sub_allotment => sub_allotment.SubAllotment)
                        .ThenInclude(suballotment_amount => suballotment_amount.SubAllotmentAmounts)
                        .ThenInclude(uacs => uacs.Uacs);


                    foreach (BudgetAllotment b in saa)
                    {

                        /*foreach (Sub_allotment sa in b.Sub_allotments.Where(x => x.AllotmentClassId == 1))
                        {*/
                        //Double suballotment_total = 0;

                        /*currentRow++;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Value = "CURRENT PS SUB-ALLOTMENT";*/
                        //currentRow++;


                        /*ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == sa.prexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        currentRow++;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = sa.Suballotment_title.ToUpper().ToString();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        currentRow++;
                        ws.Cell(currentRow, 1).Style.Font.Italic = true;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 2;
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        //ws.Cell(currentRow, 1).Value = sa.Description.ToString();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        currentRow++;*/

                        if (_MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CURRENT PS SUB-ALLOTMENT";
                            currentRow++;
                        }
                        else
                        {
                        }
                        
                        //START SAA PS LOOP
                        foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1))
                        {

                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            currentRow++;

                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                            ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                       status = oa.status,
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
                                                status = oa.status
                                            }).ToList();

                                var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                total = 0;
                                var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (AS AT)
                                //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount) / afterrealignment_amount;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                    ws.Cell(currentRow, 3).Value = "0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                        status = oa.status
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
                                                 status = oa.status
                                             }).ToList();


                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                            //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //AS AT TOTAL
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                            var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            allotment_totalSaa += (double)subAllotment.Beginning_balance;

                            currentRow++;

                        }



                        if (_MyDbContext.SubAllotment.Where(x=>x.AppropriationId ==1 && x.AllotmentClassId == 1).Any())
                        {
                            //START SAA PS LOOP
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL PERSONNEL SERVICES SAA" + " " /*+ budget_allotment.Allotment_code.ToUpper().ToString()*/;


                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaa.ToString("N", new CultureInfo("en-US"));

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotal = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = totalPercentPSSaaTotal;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            //END SAA PS LOOP
                        }
                        else
                        {
                        }
                        

                        //START SAA MOOE LOOP
                        if (budget_allotment.SubAllotment.Where(x=>x.AppropriationId == 1 && x.AllotmentClassId == 2).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CURRENT MOOE SUB-ALLOTMENT";
                            currentRow++;

                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                           status = oa.status,
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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL MOOE SAA" + " " /*+ budget_allotment.Allotment_code.ToUpper().ToString()*/;




                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOESaa.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END SAA MOOE LOOP
                        }
                        else
                        {
                        
                        }




                        if (_MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1 && x.AllotmentClassId == 3).Any())
                        {
                            //START SAA CO LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CURRENT CO SUB-ALLOTMENT";
                            currentRow++;
                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                           status = oa.status,
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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            if (budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3).Any())
                            {

                                var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
                                var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;

                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 1).Value = "TOTAL CO SAA" + " " /*+ budget_allotment.Allotment_code.ToUpper().ToString()*/;




                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                                //TOTAL - TOTAL AFTER REALIGNMENT
                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                                //TOTAL - FOR THE MONTH
                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //TOTAL - AS AT
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCOSaa.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = totalPercentCOSaa;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                currentRow++;
                                //END SAA CO LOOP
                            }
                        }

                        



                        //CONAP HEADER
                        if(_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2).Any() || _MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 2).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.OrangePeel;
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 12;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CONTINUING APPROPRIATION";
                            currentRow++;
                        }
                        else
                        {
                        }

                        if (_MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "Personnel Services";
                            currentRow++;

                            //START CONAP PS LOOP
                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
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
                                                           status = oa.status

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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                                    //REALIGNMENT DATA
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                                    //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                    {




                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();

                                //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_total += (double)fundSource.Beginning_balance;

                                currentRow++;




                            }

                            /*var fortheMonthTotalinTotalPSConap = (from oa in _MyDbContext.ObligationAmount
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
                                                                      status = oa.status,
                                                                      allotmentClassID = f.AllotmentClassId,
                                                                      appropriationID = f.AppropriationId
                                                                  }).ToList();*/

                            /*var asAtTotalinTotalPSConap = (from oa in _MyDbContext.ObligationAmount
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
                                                               status = oa.status,
                                                               allotmentClassID = f.AllotmentClassId,
                                                               appropriationID = f.AppropriationId
                                                           }).ToList();*/


                            //var PsConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 1).Sum(x => x.Beginning_balance);
                            //var unobligatedTotalinTotalPSConap = PsConapTotal - asAtTotalinTotalPSConap.Where(x => x.appropriationID == 2).Sum(x => x.amount);
                            //var totalPercentPSConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;

                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP PERSONNEL SERVICES";


                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSConap.ToString("N", new CultureInfo("en-US"));

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
                                var totalPercentPSConaps = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentPSConaps;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        //END CONAP PS LOOP

                        //START CONAP MOOE LOOP

                        foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2))
                        {

                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                            ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
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
                                                       status = oa.status

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
                                                status = oa.status
                                            }).ToList();

                                var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                total = 0;
                                var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL ADJUSTED ALLOTMENT
                                ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (FOR THE MONTH)
                                ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //OBLIGATED (AS AT)
                                //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                                //REALIGNMENT DATA
                                foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                                //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                {




                                    currentRow++;
                                    Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = "0.00";
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                        status = oa.status
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
                                                 status = oa.status
                                             }).ToList();

                            //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                            //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //AS AT TOTAL
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                            var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                            //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / fundSource.Beginning_balance;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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
                                                                    status = oa.status,
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
                                                             status = oa.status,
                                                             allotmentClassID = f.AllotmentClassId,
                                                             appropriationID = f.AppropriationId
                                                         }).ToList();


                        var MooeConapTotal = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2 && x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                        var unobligatedTotalinTotalMooeConap = MooeConapTotal - asAtTotalinTotalMooeConap.Where(x => x.appropriationID == 2 && x.allotmentClassID == 2).Sum(x => x.amount);
                        //var totalPercentPSConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;

                        if(_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 2).Any())
                        {
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 1).Value = "TOTAL CONAP MOOE";


                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = MooeConapTotal.ToString("N", new CultureInfo("en-US"));

                                //TOTAL - TOTAL AFTER REALIGNMENT
                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = MooeConapTotal.ToString("N", new CultureInfo("en-US"));

                                //TOTAL - FOR THE MONTH
                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //TOTAL - AS AT
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMooeConap.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                                {
                                    ws.Cell(currentRow, 10).Value = "";
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                    currentRow++;
                                }
                                else { 
                                var totalPercentPSConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentPSConap;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                                }
                                //END CONAP MOOE LOOP
                            }


                            //START CONAP CO LOOP
                            if(_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 3).Any())
                            {
                            foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
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
                                                           status = oa.status

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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = fundsource_amount.beginning_balance - asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount);


                                    total = 0;
                                    var afterrealignment_amount = fundsource_amount.beginning_balance - fundsource_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId && x.sourceId == fundsource_amount.FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == fundsource_amount.UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                                    //REALIGNMENT DATA
                                    foreach (var realignment in _MyDbContext.FundsRealignment.Where(x => x.FundSourceAmountId == fundsource_amount.FundSourceAmountId && x.FundSourceId == fundsource_amount.FundSourceId))
                                    //foreach(var realignment in fundsource_amount.FundSource.FundsRealignment)
                                    {




                                        currentRow++;
                                        Debug.WriteLine($"fsaid: {fundsource_amount.FundSourceAmountId}\nfundsrc_id {fundsource_amount}");
                                        ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Account_title.ToUpper();
                                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                        ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == realignment.Realignment_to).Expense_code;
                                        ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();

                                //ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = fundSource.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = fundSource.Beginning_balance - asAtTotal.Where(x => x.sourceId == fundSource.FundSourceAmounts.FirstOrDefault().FundSourceId).Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

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
                                                                      status = oa.status,
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
                                                               status = oa.status,
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
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCoConap.ToString("N", new CultureInfo("en-US"));

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



                    if(_MyDbContext.SubAllotment.Where(x=>x.AppropriationId ==2 && x.AllotmentClassId == 1).Any())
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
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                           status = oa.status,
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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

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


                            var PsTotalSaaConapPS = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalPSSaaConap = PsTotalSaaConapPS - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var totalPercentPSSaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaaConapPS.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaaConapPS.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaaConap.ToString("N", new CultureInfo("en-US"));

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
                        else
                        {
                        }


                    if(_MyDbContext.SubAllotment.Where(x=>x.AppropriationId ==2 && x.AllotmentClassId == 2).Any())
                    {
                            //START CONAP SAA MOOE LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CONAP MOOE SUB-ALLOTMENT";
                            currentRow++;
                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                           status = oa.status,
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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var MooeTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaaConap = MooeTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var totalPercentMOOESaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP SAA MOOE" + " " /*+ budget_allotment.Allotment_code.ToUpper().ToString()*/;




                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = MooeTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = MooeTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOESaaConap.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = totalPercentMOOESaaConap;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END CONAP SAA MOOE LOOP
                    }

                        if (_MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 3).Any())
                        {
                            //START CONAP CO SAA LOOP
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "CONAP CO SUB-ALLOTMENT";
                            currentRow++;
                            foreach (SubAllotment subAllotment in budget_allotment.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2))
                            {

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == subAllotment.prexcId)?.pap_code1;
                                ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;

                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = subAllotment.FundId.ToString();
                                ws.Cell(currentRow, 1).Value = subAllotment.Suballotment_title.ToUpper().ToString();
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
                                                           status = oa.status,
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
                                                    status = oa.status
                                                }).ToList();

                                    var unobligated_amount = suballotment_amount.beginning_balance - asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);

                                    total = 0;
                                    var afterrealignment_amount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;
                                    ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                    ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    //ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                    ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //REALIGNMENT AMOUNT
                                    ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                    ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //TOTAL ADJUSTED ALLOTMENT
                                    ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (FOR THE MONTH)
                                    ws.Cell(currentRow, 7).Value = fortheMonth.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //OBLIGATED (AS AT)
                                    //ws.Cell(currentRow, 8).Value = asAt.FirstOrDefault(x => x.uacsId == fundsource_amount.UacsId)?.amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Value = asAt.Where(x => x.uacsId == suballotment_amount.UacsId && x.sourceId == suballotment_amount.SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //UNOBLIGATED BALANCE OF ALLOTMENT
                                    //ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Value = unobligated_amount.ToString("N", new CultureInfo("en-US"));
                                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                    //PERCENT OF UTILIZATION
                                    ws.Cell(currentRow, 10).Value = asAt.Where(x => x.uacsId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().UacsId).Sum(x => x.amount) / afterrealignment_amount;
                                    ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                                        ws.Cell(currentRow, 3).Value = "0.00";
                                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                        //REALIGNMENT AMOUNT
                                        ws.Cell(currentRow, 4).Value = suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US"));
                                        ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
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
                                                            status = oa.status
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
                                                     status = oa.status
                                                 }).ToList();


                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                                ws.Cell(currentRow, 1).Style.Font.SetBold();
                                ws.Cell(currentRow, 1).Value = "SUBTOTAL " + subAllotment.Suballotment_title.ToUpper()/* + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code*/;

                                //ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                                //ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                                ws.Cell(currentRow, 3).Style.Font.SetBold();
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 3).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 6).Style.Font.SetBold();
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 6).Value = subAllotment.Beginning_balance.ToString("N", new CultureInfo("en-US"));

                                ws.Cell(currentRow, 7).Style.Font.SetBold();
                                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 7).Value = fortheMonthTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                                //AS AT TOTAL
                                ws.Cell(currentRow, 8).Style.Font.SetBold();
                                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 8).Value = asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));


                                var unobligatedTotal = subAllotment.Beginning_balance - asAtTotal.Where(x => x.sourceId == subAllotment.SubAllotmentAmounts.FirstOrDefault().SubAllotmentId && x.status == "activated" && x.sourceType == "sub_allotment").Sum(x => x.amount);
                                //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Style.Font.SetBold();
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(currentRow, 9).Value = unobligatedTotal.ToString("N", new CultureInfo("en-US"));

                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = asAtTotal.Where(x => x.sourceId == budget_allotment.FundSources.FirstOrDefault().FundSourceAmounts.FirstOrDefault().FundSourceId && x.status == "activated").Sum(x => x.amount) / budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                allotment_totalSaa += (double)subAllotment.Beginning_balance;

                                currentRow++;

                            }

                            var CoTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalCoSaaConap = CoTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var totalPercentCoSaaConap = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount) / allotment_total;


                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP SAA MOOE";




                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCoSaaConap.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = totalPercentCoSaaConap;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            //END CONAP SAA CO LOOP
                        }




                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 1).Any() || _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Any())
                        {
                            //CURRENT APPROPRIATION
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 0;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "CURRENT APPROPRIATION";
                            currentRow++;
                        }
                        
                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 1 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL PERSONNEL SERVICES";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalPSCurrent = PsTotal - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1).Sum(x => x.amount);
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSCurrent.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            var totalPercentPSCurrent = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1).Sum(x => x.amount) / PsTotal;
                            ws.Cell(currentRow, 10).Value = totalPercentPSCurrent;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }

                        //START TOTAL AUTO APPRO

                        if (_MyDbContext.FundSources.Where(x=>x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Any())
                        {

                        var PsTotalAPTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);
                        var PsTotalAP = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 1 && x.FundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.Beginning_balance);

                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL AUTOMATIC APPROPRIATIONS";

                        /*var PTCAP = budget_allotment.FundSources.FirstOrDefault().Beginning_balance;
                        var PsTotalCurrentAP = +(double)PTC;*/


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = PsTotalAPTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = PsTotalAPTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalPSAPTotal = PsTotalAP - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSAPTotal.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        var totalPercentPSAPTotal = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.fundSourceTitle == "AUTOMATIC APPROPRIATION").Sum(x => x.amount) / allotment_total;
                        ws.Cell(currentRow, 10).Value = totalPercentPSAPTotal;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                        }
                        else
                        {
                        }
                        //END TOTAL AUTO APPRO


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
                                                               status = oa.status,
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
                                                        status = oa.status,
                                                        allotmentClassID = f.AllotmentClassId
                                                    }).ToList();

                        if (budget_allotment.FundSources.Where(x=>x.AllotmentClassId == 2 && x.AppropriationId == 1).Any()) { 

                        var MooeTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 2 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                        var allotment_totalMOOE = +MooeTotal;

                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL MOOE";

                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = MooeTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = MooeTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalMOOECurent = MooeTotal - asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOECurent.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        if (asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) == 0 || MooeTotal == 0)
                        {
                            ws.Cell(currentRow, 10).Value = "";
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }
                        else
                        {
                            var totalPercentMOOECurrent = asAtTotalinTotalMOOE.Where(x => x.allotmentClassID == 2).Sum(x => x.amount) / MooeTotal;
                            ws.Cell(currentRow, 10).Value = totalPercentMOOECurrent;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            currentRow++;
                        }

                        }


                        //CO

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
                                                             status = oa.status,
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
                                                      status = oa.status,
                                                      allotmentClassID = f.AllotmentClassId
                                                  }).ToList();

                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Any())
                        {

                            var CoTotal = _MyDbContext.FundSources.Where(x => x.AllotmentClassId == 3 && x.AppropriationId == 1).Sum(x => x.Beginning_balance);
                        var allotment_totalCO = +CoTotal;

                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL CO";


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = CoTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = CoTotal.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalCOCurrent = CoTotal - asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCOCurrent.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        if (asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) == 0 && CoTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentCOCurrent = asAtTotalinTotalCO.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / CoTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentCOCurrent;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }


                        if (_MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 1 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL PS SAA";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaa.ToString("N", new CultureInfo("en-US"));

                            if (asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) == 0 && PsTotalSaa == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else
                            {
                                var totalPercentPSSaaTotalinTotal = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount) / PsTotalSaa;
                                //PERCENT OF UTILIZATION
                                ws.Cell(currentRow, 10).Value = totalPercentPSSaaTotalinTotal;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }


                        //TOTAL MOOE SAA

                        //var MooeTotalSaa = _MyDbContext.Sub_allotment.Where(x => x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                        //var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        //var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;


                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2).Any()) {

                            var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                            var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;

                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL MOOE SAA";




                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOESaa.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            ws.Cell(currentRow, 10).Value = totalPercentMOOESaa;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;

                        }


                        //TOTAL CO SAA


                        if(budget_allotment.SubAllotment.Where(x=>x.AllotmentClassId == 3).Any())
                        {

                        

                        var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
                        var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;


                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL CO SAA";




                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCOSaa.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = totalPercentCOSaa;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        }



                        var TotalCA = _MyDbContext.FundSources.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 1).Sum(x => x.Beginning_balance);

                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL CURRENT APPROPRIATION";
                        ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                        ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.AppleGreen;

                        ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = TotalCA.ToString("N", new CultureInfo("en-US"));

                        //TOTAL FUNDSOURCE - TOTAL AFTER REALIGMENT
                        ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = TotalCA.ToString("N", new CultureInfo("en-US"));

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
                                                                  status = oa.status,
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
                                                           status = oa.status,
                                                           allotmentClassID = f.AllotmentClassId,
                                                           appropriationID = f.AppropriationId
                                                       }).ToList();

                        var CurrentTotal = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);

                        //TOTAL - FOR THE MONTH
                        var CurrentApproForthemonth = fortheMonthTotalinTotalCURRENT.Where(x=>x.appropriationID == 1).Sum(x => x.amount) + fortheMonthTotalinTotalPS.Where(x => x.appropriationID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        var CurrentApproAsat = asAtTotalinTotalCURRENT.Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = CurrentApproForthemonth.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = CurrentApproAsat.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                        var unobligatedTotalinTotalCURRENT = TotalCA - asAtTotalinTotalCURRENT.Sum(x => x.amount);
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCURRENT.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        var totalPercentCURRENT = asAtTotalinTotalCURRENT.Sum(x => x.amount) / CurrentTotal;
                        ws.Cell(currentRow, 10).Value = totalPercentCURRENT;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2).Any() || _MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 2).Any())
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
                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP PERSONNEL SERVICES";


                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSConap.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            if (asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) == 0 && PsConapTotal == 0)
                            {
                                ws.Cell(currentRow, 10).Value = "";
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                            else{
                                var totalPercentPSConapTotal = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentPSConapTotal;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 2).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP MOOE";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = MooeConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = MooeConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalMooeConap.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMooeConap.ToString("N", new CultureInfo("en-US"));

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
                                var totalPercentMooeConap = asAtTotalinTotalPSConap.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2).Sum(x => x.amount) / PsConapTotal;
                                ws.Cell(currentRow, 10).Value = totalPercentMooeConap;
                                ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                                ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }
                        }
                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 3).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP CO";

                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = CoConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = CoConapTotal.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalCoConap.Where(x => x.allotmentClassID == 3 && x.appropriationID == 2).Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCoConap.ToString("N", new CultureInfo("en-US"));

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
                        if (_MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 2 && x.AllotmentClassId == 1).Any())
                        {
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONAP PS SAA";

                            var PsTotalSaaConap = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 1 && x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = PsTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = PsTotalSaaConap.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - FOR THE MONTH
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalPSSaaConapTotal = PsTotalSaaConap - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalPSSaaConapTotal.ToString("N", new CultureInfo("en-US"));

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


                        //TOTAL MOOE SAA

                        //var MooeTotalSaa = _MyDbContext.Sub_allotment.Where(x => x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                        //var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        //var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;

                        if (budget_allotment.FundSources.Where(x => x.AllotmentClassId == 2).Any()) { 

                        var MooeTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 2).Sum(x => x.Beginning_balance);
                        var unobligatedTotalinTotalMOOESaa = MooeTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        var totalPercentMOOESaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;

                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL CONAP MOOE SAA";


                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = MooeTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalMOOESaa.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = totalPercentMOOESaa;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        }


                        //TOTAL CO SAA

                        if (budget_allotment.SubAllotment.Where(x=>x.AllotmentClassId == 3 && x.AppropriationId == 2).Any())
                        {

                        var CoTotalSaa = _MyDbContext.SubAllotment.Where(x => x.AllotmentClassId == 3).Sum(x => x.Beginning_balance);
                        var unobligatedTotalinTotalCOSaa = CoTotalSaa - asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                        var totalPercentCOSaa = (double)asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3).Sum(x => x.amount) / allotment_total;


                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL CONAP CO SAA";




                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - TOTAL AFTER REALIGNMENT
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = CoTotalSaa.ToString("N", new CultureInfo("en-US"));

                        //TOTAL - FOR THE MONTH
                        ws.Cell(currentRow, 7).Style.Font.SetBold();
                        ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 7).Value = fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - AS AT
                        ws.Cell(currentRow, 8).Style.Font.SetBold();
                        ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 8).Value = asAtTotalinTotalPS.Where(x => x.allotmentClassID == 3 && x.sourceType == "sub_allotment").Sum(x => x.amount).ToString("N", new CultureInfo("en-US"));

                        //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT

                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCOSaa.ToString("N", new CultureInfo("en-US"));

                        //PERCENT OF UTILIZATION
                        ws.Cell(currentRow, 10).Value = totalPercentCOSaa;
                        ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                        ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                        currentRow++;

                        }


                        if (_MyDbContext.FundSources.Where(x=>x.AppropriationId == 2).Any() || _MyDbContext.SubAllotment.Where(x=>x.AppropriationId == 2).Any())
                        {
                            var TotalCONAP = _MyDbContext.FundSources.Where(x => x.AppropriationId == 2).Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Where(x => x.AppropriationId == 2).Sum(x => x.Beginning_balance);
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(currentRow, 1).Value = "TOTAL CONTINUING APPROPRIATION";
                            ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.DarkOrange;
                            ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.DarkOrange;

                            ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = TotalCONAP.ToString("N", new CultureInfo("en-US"));

                            //TOTAL FUNDSOURCE - TOTAL AFTER REALIGMENT
                            ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = TotalCONAP.ToString("N", new CultureInfo("en-US"));

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
                                                                    status = oa.status,
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
                                                             status = oa.status,
                                                             allotmentClassID = f.AllotmentClassId,
                                                             appropriationID = f.AppropriationId
                                                         }).ToList();

                            var ConapTotal = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);

                            //TOTAL - FOR THE MONTH
                            var ConapApproForthemonth = fortheMonthTotalinTotalCONAP.Where(x => x.appropriationID == 2).Sum(x => x.amount) + fortheMonthTotalinTotalPS.Where(x => x.allotmentClassID == 1 && x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            var ConapApproAsat = asAtTotalinTotalCONAP.Where(x => x.appropriationID == 2).Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.appropriationID == 2 && x.sourceType == "sub_allotment").Sum(x => x.amount);
                            ws.Cell(currentRow, 7).Style.Font.SetBold();
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 7).Value = ConapApproForthemonth.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - AS AT
                            ws.Cell(currentRow, 8).Style.Font.SetBold();
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 8).Value = ConapApproAsat.ToString("N", new CultureInfo("en-US"));

                            //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                            var unobligatedTotalinTotalCONAP = TotalCONAP - ConapApproAsat;
                            ws.Cell(currentRow, 9).Style.Font.SetBold();
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 9).Value = unobligatedTotalinTotalCONAP.ToString("N", new CultureInfo("en-US"));

                            //PERCENT OF UTILIZATION
                            var totalPercentCONAP = ConapApproAsat / ConapTotal;
                            ws.Cell(currentRow, 10).Value = totalPercentCONAP;
                            ws.Cell(currentRow, 10).Style.NumberFormat.Format = "0.00%";
                            ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            //END CONAP SUMMARY
                        }

                    }

                    currentRow++;

                    /*ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 2;
                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    *//*ws.Cell(currentRow, 1).Value = "TOTAL" + " " + "SAA" + " " + budget_allotment.Allotment_code.ToString();*//*


                    //TOTAL SAA - TOTAL AFTER REALIGNMENT
                    ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = suballotment_total.ToString("N", new CultureInfo("en-US"));*/

                }


                currentRow++;
                currentRow++;

                var GrandTotals = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);

                ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                ws.Cell(currentRow, 1).Style.Font.SetBold();
                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                ws.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 1).Value = "GRANDTOTAL";
                //ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.Cyan;
                ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.DarkCyan;
                ws.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.DarkCyan;



                //currentRow++;
                ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                ws.Cell(currentRow, 3).Style.Font.SetBold();
                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 3).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 3).Value = GrandTotals.ToString("N", new CultureInfo("en-US"));

                //GRANDTOTAL - TOTAL AFTER REALIGNMENT
                ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                ws.Cell(currentRow, 6).Style.Font.SetBold();
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 6).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 6).Value = GrandTotals.ToString("N", new CultureInfo("en-US"));

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
                                                        status = oa.status,
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
                                                 status = oa.status,
                                                 allotmentClassID = f.AllotmentClassId,
                                                 appropriationID = f.AppropriationId
                                             }).ToList();

                var ConapTotalGrand = _MyDbContext.FundSources.Sum(x => x.Beginning_balance) + _MyDbContext.SubAllotment.Sum(x => x.Beginning_balance);
                var ConapApproForthemonthGrand = fortheMonthTotalinTotalCONAPGrand.Where(x => x.appropriationID == 1).Sum(x => x.amount) + fortheMonthTotalinTotalCONAPGrand.Where(x => x.appropriationID == 2).Sum(x => x.amount);
                var ConapApproAsatGrand = asAtTotalinTotalCONAPGrand.Where(x => x.appropriationID == 1).Sum(x => x.amount) + asAtTotalinTotalPS.Where(x => x.appropriationID == 2).Sum(x => x.amount);
                ws.Cell(currentRow, 7).Style.Font.SetBold();
                ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 7).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 7).Value = ConapApproForthemonthGrand.ToString("N", new CultureInfo("en-US"));

                ws.Cell(currentRow, 8).Style.Font.SetBold();
                ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 8).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 8).Value = ConapApproAsatGrand.ToString("N", new CultureInfo("en-US"));


                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "JAN-DEC.xlsx");
                }
            }
        }

        public IActionResult DownloadSaob()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            return View();
        }
    }
}

//JOHNS UPDATE