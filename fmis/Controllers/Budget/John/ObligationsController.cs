using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Rotativa.AspNetCore;
using System.IO;
using fmis.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using System.Globalization;
using System.Collections;
using iTextSharp.tool.xml;
using Image = iTextSharp.text.Image;
using Grpc.Core;
using fmis.ViewModel;
using fmis.DataHealpers;

namespace fmis.Controllers
{
    public class ObligationsController : Controller
    {
        private readonly ObligationContext _context;
        private readonly ObligationAmountContext _Ucontext;
        private readonly UacsContext _UacsContext;
        private readonly MyDbContext _MyDbContext;

        ORSReporting rpt_ors = new ORSReporting();
        private Obligation obligation;

        public ObligationsController(ObligationContext context, ObligationAmountContext Ucontext, UacsContext UacsContext, MyDbContext MyDbContext)
        {
            _context = context;
            _Ucontext = Ucontext;
            _UacsContext = UacsContext;
            _MyDbContext = MyDbContext;
        }

        public IActionResult PrintPdf(ManyId many)
        {

            return new ViewAsPdf("PrintPdf")
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public DateTime CheckExcelDate(string excel_data)
        {
            string dateString = @"d/M/yyyy";

            DateTime date1 = DateTime.ParseExact(dateString, @"d/M/yyyy",
            System.Globalization.CultureInfo.InvariantCulture);
            if (dateString == null)
                return DateTime.ParseExact(dateString, @"d/M/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            return (DateTime)date1;
        }

        public class ObligationData
        {
            public int Id { get; set; }
            public int source_id { get; set; }
            public string source_title { get; set; }
            public string source_type { get; set; }
            public decimal source_balance { get; set; }
            public string Date { get; set; }
            public string Dv { get; set; }
            public string Pr_no { get; set; }
            public string Po_no { get; set; }
            public string Payee { get; set; }
            public string Address { get; set; }
            public string Particulars { get; set; }
            public int Ors_no { get; set; }
            public float Gross { get; set; }
            public int Created_by { get; set; }
            public string obligation_token { get; set; }
            public string status { get; set; }
        }

        public class ManyId
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<ManyId> many_token { get; set; }
        }

        // GET: Obligations
        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("ors", "obligation", "");

            var obligation = await _context
                                    .Obligation
                                    .Where(x => x.status == "activated")
                                    .Include(x => x.ObligationAmounts)
                                    .AsNoTracking()
                                    .ToListAsync();

            //return Json(obligation_json);
            ViewBag.obligation_json = JsonSerializer.Serialize(obligation);

            var fund_sub_data = (from x in _MyDbContext.FundSources select new { source_id = x.FundSourceId, source_title = x.FundSourceTitle, remaining_balance = x.Remaining_balance, source_type = "fund_source", obligated_amount = x.obligated_amount })
                                    .Concat(from y in _MyDbContext.Sub_allotment select new { source_id = y.SubAllotmentId, source_title = y.Suballotment_title, remaining_balance = y.Remaining_balance, source_type = "sub_allotment", obligated_amount = y.obligated_amount });
            ViewBag.fund_sub = JsonSerializer.Serialize(fund_sub_data);

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            return View("~/Views/Budget/John/Obligations/Index.cshtml", obligation);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> openObligationAmount(int id, string obligation_token)
        {
            var uacs_data = JsonSerializer.Serialize(await _UacsContext.Uacs.AsNoTracking().ToListAsync());
            ViewBag.uacs = uacs_data;

            if (id != 0)
            {
                obligation = await _context.Obligation
                    .Include(x => x.ObligationAmounts.Where(x => x.status == "activated"))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
                obligation.Uacs = await _UacsContext.Uacs.AsNoTracking().ToListAsync();
            }
            else
            {
                obligation = await _context.Obligation
                    .Include(x => x.ObligationAmounts.Where(x => x.status == "activated"))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.obligation_token == obligation_token);
                obligation.Uacs = await _UacsContext.Uacs.AsNoTracking().ToListAsync();
            }

            if (obligation.source_type == "fund_source")
                obligation.FundSource = await _MyDbContext.FundSources.Where(x => x.FundSourceId == obligation.source_id).ToListAsync();
            else if (obligation.source_type == "sub_allotment")
                obligation.SubAllotment = await _MyDbContext.Sub_allotment.Where(x => x.SubAllotmentId == obligation.source_id).ToListAsync();

            /*return Json(obligation);*/
            return View("~/Views/Budget/John/Obligations/ObligationAmount.cshtml", obligation);
        }

