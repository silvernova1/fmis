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
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass","");
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
            var firstDayOfMonth = new DateTime(date1.Year, date1.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime lastday = Convert.ToDateTime(lastDayOfMonth);
            lastday.ToString("yyyy-MM-dd 23:59:59");


            var dateTime = _MyDbContext.FundSources.Where(x => x.CreatedAt >= date1 && x.CreatedAt <= dateTomorrow).Select(y => new { y.FundSourceTitle} );





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
                Double suballotment_total = 0;
                Double GrandTotal = 0;


                var wsFreeze = ws.Worksheet.Cell("Freeze View");



                ws.Worksheet.SheetView.FreezeColumns(1);
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
                ws.Cell(11, 6).RichText.AddText("TOTAL AFTER");
                ws.Cell(11, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                //SECOND ROW
                ws.Cell(12, 6).Style.Font.SetBold();
                ws.Cell(12, 6).RichText.AddText("REALIGNMENT");
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
                        .ThenInclude(uacs => uacs.Uacs);

               // var realignment_amount = 50;
               

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
                    ws.Cell(currentRow, 1).Value = budget_allotment.FundSources.FirstOrDefault().Appropriation.AppropriationSource + "APPROPRIATION";
                    currentRow++;


                    //ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Value = budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Desc;
                    currentRow++;


                    var query = (from fundsource in _MyDbContext.FundSources
                                 join prexc in _MyDbContext.Prexc
                                 on fundsource.PrexcId equals prexc.Id
                                 select new
                                 {
                                     papcode1 = Convert.ToString(prexc.pap_code1)
                                 }).ToList();


                    foreach (FundSource fundSource in budget_allotment.FundSources.Where(x => x.CreatedAt >= date1 && x.CreatedAt <= dateTomorrow))
                    {

                        ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == fundSource.PrexcId)?.pap_code1;
                        ws.Cell(currentRow, 1).Style.NumberFormat.Format = "00";
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                        //ws.Cell(currentRow, 1).Value = dateTime.FirstOrDefault().FundSourceTitle.ToUpper().ToString();
                        currentRow++;

                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts.Where(x=>x.status == "activated"))
                        {
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
                            ws.Cell(currentRow, 4).Value = "("+ fundsource_amount.realignment_amount.ToString("N", new CultureInfo("en-US"))+")";
                            ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Value = afterrealignment_amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (AS OF THE MONTH)
                            ws.Cell(currentRow, 7).Value = _MyDbContext.ObligationAmount.Where(x=>x.CreatedAt >= date1 && x.CreatedAt <= lastday).FirstOrDefault(x => x.UacsId == fundsource_amount.UacsId)?.Amount.ToString("N", new CultureInfo("en-US"));
                            //ws.Cell(currentRow, 7).Value = _MyDbContext.Obligation.Where(x => x.Date >= date1 && x.CreatedAt <= lastday).FirstOrDefault(x => x.source_id == fundsource_amount.UacsId).ObligationAmounts.FirstOrDefault().Amount;
                            ws.Cell(currentRow, 7).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //OBLIGATED (AST AT)
                            ws.Cell(currentRow, 8).Value = _MyDbContext.ObligationAmount.FirstOrDefault(x=>x.UacsId == fundsource_amount.UacsId)?.Amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 8).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            //UNOBLIGATED BALANCE OF ALLOTMENT
                            ws.Cell(currentRow, 9).Value = fundsource_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


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

                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper() + " - " + budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Account_Code;

                        ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = fundSource.Beginning_balance;

                        ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = fundSource.realignment_amount;


                        //SUBTOTAL UNOBLIGATED BALANCE OF ALLOTMENT
                        ws.Cell(currentRow, 9).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 9).Style.Font.SetBold();
                        ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws.Cell(currentRow, 9).Value = fundSource.Beginning_balance;

                        allotment_total += (double)fundSource.Beginning_balance;

                        currentRow++;


                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 4;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 1).Value = "TOTAL" + " " /*+ budget_allotment.Allotment_code.ToUpper().ToString()*/;

                    }

                    
                    ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = allotment_total.ToString("N", new CultureInfo("en-US"));

                    //TOTAL - TOTAL AFTER REALIGNMENT
                    ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 6).Style.Font.SetBold();
                    ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 6).Value = allotment_total.ToString("N", new CultureInfo("en-US"));

                    //TOTAL - UNOBLIGATED BALANCE OF ALLOTMENT
                    ws.Cell(currentRow, 9).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 9).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 9).Style.Font.SetBold();
                    ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 9).Value = allotment_total.ToString("N", new CultureInfo("en-US"));

                    currentRow++;


                    var saa = _MyDbContext.Budget_allotments
                        .Include(sub_allotment => sub_allotment.Sub_allotments)
                        .ThenInclude(suballotment_amount => suballotment_amount.SubAllotmentAmounts)
                        .ThenInclude(uacs => uacs.Uacs);
                    
                        
                    foreach (BudgetAllotment b in saa)
                    {

                        /*ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.OrangePeel;
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


                        //ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Value = budget_allotment.FundSources.FirstOrDefault().AllotmentClass.Desc;
                        currentRow++;*/

                        foreach (Sub_allotment sa in b.Sub_allotments.Where(x => x.CreatedAt >= date1 && x.CreatedAt <= dateTomorrow))
                        {
                            //Double suballotment_total = 0;

                            currentRow++;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 12;
                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Value = "SUB-ALLOTMENT" + "-" /*+ b.Allotment_code.ToUpper().ToString()*/;
                            currentRow++;


                            ws.Cell(currentRow, 1).Value = _MyDbContext.Prexc.FirstOrDefault(x => x.Id == sa.prexcId)?.pap_code1;
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
/*                            ws.Cell(currentRow, 1).Value = sa.Description.ToString();
*/                            ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                            currentRow++;

                            foreach (Suballotment_amount suballotment_amount in sa.SubAllotmentAmounts)
                            {
                                var afterrealignment_suballotmentamount = suballotment_amount.beginning_balance - suballotment_amount.realignment_amount;

                                ws.Cell(currentRow, 1).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Account_title.ToUpper().ToString();
                                ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 2).Value = _MyDbContext.Uacs.FirstOrDefault(x => x.UacsId == suballotment_amount.UacsId)?.Expense_code;
                                ws.Cell(currentRow, 2).Style.Alignment.Indent = 3;

                                ws.Cell(currentRow, 3).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //REALIGNMENT AMOUNT
                                ws.Cell(currentRow, 4).Value = "(" + suballotment_amount.realignment_amount.ToString("N", new CultureInfo("en-US")) + ")";
                                ws.Cell(currentRow, 4).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //TOTAL AFTER REALIGNMENT
                                ws.Cell(currentRow, 6).Value = afterrealignment_suballotmentamount.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                                //UNOBLIGATED BALANCE OF ALLOTMENT
                                ws.Cell(currentRow, 9).Value = suballotment_amount.beginning_balance.ToString("N", new CultureInfo("en-US"));
                                ws.Cell(currentRow, 9).Style.NumberFormat.Format = "0.00";
                                ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                currentRow++;
                            }

                            suballotment_total += (double)sa.Remaining_balance;

                            ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;
                            ws.Cell(currentRow, 1).Style.Font.SetBold();
                            ws.Cell(currentRow, 1).Value = "SUBTOTAL " + sa.Suballotment_title.ToUpper() + " - " + sa.Suballotment_code.ToUpper();

                            ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 3).Style.Font.SetBold();
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 3).Value = sa.Remaining_balance.ToString("N", new CultureInfo("en-US"));

                            //SUBTOTAL - TOTAL AFTER REALIGNMENT
                            ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                            ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                            ws.Cell(currentRow, 6).Style.Font.SetBold();
                            ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(currentRow, 6).Value = sa.Remaining_balance.ToString("N", new CultureInfo("en-US"));

                            currentRow++;

                        }

                        currentRow++;

                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                        ws.Cell(currentRow, 1).Style.Alignment.Indent = 2;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                        ws.Cell(currentRow, 1).Value = "TOTAL" + " " /*+ budget_allotment.Allotment_code.ToString()*/;

                        ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = allotment_total.ToString("N", new CultureInfo("en-US"));

                        //TOTAL FUNDSOURCE - TOTAL AFTER REALIGMENT
                        ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 6).Style.Font.SetBold();
                        ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 6).Value = allotment_total.ToString("N", new CultureInfo("en-US"));

                    }

                    currentRow++;

                    ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                    ws.Cell(currentRow, 1).Style.Alignment.Indent = 2;
                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                    /*ws.Cell(currentRow, 1).Value = "TOTAL" + " " + "SAA" + " " + budget_allotment.Allotment_code.ToString();*/

                    ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = suballotment_total.ToString("N", new CultureInfo("en-US"));

                    //TOTAL SAA - TOTAL AFTER REALIGNMENT
                    ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = suballotment_total.ToString("N", new CultureInfo("en-US"));

                }
                currentRow++;
                currentRow++;

                GrandTotal = allotment_total + suballotment_total;

                ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 1).Style.Font.FontSize = 9;
                ws.Cell(currentRow, 1).Style.Font.SetBold();
                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                ws.Cell(currentRow, 1).Value = "GRANDTOTAL";


                //currentRow++;
                ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                ws.Cell(currentRow, 3).Style.Font.SetBold();
                ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 3).Value = GrandTotal.ToString("N", new CultureInfo("en-US"));

                //GRANDTOTAL - TOTAL AFTER REALIGNMENT
                ws.Cell(currentRow, 6).Style.Font.FontName = "TAHOMA";
                ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                ws.Cell(currentRow, 6).Style.Font.SetBold();
                ws.Cell(currentRow, 6).Style.NumberFormat.Format = "0.00";
                ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(currentRow, 6).Value = GrandTotal.ToString("N", new CultureInfo("en-US"));

                
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
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment","");

            /*var allotments = (from list in _MyDbContext.Yearly_reference where list.YearlyReference == "2021" select list).ToList();
            return PartialView(allotments);*/
            return View();
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadSaob(FormCollection collection)
        {
            String date_from = collection.Get("date_from");
            String date_to = collection.Get("date_to");
            FileStreamResult fsResult = null;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                SaobExcel saobexcel = new SaobExcel();
                //saobexcel.ExcelEPP();
                saobexcel.CreateExcel(date_from, date_to);
                var filesStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(), FileMode.Open);
                fsResult = new FileStreamResult(filesStream, contentType);
            return fsResult;

        }*/
    }
}