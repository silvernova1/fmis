using ClosedXML.Excel;
using fmis.Data;
using fmis.DataHealpers;
using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
        public FileResult Export()
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





            /*var customers = from customer in _MyDbContext.FundSources.ToList()
                            select customer;*/


            var ballots = from ballot in _MyDbContext.Budget_allotments.ToList()
                            select ballot;

            /*foreach (var customer in customers)
            {
                dt.Rows.Add(customer.FundSourceId, customer.FundSourceTitle, customer.FundSourceTitleCode);
            }*/

            using (XLWorkbook wb = new XLWorkbook())
            {

                int row = 1;
                int col = 1;




                var ws = wb.Worksheets.Add(dt);
                var wsFreeze = ws.Worksheet.Cell("Freeze View");


                //  wsFreeze.Worksheet.SheetView.FreezeColumns();

                //ws.Range(ws.Cell(row, col++), ws.Cell(row, col++)).Merge();

                ws.Worksheet.SheetView.FreezeColumns(2);
                ws.Worksheet.SheetView.FreezeRows(13);

                IXLRange range = ws.Range(ws.Cell(2, 1).Address, ws.Cell(13, 11).Address);

                range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                // ws.PageSetup.CenterHorizontally = 1;


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
                ws.Cell(12, 7).RichText.AddText("December");

                //SECOND ROW
                ws.Cell(12, 8).Style.Font.SetBold();
                ws.Cell(1, 8).Style.Font.FontSize = 12;
                ws.Cell(1, 8).Style.Font.SetFontColor(XLColor.RichBlack);
                ws.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.White;
                ws.Cell(12, 8).WorksheetColumn().AdjustToContents();
                ws.Cell(12, 8).RichText.AddText("As Of December");

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


                foreach (var ballot in ballots)
                    {
                    ws.Cell(15, 1).Style.Font.SetBold();
                    ws.Cell(15, 1).Style.Font.FontSize = 14;
                    ws.Cell(15, 1).Value = ballot.Allotment_code;

                    ws.Cell(16, 1).Style.Font.SetBold();
                    ws.Cell(16, 1).Style.Font.FontSize = 14;
                    ws.Cell(16, 1).Value = "Total" + " " + ballot.Allotment_title;
                    ws.Cell(16, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Range("A16:B16").Merge();

                    ws.Cell(16, 11).Style.Font.SetBold();
                    ws.Cell(16, 11).Style.Font.FontSize = 10;
                    ws.Cell(16, 11).Style.NumberFormat.Format = "0.00";
                    ws.Cell(16, 11).Value = "0";
                    ws.Cell(16, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


                    ws.Cell(19, 1).Style.Font.SetBold();
                    ws.Cell(19, 1).Style.Font.FontSize = 10;
                    ws.Cell(19, 1).Value = "Total" + " " + ballot.Allotment_title;                  
                    ws.Cell(19, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    // ws.Range("A15:B15").Merge();

                    ws.Cell(19, 3).Style.Font.SetBold();
                    ws.Cell(19, 3).Style.Font.FontSize = 10;
                    ws.Cell(19, 3).Style.NumberFormat.Format = "0.00";
                    ws.Cell(19, 3).Value = "0";
                    ws.Cell(19, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    ws.Cell(19, 6).Style.Font.SetBold();
                    ws.Cell(19, 6).Style.Font.FontSize = 10;
                    ws.Cell(19, 6).Style.NumberFormat.Format = "0.00";
                    ws.Cell(19, 6).Value = "0";
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
                    ws.Cell(19, 9).Value = "0";
                    ws.Cell(19, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

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

                    ws.Cell(21, 11).Style.Font.SetBold();
                    ws.Cell(21, 11).Style.Font.FontSize = 10;
                    ws.Cell(21, 11).Style.NumberFormat.Format = "0.00";
                    ws.Cell(21, 11).Value = "0";
                    ws.Cell(21, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                }


                //var CustomCells = ws.Cells().Style.Fill.SetBackgroundColor(XLColor.Gray);

                ws.Cell(14, 1).Style.Fill.BackgroundColor = XLColor.Gray;
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
                ws.Cell(25, 3).Style.Fill.BackgroundColor = XLColor.Gray;


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