        // GET: Obligations/Create
        public IActionResult Create()
        {

            return View();
        }

        private DateTime ToDateTime(string date)
        {
            if (DateTime.TryParse(date, out DateTime result))
            {
                return result;
            }

            return DateTime.MinValue;
        }

        public ActionResult AddData(List<string[]> dataListFromTable)
        {
            var dataListTable = dataListFromTable;
            return Json("Response, Data Received Successfully");
        }

        [HttpPost]
        public async Task<IActionResult> SaveObligation(List<ObligationData> data)
        {

            var data_holder = _context.Obligation;
            foreach (var item in data)
            {

                var obligation = new Obligation(); //CLEAR OBJECT

                if (await data_holder.Where(s => s.obligation_token == item.obligation_token).FirstOrDefaultAsync() != null) //CHECK IF EXIST
                {
                    obligation = await data_holder.Where(s => s.obligation_token == item.obligation_token).FirstOrDefaultAsync();
                }

                obligation.source_id = item.source_id;
                obligation.source_type = item.source_type;
                obligation.Date = ToDateTime(item.Date);
                obligation.Dv = item.Dv;
                obligation.Pr_no = item.Pr_no;
                obligation.Po_no = item.Po_no;
                obligation.Payee = item.Payee;
                obligation.Address = item.Address;
                obligation.Particulars = item.Particulars;
                obligation.Ors_no = item.Ors_no;
                obligation.Created_by = item.Created_by;
                obligation.Gross = item.Gross;
                obligation.status = "activated";
                obligation.obligation_token = item.obligation_token;
                _context.Update(obligation);
                await _context.SaveChangesAsync();

            }
            return Json(data);

        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,Fund_source,Gross,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obligation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obligation);
        }

        [HttpPost]

        public ActionResult AddObligation(IEnumerable<Obligation> ObligationsInput)

        {
            var p = ObligationsInput;
            return null;
        }

