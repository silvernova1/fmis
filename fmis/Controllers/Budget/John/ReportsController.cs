using ClosedXML.Excel;
using fmis.Data;
using fmis.DataHealpers;
using fmis.Filters;
using fmis.Models;
using fmis.Models.John;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
            ViewBag.filter = new FilterSidebar("master_data", "allotmentclass");
            return View(await _MyDbContext.FundSources.ToListAsync());
        }

        [HttpPost]
        public IActionResult Export(string fn)
        {

            DataTable dt = new DataTable("Saob Report");
            /*dt.Columns.AddRange(new DataColumn[11] { new DataColumn("P/A/P / ALLOTMENT CLASS /"),
                                            new DataColumn("EXPENSES CODE"),
                                            new DataColumn("ALLOTMENT RECEIVED"),
                                            new DataColumn("REALIGNMENT"),
                                            new DataColumn("TRANSFER TO"),
                                            new DataColumn("TOTAL AFTER REALIGNMENT"),
                                            new DataColumn("DECEMBER"),
                                            new DataColumn("AS OF DECEMBER"),
                                            new DataColumn("UNOBLIGATED BALANCE OF ALLOTMENT"),
                                            new DataColumn("% OF UTILIZATION"),
                                            new DataColumn("DISBURSEMENT As of December")});*/
            /*  dt.Columns.Add("P/A/P / ALLOTMENT CLASS /");
              dt.Columns.Add("EXPENSES CODE");
              dt.Columns.Add("ALLOTMENT RECEIVED");
              dt.Columns.Add("REALIGNMENT");
              dt.Columns.Add("TRANSFER TO");
              dt.Columns.Add("TOTAL AFTER REALIGNMENT");
              dt.Columns.Add("DECEMBER");
              dt.Columns.Add("AS OF DECEMBER");
              dt.Columns.Add("UNOBLIGATED BALANCE OF ALLOTMENT");
              dt.Columns.Add("% OF UTILIZATION");
              dt.Columns.Add("DISBURSEMENT As of December");*/



            //var ballots = _MyDbContext.Budget_allotments;



            /*  var ballots = (from ba in _MyDbContext.Budget_allotments
                             join fs in _MyDbContext.FundSources
                             on ba.BudgetAllotmentId equals fs.Budget_allotmentBudgetAllotmentId
                             join fsa in _MyDbContext.FundSourceAmount
                             on fs.FundSourceId equals fsa.FundSourceId
                             join U in _MyDbContext.Uacs
                             on fsa.Account_title equals U.Account_title
                             select new
                             {
                                 BaTitle = ba.Allotment_title,
                                 BaCode = ba.Allotment_code,
                                 SelectedFs = fs.Prexc.pap_code1,
                                 fsTitle = fs.FundSourceTitle,
                                 fsaAccTitle = fsa.Account_title,
                                 fsaAmount = fsa.Amount,
                                 UaccCode = U.Expense_code
                             }).ToList();*/




            using (XLWorkbook wb = new XLWorkbook())
            {

                /*var worksheet = wb.Worksheets.Add("Users");
                var currentRow = 1;*/

                /*  worksheet.Cell(currentRow, 1).Value = "Id";
                  worksheet.Cell(currentRow, 2).Value = "Username";*/

                /*var customers = from customer in _MyDbContext.FundSourceAmount.Take(10)
                                select customer;

                foreach (var user in customers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Account_title;
                    worksheet.Cell(currentRow, 2).Value = user.Amount;
                }*/


                var ws = wb.Worksheets.Add(dt);
                ws.Worksheet.SheetView.FreezeColumns(2);
                ws.Worksheet.SheetView.FreezeRows(13);

                IXLRange range = ws.Range(ws.Cell(2, 1).Address, ws.Cell(13, 11).Address);

                range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                var currentRow = 14;
                Double total = 0.00;
                Double allotment_total = 0;

                var co = 2;
                var ro = 1;
                var wsFreeze = ws.Worksheet.Cell("Freeze View");


                /*ws.Cell(++ro, co).Value = "Horizontal = Right";
                ws.Cell(ro, co).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(++ro, co).Value = "Indent = 2";
                ws.Cell(ro, co).Style.Alignment.Indent = 2;

                ws.Cell(++ro, co).Value = "JustifyLastLine = true";
                ws.Cell(ro, co).Style.Alignment.JustifyLastLine = true;

                ws.Cell(++ro, co).Value = "ReadingOrder = ContextDependent";
                ws.Cell(ro, co).Style.Alignment.ReadingOrder = XLAlignmentReadingOrderValues.ContextDependent;

                ws.Cell(++ro, co).Value = "RelativeIndent = 2";
                ws.Cell(ro, co).Style.Alignment.RelativeIndent = 2;

                ws.Cell(++ro, co).Value = "ShrinkToFit = true";
                ws.Cell(ro, co).Style.Alignment.ShrinkToFit = true;

                ws.Cell(++ro, co).Value = "TextRotation = 45";
                ws.Cell(ro, co).Style.Alignment.TextRotation = 45;

                ws.Cell(++ro, co).Value = "TopToBottom = true";
                ws.Cell(ro, co).Style.Alignment.TopToBottom = true;

                ws.Cell(++ro, co).Value = "Vertical = Center";
                ws.Cell(ro, co).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cell(++ro, co).Value = "WrapText = true";
                ws.Cell(ro, co).Style.Alignment.WrapText = true;*/


                ws.Cell("A7").RichText.AddText("Department:");
                ws.Cell("A8").RichText.AddText("Agency/OU:");
                ws.Cell("A9").RichText.AddText("Fund");
                ws.Cell("D7").RichText.AddText("HEALTH");
                ws.Cell("D8").RichText.AddText("REGIONAL OFFICE VII");
                ws.Range("A7:C7").Merge();
                ws.Range("A8:C8").Merge();
                ws.Range("A9:C9").Merge();


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
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("F5").RichText.AddText("DECEMBER 15, 2021");

                ws.Cell("F6").Style.Font.FontSize = 10;
                ws.Cell("F6").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
                ws.Cell("F6").RichText.AddText("(In Pesos)");

                //FIRST ROW
                ws.Cell(11, 1).Style.Font.SetBold();
                ws.Cell(11, 1).Style.Font.FontSize = 12;
                ws.Cell(11, 1).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.White;
                ws.Columns(11, 1).AdjustToContents();
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
                ws.Cell(12, 7).Value = DateTime.Now.ToString("MMMM");
                //ws.Cell(12, 7).Value = DateTime.Now.ToString("MMMM yyyy");

                //SECOND ROW
                ws.Cell(12, 8).Style.Font.SetBold();
                ws.Cell(1, 8).Style.Font.FontSize = 12;
                ws.Cell(1, 8).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell(12, 8).WorksheetColumn().AdjustToContents();
                ws.Cell(12, 8).Value = "As of " + DateTime.Now.ToString("MMMM");

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
                    .ThenInclude(fundsource_amount => fundsource_amount.FundSourceAmounts)
                    .ThenInclude(uacs => uacs.Uacs);


                foreach (Budget_allotment budget_allotment in budget_allotments)
                {
                    ws.Cell(currentRow, 1).Style.Font.SetBold();
                    ws.Cell(currentRow, 1).Style.Font.FontSize = 12;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 1).Value = budget_allotment.Allotment_title.ToUpper().ToString();
                    currentRow++;

                    var fsh = _MyDbContext.FundSources.Where(p => p.Budget_allotmentBudgetAllotmentId == budget_allotment.BudgetAllotmentId).ToString();



                    var ballots = (from ba in _MyDbContext.Budget_allotments
                                   join fs in _MyDbContext.FundSources
                                   on ba.BudgetAllotmentId equals fs.Budget_allotmentBudgetAllotmentId
                                   join fsa in _MyDbContext.FundSourceAmount
                                   on fs.FundSourceId equals fsa.FundSourceId
                                   join U in _MyDbContext.Uacs
                                   on fsa.Account_title equals U.Account_title
                                   select new
                                   {
                                       BaTitle = ba.Allotment_title,
                                       BaCode = ba.Allotment_code,
                                       SelectedFs = fs.Prexc.pap_code1,
                                       fsTitle = fs.FundSourceTitle,
                                       fsaAccTitle = fsa.Account_title,
                                       fsaAmount = fsa.Amount,
                                       UaccCode = U.Expense_code
                                   }).ToList();


                    var query = (from fundsource in _MyDbContext.FundSources
                                 join prexc in _MyDbContext.Prexc
                                 on fundsource.PrexcId equals prexc.Id
                                 select new
                                 {
                                     papcode1 = prexc.pap_code1
                                 }).ToList();

                    
                    foreach (FundSource fundSource in budget_allotment.FundSources)
                    {
                        
                        foreach (var uacs in query)
                        {
                                ws.Cell(currentRow, 1).Value = uacs.papcode1;
                                ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                currentRow++;
                        }


                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = fundSource.FundSourceTitle.ToUpper().ToString();
                        currentRow++;
                        


                        foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts)
                        {
                            total = 0;

                            ws.Cell(currentRow, 1).Value = fundsource_amount.Account_title.ToUpper().ToString();
                            ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                            ws.Cell(currentRow, 3).Value = fundsource_amount.Amount.ToString("N", new CultureInfo("en-US"));
                            ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                            ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                            currentRow++;
                            total = (double)fundsource_amount.Amount;
                            
                        }

                        allotment_total += (double)fundSource.Remaining_balance;

                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Value = "SUBTOTAL " + fundSource.FundSourceTitle.ToUpper() + " - " + budget_allotment.Allotment_title.ToUpper();


                        ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 3).Style.Font.SetBold();
                        ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 3).Value = fundSource.Remaining_balance;




                        currentRow++;


                        ws.Cell(currentRow, 1).Style.Font.FontName = "TAHOMA";
                        ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 1).Style.Font.SetBold();
                        ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws.Cell(currentRow, 1).Value = "TOTAL" + " " + budget_allotment.Allotment_title.ToUpper().ToString();
                        

                    }
                    ws.Cell(currentRow, 3).Style.Font.FontName = "TAHOMA";
                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.SetBold();
                    ws.Cell(currentRow, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    ws.Cell(currentRow, 3).Value = allotment_total.ToString("N", new CultureInfo("en-US"));
                }

                



                /* foreach (FundSource fundSource in budget_allotment.FundSources)
                 {
                     ws.Cell(20, 1).Style.Font.SetBold();
                     ws.Cell(20, 1).Style.Font.FontSize = 10;
                     ws.Cell(20, 1).Value = "SUBTOTAL" + " " + fundSource.FundSourceTitle + "-" + budget_allotment.Allotment_code;
                     ws.Cell(20, 1).Style.Alignment.Indent = 2;

                     ws.Cell(17, 1).Style.Font.SetBold();
                     ws.Cell(17, 1).Style.Font.FontSize = 10;
                     ws.Cell(17, 1).Value = fundSource.FundSourceTitle;
                     ws.Cell(17, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);


                     ws.Cell(16, 1).Value = fundSource.PrexcId;
                     ws.Cell(16, 1).SetDataType(XLDataType.Number);
                     ws.Cell(16, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                     foreach (FundSourceAmount fundsource_amount in fundSource.FundSourceAmounts)
                     {
                         currentRow++;

                         ws.Cell(currentRow, 1).Value = fundsource_amount.Account_title;
                         ws.Cell(currentRow, 1).Style.Alignment.Indent = 3;

                         *//*ws.Cell(currentRow, 2).Value = 
                         ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);*//*

                         ws.Cell(currentRow, 3).Value = fundsource_amount.Amount;
                         ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(currentRow, 6).Value = fundsource_amount.Amount;
                         ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(currentRow, 9).Value = fundsource_amount.Amount;
                         ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(currentRow, 10).Value = fundsource_amount.Amount;
                         ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell("C20").Style.Font.SetBold();
                         ws.Cell("C20").FormulaA1 = "SUM(C18:C19)";
                         ws.Cell("C20").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell("F20").Style.Font.SetBold();
                         ws.Cell("F20").FormulaA1 = "SUM(F18:F19)";
                         ws.Cell("F20").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell("I20").Style.Font.SetBold();
                         ws.Cell("I20").FormulaA1 = "SUM(I18:I19)";
                         ws.Cell("I20").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell("J20").Style.Font.SetBold();
                         ws.Cell("J20").FormulaA1 = "SUM(J18:J19)";
                         ws.Cell("J20").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);



                         foreach (Uacs uacs in fundsource_amount.Uacs.Where(uacs=> uacs.Account_title == fundsource_amount.Account_title))
                         {

                             ws.Cell(currentRow, 2).Value = uacs.Expense_code;
                             ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                             *//* currentRow++;
                              ws.Cell(currentRow, 1).Value = fundsource_amount.Account_title;
                              ws.Cell(currentRow, 2).Value = fundsource_amount.Account_title;*/





                /* var uacsdata = from fs in _MyDbContext.FundSources
                                join fsa in _MyDbContext.FundSourceAmount
                                on fs.FundSourceId equals fsa.FundSourceId
                                select fs;*//*


                //  wsFreeze.Worksheet.SheetView.FreezeColumns();

                //ws.Range(ws.Cell(row, col++), ws.Cell(row, col++)).Merge();




                // ws.PageSetup.CenterHorizontally = 1;





                //WITH DB DATA




                *//*    foreach (var ballot in ballots)
                    {*/


















                /*ws.Cell(16, 1).Value = ballot.SelectedFs;
                ws.Cell(16, 1).SetDataType(XLDataType.Number);
                ws.Cell(16, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);*/

                /*ws.Cell(16, 1).Style.Font.SetBold();
                ws.Cell(16, 1).Style.Font.FontSize = 14;
                ws.Cell(16, 1).Value = "Total" + " " + ballot.BaTitle;
                ws.Cell(16, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("A16:B16").Merge();*//*

                //DISBURSEMENT COLUMNS
              *//*  ws.Cell(16, 11).Style.Font.SetBold();
                ws.Cell(16, 11).Style.Font.FontSize = 10;
                ws.Cell(16, 11).Style.NumberFormat.Format = "0.00";
                ws.Cell(16, 11).Value = "0";
                ws.Cell(16, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);*/



                /* ws.Cell(17, 1).Style.Font.SetBold();
                 ws.Cell(17, 1).Style.Font.FontSize = 10;
                 ws.Cell(17, 1).Value = ballot.fsTitle;
                 ws.Cell(17, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);*/


                /*         ws.Cell(18, 1).Style.Font.FontSize = 14;
                         ws.Cell(18, 1).Style.Font.FontSize = 10;
                        // ws.Cell(18, 1).Value = fundsource_amount.Account_title;
                         ws.Cell(18, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                         ws.Cell(19, 1).Style.Font.FontSize = 14;
                         ws.Cell(19, 1).Style.Font.FontSize = 10;
                        // ws.Cell(19, 1).Value = fundsource_amount.Account_title;
                         ws.Cell(19, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                         ws.Cell(18, 2).Style.Font.FontSize = 14;
                         ws.Cell(18, 2).Style.Font.FontSize = 10;
                         ws.Cell(18, 2).Value = ballot.UaccCode;
                         ws.Cell(18, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);



                         ws.Cell(18, 3).Style.Font.FontSize = 14;
                         ws.Cell(18, 3).Style.Font.FontSize = 10;
                         ws.Cell(18, 3).Value = ballot.fsaAmount;
                         ws.Cell(18, 3).Style.NumberFormat.Format = "0.00";
                         ws.Cell(18, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell(18, 6).Style.Font.FontSize = 14;
                         ws.Cell(18, 6).Style.Font.FontSize = 10;
                         ws.Cell(18, 6).Value = ballot.fsaAmount;
                         ws.Cell(18, 6).Style.NumberFormat.Format = "0.00";
                         ws.Cell(18, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell(18, 9).Style.Font.FontSize = 14;
                         ws.Cell(18, 9).Style.Font.FontSize = 10;
                         ws.Cell(18, 9).Value = ballot.fsaAmount;
                         ws.Cell(18, 9).Style.NumberFormat.Format = "0.00";
                         ws.Cell(18, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell(18, 10).Style.Font.FontSize = 14;
                         ws.Cell(18, 10).Style.Font.FontSize = 10;
                         ws.Cell(18, 10).Value = "0";
                         ws.Cell(18, 10).Style.NumberFormat.Format = "0.00%";
                         ws.Cell(18, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell(20, 1).Style.Font.SetBold();
                         ws.Cell(20, 1).Style.Font.FontSize = 10;
                         ws.Cell(20, 1).Value = "SUBTOTAL" + " " + ballot.fsTitle + "-" + ballot.BaCode;
                         ws.Cell(20, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                         ws.Cell("C20").Style.Font.SetBold();
                         ws.Cell("C20").FormulaA1 = "SUM(C18:C19)";


                         ws.Cell(19, 1).Style.Font.FontSize = 14;
                         ws.Cell(19, 1).Style.Font.FontSize = 10;
                         ws.Cell(19, 1).Value = ballot.fsaAccTitle;
                         ws.Cell(19, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                         ws.Cell(19, 2).Style.Font.FontSize = 14;
                         ws.Cell(19, 2).Style.Font.FontSize = 10;
                         ws.Cell(19, 2).Value = ballot.UaccCode;
                         ws.Cell(19, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);



                                             ws.Cell(19, 1).Style.Font.SetBold();
                                             ws.Cell(19, 1).Style.Font.FontSize = 10;
                                             ws.Cell(19, 1).Value = "Total" + " " + ballot.BaCode;
                                             ws.Cell(19, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                         // ws.Range("A15:B15").Merge();

                         ws.Cell(19, 3).Style.Font.FontSize = 10;
                         ws.Cell(19, 3).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 3).Value = "100";
                         ws.Cell(19, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         ws.Cell(19, 6).Style.Font.SetBold();
                         ws.Cell(19, 6).Style.Font.FontSize = 10;
                         ws.Cell(19, 6).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 6).Value = "100";
                         ws.Cell(19, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(19, 7).Style.Font.SetBold();
                         ws.Cell(19, 7).Style.Font.FontSize = 10;
                         ws.Cell(19, 7).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 7).Value = "0";
                         ws.Cell(19, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(19, 8).Style.Font.SetBold();
                         ws.Cell(19, 8).Style.Font.FontSize = 10;
                         ws.Cell(19, 8).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 8).Value = "0";
                         ws.Cell(19, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(19, 9).Style.Font.SetBold();
                         ws.Cell(19, 9).Style.Font.FontSize = 10;
                         ws.Cell(19, 9).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 9).Value = "100";
                         ws.Cell(19, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         //DISBURSEMENT COLUMNS
                         ws.Cell(19, 11).Style.Font.SetBold();
                         ws.Cell(19, 11).Style.Font.FontSize = 10;
                         ws.Cell(19, 11).Style.NumberFormat.Format = "0.00";
                         ws.Cell(19, 11).Value = "0";
                         ws.Cell(19, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);



                         ws.Cell(21, 1).Style.Font.SetBold();
                         ws.Cell(21, 1).Style.Font.FontSize = 12;
                         ws.Cell(21, 1).Value = "GRAND TOTAL";


                         ws.Cell(21, 3).Style.Font.SetBold();
                         ws.Cell(21, 3).Style.Font.FontSize = 10;
                         ws.Cell(21, 3).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 3).Value = "0";
                         ws.Cell(21, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(21, 6).Style.Font.SetBold();
                         ws.Cell(21, 6).Style.Font.FontSize = 10;
                         ws.Cell(21, 6).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 6).Value = "0";
                         ws.Cell(21, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(21, 7).Style.Font.SetBold();
                         ws.Cell(21, 7).Style.Font.FontSize = 10;
                         ws.Cell(21, 7).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 7).Value = "0";
                         ws.Cell(21, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(21, 8).Style.Font.SetBold();
                         ws.Cell(21, 8).Style.Font.FontSize = 10;
                         ws.Cell(21, 8).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 8).Value = "0";
                         ws.Cell(21, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                         ws.Cell(21, 9).Style.Font.SetBold();
                         ws.Cell(21, 9).Style.Font.FontSize = 10;
                         ws.Cell(21, 9).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 9).Value = "0";
                         ws.Cell(21, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                         //DISBURSEMENT COLUMNS
                         ws.Cell(21, 11).Style.Font.SetBold();
                         ws.Cell(21, 11).Style.Font.FontSize = 10;
                         ws.Cell(21, 11).Style.NumberFormat.Format = "0.00";
                         ws.Cell(21, 11).Value = "0";
                         ws.Cell(21, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);*//*
            }
        }
        }*/
                //}
                //}



                //var CustomCells = ws.Cells().Style.Fill.SetBackgroundColor(XLColor.Gray);


                //Cell backgound
                /*ws.Cell(14, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(15, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(16, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(17, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(18, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(19, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(20, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(21, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(22, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(23, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(24, 1).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(25, 1).Style.Fill.BackgroundColor = XLColor.Gray;

                ws.Cell(14, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(15, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(16, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(17, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(18, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(19, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(20, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(21, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(22, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(23, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(24, 2).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(25, 2).Style.Fill.BackgroundColor = XLColor.Gray;

                ws.Cell(14, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(15, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(16, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(17, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(18, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(19, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(20, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(21, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(22, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(23, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(24, 3).Style.Fill.BackgroundColor = XLColor.Gray;
                ws.Cell(25, 3).Style.Fill.BackgroundColor = XLColor.Gray;*/


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Saob Report.xlsx");
                }
            }
        }





        public IActionResult DownloadSaob()
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment");

            var allotments = (from list in _MyDbContext.Yearly_reference where list.YearlyReference == "2021" select list).ToList();
            return PartialView(allotments);
        }
    }
}