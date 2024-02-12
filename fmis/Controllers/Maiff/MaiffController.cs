using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;
using fmis.Models.Maiff;
using System;
using fmis.Data;
using System.Threading.Tasks;
using fmis.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System.IO;

namespace fmis.Controllers.Maiff
{
    public class MaiffController : Controller
    {

        private readonly MyDbContext _MyDbContext;


        public MaiffController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;

        }


        [Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
        public async Task <IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("Maiff", "Dv", "");
        
            return View(await _MyDbContext.MaiffDv.OrderBy(x=> x.Id).ToListAsync());
        }

        [Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMaiffDv(MaiffDv model, string selectedSaa1, string selectedSaa2, string selectedSaa3, string facilityAddress, double amount1, double amount2, double amount3, string deduction1, string deduction2, double deductionAmount1, double deductionAmount2)
        {
            ViewBag.filter = new FilterSidebar("Maiff", "Dv", "");

            if (ModelState.IsValid)
            {
                model.SaaNumber = "";
                string[] selectedSaas = { selectedSaa1, selectedSaa2, selectedSaa3 };
                model.SaaNumber = string.Join(",", selectedSaas.Where(s => !string.IsNullOrEmpty(s) && s != "SAA"));

                model.Address = facilityAddress;
                model.Amount1 = amount1;
                model.Amount2 = amount2;
                model.Amount3 = amount3;
                model.Deduction1 = deduction1;
                model.Deduction2 = deduction2;
                model.DeductionAmount1 = deductionAmount1;
                model.DeductionAmount2 = deductionAmount2;
                model.TotalDeductionAmount = model.DeductionAmount1 + model.DeductionAmount2;
                model.TotalAmount = model.Amount1 + model.Amount2 + model.Amount3;
                model.OverallTotalAmount = model.TotalAmount - model.TotalDeductionAmount;

                _MyDbContext.Add(model);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }



        public IActionResult PrintMaiffDv(string[] token, int id)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {

                string ExportData = "This is pdf generated";
                StringReader reader = new StringReader(ExportData);
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();


                doc.NewPage();



        var maiffDv = (from MaiffDv in _MyDbContext.MaiffDv
                           where MaiffDv.Id == id
                                  
                                   select new
                                   {
                                       Date = MaiffDv.Date,
                                       Payee = MaiffDv.Payee,
                                       Address = MaiffDv.Address,
                                       MonthYearFrom = MaiffDv.MonthYearFrom,
                                       MonthYearTo = MaiffDv.MonthYearTo,
                                       saaNumber = MaiffDv.SaaNumber,
                                       Amount1 = MaiffDv.Amount1,
                                       Amount2 = MaiffDv.Amount2,
                                       Amount3 = MaiffDv.Amount3,
                                       TotalAmount = MaiffDv.TotalAmount,
                                       Deduction1 = MaiffDv.Deduction1,
                                       Deduction2 = MaiffDv.Deduction2,
                                       DeductionAmount1 = MaiffDv.DeductionAmount1,
                                       DeductionAmount2 = MaiffDv.DeductionAmount2,
                                       TotalDeductionAmount = MaiffDv.TotalDeductionAmount,
                                       OverallTotalAmount = MaiffDv.OverallTotalAmount
                                   
                                   }).ToList();


                var fundCluster = (from dv in _MyDbContext.Dv
                                   join fc in _MyDbContext.FundCluster
                                   on dv.FundClusterId equals fc.FundClusterId

                                   join r in _MyDbContext.RespoCenter
                                   on dv.RespoCenterId equals r.RespoId

                                   join a in _MyDbContext.Assignee
                                   on dv.AssigneeId equals a.AssigneeId
                                   where dv.DvId == id
                                   select new
                                   {
                                       fcDes = fc.FundClusterDescription,
                                       dvNo = dv.DvNo,
                                       dvDate = dv.Date,
                                       dvParticulars = dv.Particulars,
                                       dvPayee = dv.Payee.PayeeDescription,
                                       dvTinNo = dv.Payee.TinNo,
                                       dvGrossAmount = dv.GrossAmount,
                                       dvTotalDeductions = dv.TotalDeduction,
                                       dvNetAmount = dv.NetAmount,
                                       respo = r.RespoHead,
                                       respoHeadPosition = r.RespoHeadPosition,
                                       assigneeDvId = dv.AssigneeId,
                                       assigneeName = a.FullName,
                                       assigneeDesignation = a.Designation,
                                       dvDeduction = dv.dvDeductions
                                   }).ToList();


                Paragraph header_text = new Paragraph("DISBURSEMENT VOUCHER");

                header_text.Font = FontFactory.GetFont("Times New Roman", 10, Font.BOLD, BaseColor.BLACK);
                header_text.Alignment = Element.ALIGN_CENTER;

                Paragraph nextline = new Paragraph("\n");
                doc.Add(nextline);
                PdfPTable table = new PdfPTable(3);
                table.PaddingTop = 5f;
                table.WidthPercentage = 100f;
                float[] columnWidths = { 5, 25, 15 };
                table.SetWidths(columnWidths);

                Image logo = Image.GetInstance("wwwroot/assets/images/doh_logo_updated.png");
                logo.ScaleAbsolute(60f, 60f);
                PdfPCell logo_cell = new PdfPCell(logo);
                logo_cell.DisableBorderSide(8);

                logo_cell.Padding = 11f;
                table.AddCell(logo_cell);


                Font arial_font_8 = FontFactory.GetFont("", 8, Font.NORMAL, BaseColor.BLACK);
                Font arial_font_9 = FontFactory.GetFont("", 9, Font.NORMAL, BaseColor.BLACK);
                Font arial_font_9b = FontFactory.GetFont("", 9, Font.BOLD, BaseColor.BLACK);
                Font arial_font_10 = FontFactory.GetFont("Times New Roman", 10, Font.NORMAL, BaseColor.BLACK);
                Font arial_font_11 = FontFactory.GetFont("Times New Roman", 11, Font.NORMAL, BaseColor.BLACK);
                Font arial_font_12 = FontFactory.GetFont("Times New Roman", 12, Font.BOLD, BaseColor.BLACK);
                Font header = FontFactory.GetFont("Times New Roman", 10, Font.BOLD, BaseColor.BLACK);

                var table2 = new PdfPTable(1);
                table2.DefaultCell.Border = 0;

                FontFactory.Register("C:\\Windows\\Fonts\\timesbd.ttf", "Times New Roman Bold");

                var times_new_roman_b12 = FontFactory.GetFont("Times New Roman Bold", 12f);
                var times_new_roman_b11 = FontFactory.GetFont("Times New Roman Bold", 11f);
                var times_new_roman_b10 = FontFactory.GetFont("Times New Roman Bold", 10f);
                var times_new_roman_b9 = FontFactory.GetFont("Times New Roman Bold", 9f);
                var times_new_roman_b8 = FontFactory.GetFont("Times New Roman Bold", 8f);


                FontFactory.Register("C:\\Windows\\Fonts\\times.ttf", "Times New Roman Regular");
                var times_new_roman_r12 = FontFactory.GetFont("Times New Roman Regular", 12f);
                var times_new_roman_r11 = FontFactory.GetFont("Times New Roman Regular", 11f);
                var times_new_roman_r10 = FontFactory.GetFont("Times New Roman Regular", 10f);
                var times_new_roman_r9 = FontFactory.GetFont("Times New Roman Regular", 9f);
                var times_new_roman_r8 = FontFactory.GetFont("Times New Roman Regular", 8f);
                var times_new_roman_r7 = FontFactory.GetFont("Times New Roman Regular", 7f);


                // table2.AddCell(new PdfPCell(new Paragraph(" __________________________________", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Republic of Philippines", times_new_roman_r9)) { Padding = 2f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Department of Health", times_new_roman_r9)) { Padding = 2f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("CENTRAL VISAYAS CENTER FOR HEALTH DEVELOPMENT", times_new_roman_b9)) { Padding = 2f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Osmeña Boulevard, Sambag II, Cebu City, 6000 Philippines", times_new_roman_r8)) { Padding = 2f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Regional Director's Office Tel. No. (032) 253-6335 Fax No. (032) 254-0109", times_new_roman_r8)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Phrase("DISBURSEMENT VOUCHER", times_new_roman_b12)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });

                //table2.AddCell(new PdfPCell(new Paragraph("Entity Name", arial_font_11)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });


                var no_left_bor = new PdfPCell(table2);
                no_left_bor.DisableBorderSide(4);
                table.AddCell(no_left_bor);

                var table3 = new PdfPTable(2);
                float[] table3widths = { 6, 10 };
                table3.SetWidths(table3widths);
                table3.DefaultCell.Border = 0;

                table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster : ", arial_font_9))
                {
                    Border = 2,
                    PaddingTop = 7,
                    FixedHeight = 30,
                });

                table3.AddCell(new PdfPCell(new Paragraph(fundCluster?.FirstOrDefault()?.fcDes.ToString(), arial_font_9))
                {
                    Border = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingRight = 5,
                    PaddingTop = 5,
                });
                table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_9))
                {
                    Padding = 6f,
                    Border = 0
                });
                table3.AddCell(new PdfPCell(new Paragraph(maiffDv?.FirstOrDefault()?.Date.ToShortDateString(), arial_font_9))
                {
                    Border = 0,
                    Padding = 6f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingRight = 5
                });
                table3.AddCell(new PdfPCell(new Paragraph("DV No :", arial_font_9))
                {
                    Padding = 6f,
                    Border = 0
                }
                );
                table3.AddCell(new PdfPCell(new Paragraph(fundCluster?.FirstOrDefault()?.dvNo, arial_font_9))
                {
                    Border = 0,
                    Padding = 6f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingRight = 5
                });

                table.AddCell(table3);
                doc.Add(table);

                //MODE OF PAYMENT
                iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance("wwwroot/assets/images/final_textbox_f.png");
                PdfPCell cell = new PdfPCell(myImage);
                var table_row_2 = new PdfPTable(9);
                float[] tbt_row2_width = { 7, 3, 10, 3, 10, 3, 9, 3, 29 };
                table_row_2.WidthPercentage = 100f;

                table_row_2.SetWidths(tbt_row2_width);
                table_row_2.AddCell(new PdfPCell(new Paragraph("Mode Of Payment", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table_row_2.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 5,
                    PaddingLeft = 10,

                });
                table_row_2.AddCell(new PdfPCell(new Paragraph("MDS Check", arial_font_9))
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 15,

                });
                table_row_2.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 5,
                    PaddingLeft = 10,
                });
                table_row_2.AddCell(new PdfPCell(new Paragraph("Commercial Check", arial_font_9))
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 15,
                });
                table_row_2.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 5,
                    PaddingLeft = 10,
                });
                table_row_2.AddCell(new PdfPCell(new Paragraph("ADA", arial_font_9))
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 15,
                });
                table_row_2.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 5,
                    PaddingLeft = 10,
                });
                table_row_2.AddCell(new PdfPCell(new Paragraph("Others (Please specify) ____________ ", arial_font_9))
                {
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 15,
                });

                doc.Add(table_row_2);


                //PAYEE
                var table_row_3 = new PdfPTable(4);
                float[] tbt_row3_width = { 7, 30, 20, 20 };
                table_row_3.WidthPercentage = 100f;
                table_row_3.SetWidths(tbt_row3_width);
                table_row_3.AddCell(new PdfPCell(new Paragraph("Payee", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table_row_3.AddCell(new PdfPCell(new Paragraph(maiffDv?.FirstOrDefault()?.Payee.ToString(), arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 10,
                });
                table_row_3.AddCell(new PdfPCell(new Paragraph("Tin/Employee No.:" + fundCluster?.FirstOrDefault()?.dvTinNo.ToString(), arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table_row_3.AddCell(new PdfPCell(new Paragraph("ORS/BURS No.: ", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                doc.Add(table_row_3);

                //ADDRESS
                var table_row_4 = new PdfPTable(2);
                float[] tbt_row4_width = { 3, 30 };
                table_row_4.WidthPercentage = 100f;
                table_row_4.SetWidths(tbt_row4_width);
                table_row_4.AddCell(new PdfPCell(new Paragraph("Address", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table_row_4.AddCell(new PdfPCell(new Paragraph(maiffDv?.FirstOrDefault()?.Address.ToString(), arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
                table_row_4.AddCell(new PdfPCell(new Paragraph()));
                doc.Add(table_row_4);


                var table_row_5 = new PdfPTable(4);
                float[] tbt_row5_width = { 20, 5, 5, 5 };
                table_row_5.WidthPercentage = 100f;
                table_row_5.SetWidths(tbt_row5_width);

                table_row_5.AddCell(new PdfPCell(new Paragraph("Particulars", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 25
                });
                table_row_5.AddCell(new PdfPCell(new Paragraph("Responsibility Center", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 25
                });
                table_row_5.AddCell(new PdfPCell(new Paragraph("MFO/PAP", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 25
                });
                table_row_5.AddCell(new PdfPCell(new Paragraph("Amount", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 25
                });

                var item = _MyDbContext.Dv.Include(x => x.dvDeductions).ThenInclude(x => x.Deduction).ToList();
                List<decimal?> deductionsAmount = new List<decimal?>();
                List<string> deductionsList = new List<string>();

                Font arial_font_deductions = FontFactory.GetFont("", 8, Font.NORMAL, BaseColor.BLACK);
                decimal sum = 0;
                foreach (var dvDeductions in item.Where(x => x.DvId == id))
                {
                    var deduct = fundCluster?.FirstOrDefault()?.dvNetAmount - (dvDeductions?.dvDeductions?.FirstOrDefault()?.Amount ?? 0);
                    foreach (var deductions in dvDeductions.dvDeductions.Where(x => x.Amount != null))
                    {
                        deductionsAmount.Add(deductions.Amount);
                        string description = deductions?.Deduction?.DeductionDescription.PadRight(15);
                        string netDeduct = deduct?.ToString("##,#00.00").PadRight(12);
                        string amount = deductions?.Amount?.ToString("##,#00.00").PadLeft(15);

                        deductionsList.Add(description + " { " + netDeduct + " } " + amount);
                        //Console.WriteLine(string.Join("\n", deductionsList));
                    }
                }


                doc.Add(table_row_5);
                var table_row_6 = new PdfPTable(4);
                float[] tbt_ro6_width = { 20, 5, 5, 5 };
                table_row_6.WidthPercentage = 100f;
                table_row_6.SetWidths(tbt_ro6_width);

                // Assuming maiffDv?.FirstOrDefault().MonthYearFrom is a DateTime or string representing a date
                DateTime? dateFrom = maiffDv?.FirstOrDefault().MonthYearFrom as DateTime?;
                DateTime? dateTo = maiffDv?.FirstOrDefault().MonthYearTo as DateTime?;

                // Format the dates as "MMMM yyyy" (e.g., "November 2023")
                string formattedDateFrom = dateFrom?.ToString("MMMM yyyy") ?? "";
                string formattedDateTo = dateTo?.ToString("MMMM yyyy") ?? "";

                // Assuming you have Amount1, Amount2, and Amount3 properties in your maiffDv class
                string saaNumbers = string.Join(",", (maiffDv?.FirstOrDefault()?.saaNumber?.Split(',') ?? Array.Empty<string>())
                     .Select(s => new string(' ', 8) + s));

                string formattedAmounts = string.Join("\n", new[]
                {
                    $"Amount1: {maiffDv?.FirstOrDefault()?.Amount1}",
                    $"Amount2: {maiffDv?.FirstOrDefault()?.Amount2}",
                    $"Amount3: {maiffDv?.FirstOrDefault()?.Amount3}"
                }.Select(a => new string(' ', 8) + a));

                if (!string.IsNullOrEmpty(saaNumbers))
                {
                    // Join the array elements with newline characters
                    string formattedSaaNumbers = string.Join("\n", saaNumbers.Split(','));

                    // Update the code with formatted SAA numbers and Amounts
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "For reimbursement of medical services rendered to patients under the Medical Assistance for" +
                        " " + maiffDv?.FirstOrDefault()?.Payee.ToString() + " per billing statement dated " + formattedDateFrom + " - " + formattedDateTo + "\n" + "in the amount of:" + "\n\n"
                        + formattedSaaNumbers + "\n\n" + formattedAmounts, arial_font_9))
                    {
                        Border = 13,
                        FixedHeight = 110f,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_TOP,
                        PaddingLeft = 10
                    });
                }



                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "in the amount of:", arial_font_9)) { Border = 13, FixedHeight = 110f, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 110f, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_6.AddCell(new PdfPCell(new Paragraph("" +
                    "" + "\n" + "PHP " + fundCluster?.FirstOrDefault()?.dvGrossAmount?.ToString("##,#00.00") + "\n\n\n\n\n\n" +
                    "PHP " + fundCluster?.FirstOrDefault()?.dvTotalDeductions?.ToString("##,#00.00") + "\n\n\n\n\n\n" +
                    "PHP " + fundCluster?.FirstOrDefault()?.dvNetAmount?.ToString("##,#00.00"), arial_font_9))
                {
                    Border = 13,
                    FixedHeight = 140f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_TOP,
                    PaddingBottom = 7,

                });
                doc.Add(table_row_6);

                var table_row_7 = new PdfPTable(4);
                float[] tbt_row7_width = { 10, 5, 5, 5 };
                table_row_7.WidthPercentage = 100f;
                table_row_7.SetWidths(tbt_row7_width);
                table_row_7.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 14,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                PdfPTable tbt_total_amt = new PdfPTable(1);
                float[] tbt_total_amt_width = { 10 };
                tbt_total_amt.WidthPercentage = 100f;
                tbt_total_amt.SetWidths(tbt_total_amt_width);
                tbt_total_amt.AddCell(new PdfPCell(new Paragraph("\n", arial_font_9))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
                tbt_total_amt.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });
                table_row_7.AddCell(new PdfPCell(tbt_total_amt) { Border = 14 });

                doc.Add(table_row_7);

                var table_row_8 = new PdfPTable(1);
                var p = new Paragraph();
                float[] tbt_ro8_width = { 10 };
                table_row_8.DefaultCell.FixedHeight = 200f;
                table_row_8.WidthPercentage = 100f;
                table_row_8.SetWidths(tbt_ro8_width);
                if (fundCluster?.FirstOrDefault()?.respo == "RAMIL R. ABREA, CPA, MBA")
                {
                    table_row_8.AddCell(new PdfPCell(new Paragraph("A. Certified: Expenses/Cash Advance necessary, lawful and incurred under my direct supervision.\n\n\n\n                                                                             " + fundCluster?.FirstOrDefault()?.respo, arial_font_9b))
                    {
                        Border = 13,
                        FixedHeight = 50f,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                    });
                }
                else if (fundCluster?.FirstOrDefault()?.respo == "JONATHAN NEIL V. ERASMO, MD, MPH, FPSMS")
                {
                    table_row_8.AddCell(new PdfPCell(new Paragraph("A. Certified: Expenses/Cash Advance necessary, lawful and incurred under my direct supervision.\n\n\n\n                                                           " + fundCluster?.FirstOrDefault()?.respo, arial_font_9b))
                    {
                        Border = 13,
                        FixedHeight = 50f,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                    });
                }
                else
                {
                    table_row_8.AddCell(new PdfPCell(new Paragraph("A. Certified: Expenses/Cash Advance necessary, lawful and incurred under my direct supervision.\n\n\n\n                                                             " + fundCluster?.FirstOrDefault()?.respo, arial_font_9b))
                    {
                        Border = 13,
                        FixedHeight = 50f,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                    });
                }
                table_row_8.AddCell(new PdfPCell(new Paragraph("                                                     Printed Name, Designation and Signature of Supervisor", arial_font_9b))
                {
                    FixedHeight = 20f,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,

                });
                doc.Add(table_row_8);

                var table_row_9 = new PdfPTable(1);
                float[] tbt_ro9_width = { 10 };
                table_row_9.WidthPercentage = 100f;
                table_row_9.SetWidths(tbt_ro9_width);
                table_row_9.AddCell(new PdfPCell(new Paragraph("B. Accounting Entry: ", arial_font_9b))
                {
                    Border = 13,
                    FixedHeight = 15f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                doc.Add(table_row_9);

                var table_row_10 = new PdfPTable(4);
                float[] tbt_row10_width = { 30, 10, 10, 10 };
                table_row_10.WidthPercentage = 100f;
                table_row_10.SetWidths(tbt_row10_width);
                table_row_10.AddCell(new PdfPCell(new Paragraph("Account Title", arial_font_9)) { Border = 13, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_10.AddCell(new PdfPCell(new Paragraph("Uacs Code", arial_font_9)) { Border = 13, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_10.AddCell(new PdfPCell(new Paragraph("Debit", arial_font_9)) { Border = 13, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_10.AddCell(new PdfPCell(new Paragraph("Credit", arial_font_9)) { Border = 13, HorizontalAlignment = Element.ALIGN_CENTER });

                doc.Add(table_row_10);


                var table_row_11 = new PdfPTable(4);
                float[] tbt_row11_width = { 30, 10, 10, 10 };
                table_row_11.WidthPercentage = 100f;
                table_row_11.SetWidths(tbt_row11_width);
                table_row_11.AddCell(new PdfPCell(new Paragraph("", arial_font_9)) { Border = 13, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 60f });
                table_row_11.AddCell(new PdfPCell(new Paragraph("", arial_font_9)) { Border = 13, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 60f });
                table_row_11.AddCell(new PdfPCell(new Paragraph("", arial_font_9)) { Border = 13, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 60f });
                table_row_11.AddCell(new PdfPCell(new Paragraph("", arial_font_9)) { Border = 13, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 60f });
                doc.Add(table_row_11);

                var table_row_13 = new PdfPTable(2);
                float[] tbt_row_13_width = { 10, 10 };
                table_row_13.WidthPercentage = 100f;
                table_row_13.SetWidths(tbt_row_13_width);
                table_row_13.AddCell(new PdfPCell(new Paragraph("C. Certified:", arial_font_9b))
                {
                    FixedHeight = 15f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                table_row_13.AddCell(new PdfPCell(new Paragraph("D. Approved for Payment:", arial_font_9b))
                {
                    Border = 13,
                    FixedHeight = 15f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                doc.Add(table_row_13);


                var table_row_15 = new PdfPTable(3);
                float[] tbt_row15_width = { 5, 20, 25 };
                table_row_15.WidthPercentage = 100f;
                table_row_15.SetWidths(tbt_row15_width);
                table_row_15.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Padding = 5,
                    PaddingLeft = 25,
                    Border = PdfPCell.LEFT_BORDER
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("Cash available ", arial_font_9))
                {
                    PaddingTop = 5,
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_row_15.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Padding = 5,
                    PaddingLeft = 25,
                    Border = PdfPCell.LEFT_BORDER
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("Subject to Authority to Debit Account (when applicable) ", arial_font_9))
                {
                    PaddingTop = 2,
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_row_15.AddCell(new PdfPCell(new PdfPCell(myImage))
                {
                    Padding = 5,
                    PaddingLeft = 25,
                    Border = PdfPCell.LEFT_BORDER
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("Supporting documents complete and amount claimed proper", arial_font_9))
                {
                    PaddingTop = 1,
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                table_row_15.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = PdfPCell.RIGHT_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                doc.Add(table_row_15);

                var table_row_16 = new PdfPTable(4);
                float[] tbt_row_16_width = { 5, 20, 5, 20 };
                table_row_16.WidthPercentage = 100f;
                table_row_16.SetWidths(tbt_row_16_width);
                table_row_16.AddCell(new PdfPCell(new Paragraph("Signature", arial_font_9))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                table_row_16.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 20f
                });
                table_row_16.AddCell(new PdfPCell(new Paragraph("Signature", arial_font_9))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                table_row_16.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 20f
                });
                doc.Add(table_row_16);

                var table_row_17 = new PdfPTable(4);
                float[] tbt_row_17_width = { 5, 20, 5, 20 };
                table_row_17.WidthPercentage = 100f;
                table_row_17.SetWidths(tbt_row_17_width);
                table_row_17.AddCell(new PdfPCell(new Paragraph("Printed Name", arial_font_8))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                table_row_17.AddCell(new PdfPCell(new Paragraph(fundCluster?.FirstOrDefault()?.assigneeName, arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 15f
                });
                table_row_17.AddCell(new PdfPCell(new Paragraph("Printed Name", arial_font_8))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                if (fundCluster?.FirstOrDefault()?.dvGrossAmount <= 1000000)
                {
                    //table_row_17.AddCell(new PdfPCell(new Paragraph(fundCluster?.FirstOrDefault()?.respo, arial_font_8))
                    table_row_17.AddCell(new PdfPCell(new Paragraph("SOPHIA M. MANCAO, MD, DPSP, RN-MAN", arial_font_8))
                    {
                        Border = 13,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        FixedHeight = 20f
                    });
                }
                else
                {
                    table_row_17.AddCell(new PdfPCell(new Paragraph("JAIME S. BERNADAS, MD., MGM, C.ES.O III", arial_font_8))
                    {
                        Border = 13,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        FixedHeight = 20f
                    });
                }
                doc.Add(table_row_17);


                var table_row_18 = new PdfPTable(4);
                float[] tbt_row_18_width = { 5, 20, 5, 20 };
                table_row_18.WidthPercentage = 100f;
                table_row_18.SetWidths(tbt_row_18_width);
                table_row_18.AddCell(new PdfPCell(new Paragraph("Position", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 25f
                });
                table_row_18.AddCell(new PdfPCell(new Paragraph(fundCluster?.FirstOrDefault()?.assigneeDesignation + "\n" + "Accounting Unit/ Authorized Representative", arial_font_8))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 28f,
                });

                table_row_18.AddCell(new PdfPCell(new Paragraph("Position", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 25f,

                });
                if (fundCluster?.FirstOrDefault()?.dvGrossAmount <= 1000000)
                {
                    table_row_18.AddCell(new PdfPCell(new Paragraph("DIRECTOR III" + "\n" + "Agency Head/Authorized Representative", arial_font_8))
                    {
                        Border = 13,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        FixedHeight = 28f
                    });
                }
                else
                {
                    table_row_18.AddCell(new PdfPCell(new Paragraph("DIRECTOR IV" + "\n" + "Agency Head/Authorized Representative", arial_font_8))
                    {
                        Border = 13,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        FixedHeight = 28f
                    });
                }
                doc.Add(table_row_18);


                var table_row_19 = new PdfPTable(4);
                float[] tbt_row_19_width = { 5, 20, 5, 20 };
                table_row_19.WidthPercentage = 100f;
                table_row_19.SetWidths(tbt_row_19_width);
                table_row_19.AddCell(new PdfPCell(new Paragraph("Date", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                table_row_19.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 20f
                });
                table_row_19.AddCell(new PdfPCell(new Paragraph("Date", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 20f
                });
                table_row_19.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 20f
                });
                doc.Add(table_row_19);

                var table_row_20 = new PdfPTable(1);
                float[] tbt_row_20_width = { 1 };
                table_row_20.WidthPercentage = 100f;
                table_row_20.SetWidths(tbt_row_20_width);
                table_row_20.AddCell(new PdfPCell(new Paragraph("E. Receipt of Payment", arial_font_9b))
                {
                    Border = 13,
                    FixedHeight = 15f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                doc.Add(table_row_20);

                var table_row_21 = new PdfPTable(5);
                float[] tbt_row_21_width = { 3, 8, 5, 8, 5 };
                table_row_21.WidthPercentage = 100f;
                table_row_21.SetWidths(tbt_row_21_width);
                table_row_21.AddCell(new PdfPCell(new Paragraph("Check/ ADA No.: ", arial_font_8))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 25f
                });
                table_row_21.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25f
                });
                table_row_21.AddCell(new PdfPCell(new Paragraph(" Date: ", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25f
                });
                table_row_21.AddCell(new PdfPCell(new Paragraph("Bank Name & Account Number: ", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25f
                });
                table_row_21.AddCell(new PdfPCell(new Paragraph("JEV No.", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 25f
                });
                doc.Add(table_row_21);

                var table_row_22 = new PdfPTable(5);
                float[] tbt_row_22_width = { 3, 8, 5, 8, 5 };
                table_row_22.WidthPercentage = 100f;
                table_row_22.SetWidths(tbt_row_22_width);
                table_row_22.AddCell(new PdfPCell(new Paragraph("Signature: ", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 23f
                });
                table_row_22.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 23f
                });
                table_row_22.AddCell(new PdfPCell(new Paragraph(" Date: ", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 23f
                });
                table_row_22.AddCell(new PdfPCell(new Paragraph("Printed Name: ", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 23f
                });
                table_row_22.AddCell(new PdfPCell(new Paragraph("Date", arial_font_9))
                {
                    Border = 13,
                    VerticalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 23f
                });
                doc.Add(table_row_22);


                var table_row_23 = new PdfPTable(1);
                float[] tbt_row_23_width = { 1 };
                table_row_23.WidthPercentage = 100f;
                table_row_23.SetWidths(tbt_row_23_width);
                table_row_23.AddCell(new PdfPCell(new Paragraph("Official Receipt No. & Date/Other Documents", arial_font_9))
                {
                    Border = 13,
                    FixedHeight = 15f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                doc.Add(table_row_23);

                var table_end = new PdfPTable(1);
                float[] tbt_end_width = { 10 };
                table_end.WidthPercentage = 100f;
                table_end.SetWidths(tbt_end_width);
                table_end.AddCell(new PdfPCell(new Paragraph("" + "", arial_font_9))
                {
                    Border = 13,
                    FixedHeight = 1f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                doc.Add(table_end);

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, reader);
                doc.Close(); return File(stream.ToArray(), "application/pdf");

            }

        }

    }
}