        // GET: Obligations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation.FindAsync(id);
            if (obligation == null)
            {
                return NotFound();
            }
            return View(obligation);
        }

        // POST: Obligations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Obligation obligation)
        {
            var obli = await _context.Obligation.Where(x => x.Id == obligation.Id).AsNoTracking().FirstOrDefaultAsync();
            obli.source_id = obligation.source_id;
            obli.source_type = obligation.source_type;
            obli.Date = obligation.Date;
            obli.Dv = obligation.Dv;
            obli.Pr_no = obligation.Pr_no;
            obli.Po_no = obligation.Po_no;
            obli.Payee = obligation.Payee;
            obli.Address = obligation.Address;
            obli.Particulars = obligation.Particulars;
            obli.Ors_no = obligation.Ors_no;
            obli.Gross = obligation.Gross;
            obli.remaining_balance = obligation.remaining_balance;
            obli.Created_by = obligation.Created_by;

            _context.Update(obli);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        // GET: Obligations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obligation = await _context.Obligation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obligation == null)
            {
                return NotFound();
            }

            return View(obligation);
        }

        // POST: Obligations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteObligation(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    setUpDeleteData(many.many_token);
            }
            else
                setUpDeleteData(data.single_token);

            return Json(data);
        }

        public void setUpDeleteData(string obligation_token)
        {
            var obligation = new Obligation(); //CLEAR OBJECT
            obligation = _context.Obligation.Where(s => s.obligation_token == obligation_token).FirstOrDefault();
            obligation.status = "deactivated";

            _context.Update(obligation);
            _context.SaveChanges();
        }

        private bool ObligationExists(int id)
        {
            return _context.Obligation.Any(e => e.Id == id);
        }

        //EXPORTING PDF FILE

        public async Task<IActionResult> PrintOrs(string[] token)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                string ExportData = "This is pdf generated";
                StringReader reader = new StringReader(ExportData);
                Document doc = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);

                doc.Open();

                foreach (var i in token)
                {
                    string tok = Convert.ToString(i);
                    var ors = await _context.Obligation
                        .Include(f => f.FundSource)
                        .ThenInclude(p => p.Prexc)
                        .FirstOrDefaultAsync(m => m.obligation_token == tok);


                    doc.NewPage();
                    var budget_allotments = _MyDbContext.Budget_allotments.Include(f => f.FundSources).FirstOrDefault();

                    Paragraph header_text = new Paragraph("OBLIGATION REQUEST AND STATUS");

                    header_text.Font = FontFactory.GetFont("Times New Roman", 10, Font.BOLD, BaseColor.BLACK);
                    header_text.Alignment = Element.ALIGN_CENTER;
                    //doc.Add(header_text);

                    Paragraph nextline = new Paragraph("\n");
                    doc.Add(nextline);
                    PdfPTable table = new PdfPTable(3);
                    table.PaddingTop = 5f;
                    table.WidthPercentage = 100f;
                    float[] columnWidths = { 5, 25, 15 };
                    table.SetWidths(columnWidths);

                    Image logo = Image.GetInstance("wwwroot/assets/images/ro7.png");
                    logo.ScaleAbsolute(60f, 60f);
                    PdfPCell logo_cell = new PdfPCell(logo);
                    logo_cell.DisableBorderSide(8);

                    logo_cell.Padding = 5f;
                    table.AddCell(logo_cell);

                    Font arial_font_10 = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);
                    Font header = FontFactory.GetFont("Times New Roman", 10, Font.BOLD, BaseColor.BLACK);

                    var table2 = new PdfPTable(1);
                    table2.DefaultCell.Border = 0;
                    table2.AddCell(new PdfPCell(new Phrase("OBLIGATION REQUEST AND STATUS", header)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table2.AddCell(new PdfPCell(new Paragraph("Republic of Philippines", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table2.AddCell(new PdfPCell(new Paragraph("Department of Health", header)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table2.AddCell(new PdfPCell(new Paragraph("Central Visayas Center for Health Development", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });


                    var no_left_bor = new PdfPCell(table2);
                    no_left_bor.DisableBorderSide(4);
                    table.AddCell(no_left_bor);

                    var table3 = new PdfPTable(2);
                    float[] table3widths = { 4, 10 };
                    table3.SetWidths(table3widths);
                    table3.DefaultCell.Border = 0;

                    var allotments = (from fundsource in _MyDbContext.FundSources
                                       join obligation in _MyDbContext.Obligation
                                       on fundsource.FundSourceId equals obligation.source_id
                                       join allotmentclass in _MyDbContext.AllotmentClass
                                       on fundsource.AllotmentClassId equals allotmentclass.Id
                                       join fund in _MyDbContext.Fund
                                       on fundsource.FundId equals fund.FundId
                                      where obligation.obligation_token == tok
                                      select new
                                       {
                                           allotment = allotmentclass.Fund_Code,
                                           fundCurrent = fund.Fund_code_current,
                                           fundConap = fund.Fund_code_conap,
                                           fundsource = fundsource.AppropriationId,
                                           obligation = obligation.source_type
                                      }).ToList();

                    var allotmentsAA = (from sub_allotment in _MyDbContext.Sub_allotment
                                      join obligation in _MyDbContext.Obligation
                                      on sub_allotment.SubAllotmentId equals obligation.source_id
                                      join allotmentclass in _MyDbContext.AllotmentClass
                                      on sub_allotment.AllotmentClassId equals allotmentclass.Id
                                      join fund in _MyDbContext.Fund
                                      on sub_allotment.FundId equals fund.FundId
                                      where obligation.obligation_token == tok
                                      select new
                                      {
                                          allotment = allotmentclass.Fund_Code,
                                          fundCurrent = fund.Fund_code_current,
                                          fundConap = fund.Fund_code_conap,
                                          sub_allotment = sub_allotment.AppropriationId,
                                          obligation = obligation.source_type
                                      }).ToList();


                    if (allotments.FirstOrDefault().fundsource == 1 && allotments.FirstOrDefault().obligation == "fund_source")
                    {      

                    Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                    table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                    table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" +allotments.FirstOrDefault().fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                    table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                    table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                    table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                    table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" + allotments.FirstOrDefault().fundCurrent, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                    /*table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                    table3.AddCell(new PdfPCell(new Paragraph(budget_allotments.Allotment_series + "-01101101", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });*/

                    table.AddCell(table3);

                    doc.Add(table);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 1 && allotmentsAA.FirstOrDefault().obligation == "sub_allotment")
                    {

                        Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                        table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundCurrent, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        /*table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(budget_allotments.Allotment_series + "-01101101", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });*/

                        table.AddCell(table3);

                        doc.Add(table);

                    }


                    if (allotments.FirstOrDefault().fundsource == 2 && allotments.FirstOrDefault().obligation == "fund_source")
                    {



                        Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                        table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" + allotments.FirstOrDefault().fundConap + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" + allotments.FirstOrDefault().fundConap, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        /*table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(budget_allotments.Allotment_series + "-01101101", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });*/

                        table.AddCell(table3);

                        doc.Add(table);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 2 && allotmentsAA.FirstOrDefault().obligation == "sub_allotment")
                    {

                        Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                        table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundCurrent, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        /*table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(budget_allotments.Allotment_series + "-01101101", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });*/

                        table.AddCell(table3);

                        doc.Add(table);

                    }


                    var table_row_2 = new PdfPTable(2);
                    float[] tbt_row2_width = { 5, 25 };
                    table_row_2.WidthPercentage = 100f;
                    table_row_2.SetWidths(tbt_row2_width);
                    table_row_2.AddCell(new PdfPCell(new Paragraph("Payee", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_2.AddCell(new PdfPCell(new Paragraph(ors.Payee, arial_font_10)));
                    table_row_2.AddCell(new PdfPCell(new Paragraph("", arial_font_10)));


                    doc.Add(table_row_2);

                    var table_row_3 = new PdfPTable(2);
                    float[] tbt_row3_width = { 5, 25 };
                    table_row_3.WidthPercentage = 100f;
                    table_row_3.SetWidths(tbt_row3_width);
                    table_row_3.AddCell(new PdfPCell(new Paragraph("Office", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_3.AddCell(new PdfPCell(new Paragraph("", arial_font_10)));
                    table_row_3.AddCell(new PdfPCell(new Paragraph()));

                    doc.Add(table_row_3);

                    var table_row_4 = new PdfPTable(2);
                    float[] tbt_row4_width = { 5, 25 };
                    table_row_4.WidthPercentage = 100f;
                    table_row_4.SetWidths(tbt_row4_width);
                    table_row_4.AddCell(new PdfPCell(new Paragraph("Address", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_4.AddCell(new PdfPCell(new Paragraph("OSMEÑA BOULEVARD, CEBU CITY", arial_font_10)));
                    table_row_4.AddCell(new PdfPCell(new Paragraph()));


                    doc.Add(table_row_4);

                    Font table_row_5_font = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);

                    var table_row_5 = new PdfPTable(5);
                    float[] tbt_row5_width = { 5, 10, 5, 5, 5 };
                    table_row_5.WidthPercentage = 100f;
                    table_row_5.SetWidths(tbt_row5_width);
                    table_row_5.AddCell(new PdfPCell(new Paragraph("Responsibility Center", new Font(Font.FontFamily.HELVETICA, 8f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_5.AddCell(new PdfPCell(new Paragraph("Particulars", new Font(Font.FontFamily.HELVETICA, 8f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_5.AddCell(new PdfPCell(new Paragraph("MFO/PAP", new Font(Font.FontFamily.HELVETICA, 8f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_5.AddCell(new PdfPCell(new Paragraph("UACS Object Code", new Font(Font.FontFamily.HELVETICA, 8f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_5.AddCell(new PdfPCell(new Paragraph("Amount", new Font(Font.FontFamily.HELVETICA, 8f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });

                    doc.Add(table_row_5);

                    var table_row_6 = new PdfPTable(5);
                    float[] tbt_ro6_width = { 5, 10, 5, 5, 5 };
                    table_row_6.WidthPercentage = 100f;
                    table_row_6.SetWidths(tbt_ro6_width);

                    Double total_amt = 0.00;
                    String str_amt = "";
                    String uacs = "";
                    Double disbursements = 0.00;

                    var fundsources = (from fundsource in _MyDbContext.FundSources
                                       join obligation in _MyDbContext.Obligation
                                       on fundsource.FundSourceId equals obligation.source_id
                                       join prexc in _MyDbContext.Prexc
                                       on fundsource.PrexcId equals prexc.Id
                                       join respo in _MyDbContext.RespoCenter
                                       on fundsource.RespoId equals respo.RespoId
                                       where obligation.obligation_token == tok
                                       select new
                                       {
                                           pap = prexc.pap_code1,
                                           obligation_id = obligation.source_id,
                                           fundsource_id = fundsource.FundSourceId,
                                           fundsource_code = fundsource.FundSourceTitle,
                                           respo = respo.RespoCode,
                                           signatory = respo.RespoHead,
                                           position = respo.RespoHeadPosition,
                                           particulars = obligation.Particulars
                                       }).ToList();

                    var uacses = (from obligation in _MyDbContext.Obligation
                                  join obligation_amount in _MyDbContext.ObligationAmount
                                  on obligation.Id equals obligation_amount.ObligationId
                               
                                  where obligation.obligation_token == tok
                                  select new
                                  {
                                      expense_code = obligation_amount.Expense_code,
                                      amount = (double)Convert.ToDecimal(obligation_amount.Amount)
                                  }).ToList();

                    foreach (var u in uacses)
                    {
                        uacs += u.expense_code + "\n";
                        str_amt += u.amount.ToString("C", new CultureInfo("en-PH")) + "\n";
                        total_amt += u.amount;
                    }

                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + fundsources.FirstOrDefault().fundsource_code + "\n\n" + fundsources.FirstOrDefault().respo, FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + ors.Particulars, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_LEFT });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + fundsources.FirstOrDefault().pap, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + uacs, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingBottom = 15f });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + str_amt, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_RIGHT, PaddingBottom = 15f });
                    doc.Add(table_row_6);

                    var table_row_7 = new PdfPTable(5);
                    float[] tbt_row7_width = { 5, 10, 5, 5, 5 };

                    table_row_7.WidthPercentage = 100f;
                    table_row_7.SetWidths(tbt_row7_width);
                    table_row_7.AddCell(new PdfPCell(new Paragraph("", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });


                    //REMOVE BORDER
                    PdfPTable po_dv = new PdfPTable(2);
                    po_dv.WidthPercentage = 100f;

                    po_dv.AddCell(new PdfPCell(new Paragraph("PO No." + ors.Po_no, table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                    po_dv.AddCell(new PdfPCell(new Paragraph("PR No. " + ors.Pr_no, table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                    po_dv.AddCell(new PdfPCell(new Paragraph("DV No. " + ors.Dv, table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                    po_dv.AddCell(new PdfPCell(new Phrase("", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                    // END OF REMOVE BORDER
                    table_row_7.AddCell(new PdfPCell(po_dv) { Border = 14 });

                    table_row_7.AddCell(new PdfPCell(new Paragraph("", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_7.AddCell(new PdfPCell(new Paragraph("TOTAL", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });


                    PdfPTable tbt_total_amt = new PdfPTable(1);
                    float[] tbt_total_amt_width = { 10 };

                    tbt_total_amt.WidthPercentage = 100f;
                    tbt_total_amt.SetWidths(tbt_total_amt_width);

                    tbt_total_amt.AddCell(new PdfPCell(new Paragraph("\n", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    tbt_total_amt.AddCell(new PdfPCell(new Paragraph(total_amt.ToString("C", new CultureInfo("en-PH")), table_row_5_font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table_row_7.AddCell(new PdfPCell(tbt_total_amt) { Border = 14 });

                    doc.Add(table_row_7);


                    PdfPTable table_row_8 = new PdfPTable(4);
                    float[] w_table_row_8 = { 5, 20, 5, 20 };
                    table_row_8.WidthPercentage = 100f;
                    table_row_8.SetWidths(w_table_row_8);


                    table_row_8.AddCell(new PdfPCell(new Paragraph("A.", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Certified:", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("B.", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Certified:", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))));



                    table_row_8.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 50f, Border = 13 });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Charges to appropriation/ allotment necessary, lawful and under my direct supervision; and supporting documents valid, proper and legal \n", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { FixedHeight = 50f, Border = 13 });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 50f, Border = 13 });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Allotment available and obligated for the purpose/adjustment necessary as indicated above \n", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { FixedHeight = 50f, Border = 13 });


                    //SIGNATURE 1
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Signature :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));

                    table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))) { Border = 14 });

                    //SIGNATURE 2
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Signature :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { Border = 14 });

                    table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))) { Border = 14 });

                    table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));

                    //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                    table_row_8.AddCell(new PdfPCell(new Paragraph(fundsources.FirstOrDefault().signatory, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("LEONORA A. ANIEL", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                    table_row_8.AddCell(new PdfPCell(new Paragraph("Position :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                    //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                    table_row_8.AddCell(new PdfPCell(new Paragraph(fundsources.FirstOrDefault().position, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Position", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("BUDGET OFFICER III", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                    table_row_8.AddCell(new PdfPCell(new Paragraph("\n")) { FixedHeight = 30f });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("\n")) { FixedHeight = 30f });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("\n")) { FixedHeight = 30f });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("\n")) { FixedHeight = 30f });


                    //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                    table_row_8.AddCell(new PdfPCell(new Paragraph("")));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Head Requesting Office / Authorized Representative", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))));
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Head, Budget Unit / Authorized Representative", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_CENTER });

                    table_row_8.AddCell(new PdfPCell(new Paragraph("Date :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_8.AddCell(new PdfPCell(new Paragraph("Date", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table_row_8.AddCell(new PdfPCell(new Paragraph(ors.Date.ToString("MM/dd/yyyy"), new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                    doc.Add(table_row_8);

                    PdfPTable table_row_9 = new PdfPTable(2);
                    table_row_9.WidthPercentage = 100f;
                    table_row_9.SetWidths(new float[] { 10, 90 });
                    table_row_9.AddCell(new PdfPCell(new Paragraph("C.", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))));
                    table_row_9.AddCell(new PdfPCell(new Paragraph("STATUS OF OBLIGATION", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                    doc.Add(table_row_9);

                    PdfPTable table_row_10 = new PdfPTable(2);
                    table_row_10.WidthPercentage = 100f;
                    table_row_10.SetWidths(new float[] { 50, 50 });
                    table_row_10.AddCell(new PdfPCell(new Paragraph("Reference", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_10.AddCell(new PdfPCell(new Paragraph("Amount", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                    doc.Add(table_row_10);

                    PdfPTable table_row_11 = new PdfPTable(7);
                    table_row_11.WidthPercentage = 100f;
                    table_row_11.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 30 });

                    table_row_11.AddCell(new PdfPCell(new Paragraph("Date", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_11.AddCell(new PdfPCell(new Paragraph("Particulars", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table_row_11.AddCell(new PdfPCell(new Paragraph("ORS/JEV/RCI/RADAI No.", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });

                    //dri dapita ang adjust

                    PdfPTable obli = new PdfPTable(1);
                    float[] obliga_width = { 10 };
                    obli.WidthPercentage = 100f;
                    obli.SetWidths(obliga_width);
                    obli.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                    obli.AddCell(new PdfPCell(new Paragraph("(a)", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_11.AddCell(new PdfPCell(obli) { Border = 14 });

                    PdfPTable paya = new PdfPTable(1);
                    float[] paya_width = { 10 };
                    paya.WidthPercentage = 100f;
                    paya.SetWidths(obliga_width);
                    paya.AddCell(new PdfPCell(new Paragraph("Payable", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                    paya.AddCell(new PdfPCell(new Paragraph("(b)", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_11.AddCell(new PdfPCell(paya) { Border = 14 });

                    PdfPTable paye = new PdfPTable(1);
                    float[] paym_width = { 10 };
                    paye.WidthPercentage = 100f;
                    paye.SetWidths(obliga_width);
                    paye.AddCell(new PdfPCell(new Paragraph("Payment", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                    paye.AddCell(new PdfPCell(new Paragraph("(c)", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_11.AddCell(new PdfPCell(paye) { Border = 14 });

                    PdfPTable blnc = new PdfPTable(1);
                    float[] blnc_width = { 10 };
                    blnc.WidthPercentage = 100f;
                    blnc.SetWidths(blnc_width);
                    blnc.AddCell(new PdfPCell(new Paragraph("Balance", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 25, VerticalAlignment = Element.ALIGN_MIDDLE });

                    PdfPTable due = new PdfPTable(2);
                    float[] due_width = { 10, 10 };
                    due.WidthPercentage = 100f;
                    due.SetWidths(due_width);
                    due.AddCell(new PdfPCell(new Paragraph("Not Yet Due and Demandable", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20, VerticalAlignment = Element.ALIGN_MIDDLE });
                    due.AddCell(new PdfPCell(new Paragraph("Due and Demandable", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20, VerticalAlignment = Element.ALIGN_MIDDLE });
                    blnc.AddCell(new PdfPCell(due) { Border = 14 });

                    PdfPTable duedate = new PdfPTable(2);
                    float[] duedate_width = { 10, 10 };
                    duedate.WidthPercentage = 100f;
                    duedate.SetWidths(duedate_width);
                    duedate.AddCell(new PdfPCell(new Paragraph("(a-b)", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 10, VerticalAlignment = Element.ALIGN_MIDDLE });
                    duedate.AddCell(new PdfPCell(new Paragraph("(b-c)", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 10, VerticalAlignment = Element.ALIGN_MIDDLE });
                    blnc.AddCell(new PdfPCell(duedate) { Border = 14 });

                    table_row_11.AddCell(new PdfPCell(blnc) { Border = 14 });

                    doc.Add(table_row_11);

                    


                    if (allotments.FirstOrDefault().fundsource == 1 && allotments.FirstOrDefault().obligation == "fund_source")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" + allotments.FirstOrDefault().fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 1 && allotments.FirstOrDefault().obligation == "sub_allotment")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }

                    if (allotments.FirstOrDefault().fundsource == 2 && allotments.FirstOrDefault().obligation == "fund_source")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault().allotment + "-" + allotments.FirstOrDefault().fundConap + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 2 && allotments.FirstOrDefault().obligation == "sub_allotment")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotmentsAA.FirstOrDefault().allotment + "-" + allotmentsAA.FirstOrDefault().fundConap + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }
                }

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, reader);
                doc.Close(); return File(stream.ToArray(), "application/pdf");
            }

        }

    }
}