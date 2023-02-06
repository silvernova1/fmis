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
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Text;
using System.Diagnostics;

//SAMPLE

namespace fmis.Controllers
{
    [Authorize(Policy = "BudgetAdmin")]
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
            [Column(TypeName = "decimal(18,4)")]
            public decimal source_balance { get; set; }
            public string Date { get; set; } 
            public string Dv { get; set; } 
            public string Pr_no { get; set; } 
            public string Po_no { get; set; } 
            public string Payee { get; set; } 
            public string Address { get; set; } 
            public string Particulars { get; set; } 
            public string Ors_no { get; set; }
            public float Gross { get; set; } 
            public string Created_by { get; set; } 
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

        public async Task<ActionResult> GetExpenseCode(int allotmentId)
        {
            var expenseCode = await _MyDbContext.Uacs
                .Where(x => x.uacs_type == allotmentId)
                .Select(x => x.Expense_code)
                .ToListAsync();

            if (expenseCode.Count() == 0) return BadRequest();

            return Ok(new { items = expenseCode });
        }

        public async Task<ActionResult> GetObligation(string title)
        {
            var obligation = await _context
                .Obligation
                .Where(x => x.status == "activated")
                .Include(x => x.ObligationAmounts)
                .Include(x => x.FundSource)
                .Include(x => x.SubAllotment)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FundSource.FundSourceTitle == title);
            if (obligation is not null) return Ok(obligation);

            obligation = await _context
                .Obligation
                .Where(x => x.status == "activated")
                .Include(x => x.ObligationAmounts)
                .Include(x => x.FundSource)
                .Include(x => x.SubAllotment)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SubAllotment.Suballotment_title == title);

            if (obligation is not null) return Ok(obligation);

            return BadRequest();
        }

        public int YearlyRefId => int.Parse(User.FindFirst("YearlyRefId").Value);

        public async Task<IActionResult> Index()
        {
            ViewBag.layout = "_Layout";
            ViewBag.filter = new FilterSidebar("ors", "obligation", "");
            ViewBag.current_user = User.FindFirstValue(ClaimTypes.Name);

            string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            var yearAdded = int.Parse(year);
            var res = next_year.AddYears(-1);
            var lastYr = res.Year.ToString();

            var obligation = await _context
                                    .Obligation
                                    .Where(x => x.status == "activated" && x.yearAdded.Year == yearAdded).OrderBy(x => x.Ors_no)
                                    .Include(x => x.ObligationAmounts.Where(x => x.status == "activated"))
                                    .Include(x => x.FundSource)
                                    .Include(x => x.SubAllotment)
                                    .AsNoTracking()
                                    .ToListAsync();

            var fund_sub_data = (from x in _MyDbContext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId && x.Original != true || x.IsAddToNextAllotment == true || x.FundSourceTitle == "CANCELLED").ToList() select new { source_id = x.FundSourceId, source_title = x.FundSourceTitle, remaining_balance = x.Remaining_balance, source_type = "fund_source", obligated_amount = x.obligated_amount })
                                    .Concat(from y in _MyDbContext.SubAllotment.Where(x => x.Budget_allotment.YearlyReferenceId == YearlyRefId || x.IsAddToNextAllotment == true || x.Suballotment_title == "CANCELLED").ToList() select new { source_id = y.SubAllotmentId, source_title = y.Suballotment_title, remaining_balance = y.Remaining_balance, source_type = "sub_allotment", obligated_amount = y.obligated_amount });

            ViewBag.fund_sub = JsonSerializer.Serialize(fund_sub_data.ToList());
            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            var totalObligated = _MyDbContext.FundSources.Where(x => x.BudgetAllotment.YearlyReferenceId == YearlyRefId).Sum(x => x.obligated_amount) + _MyDbContext.SubAllotment.Where(x => x.Budget_allotment.YearlyReferenceId == YearlyRefId).Sum(x => x.obligated_amount);
            ViewBag.totalObligatedAmount = totalObligated.ToString("##,#00.00");

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

            /*  if (obligation.source_type == "fund_source")
                  obligation.FundSource = await _MyDbContext.FundSources.Where(x => x.FundSourceId == obligation.FundSourceId).ToListAsync();
              else if (obligation.source_type == "sub_allotment")
                  obligation.SubAllotment = await _MyDbContext.Sub_allotment.Where(x => x.SubAllotmentId == obligation.SubAllotmentId).ToListAsync();*/

            /*return Json(obligation);*/

            return View("~/Views/Budget/John/Obligations/ObligationAmount.cshtml", obligation);
        }



        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult openCreatedBy(int id, string obligation_token)
        {
            var obligation = (from o in _MyDbContext.Obligation
                              join u in _MyDbContext.FmisUsers
                              on o.Created_by equals u.Username
                              select new
                              {
                                  user = u.Username,
                                  Id = o.Id,
                                  obligation_token = o.obligation_token
                              }).ToList();

            return Json(obligation.Where(x => x.Id == id).FirstOrDefault().user);
            // return View("~/Views/Budget/John/Obligations/CreatedBy.cshtml"); 
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

        public IActionResult UploadObligation()
        {
            ViewBag.filter = new FilterSidebar("ors", "upload", "");
            return View("~/Views/Budget/John/Obligations/UploadObligation.cshtml");
        }

        private DateTime ToDateTime(string dateString, string format)
        {
            if (!string.IsNullOrEmpty(dateString))
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }
            return default;
        }

        private int ToInt(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                if (int.TryParse(item, out int result))
                {
                    return result;
                }
            }
            return 0;
        }

        [HttpPost]
        public async Task<ActionResult> ImportObligations()
        {
            IFormFile excelfile = Request.Form.Files[0];
            string sWebRootFolder = Directory.GetCurrentDirectory() + @"\UploadFile";
            if (!Directory.Exists(sWebRootFolder)) Directory.CreateDirectory(sWebRootFolder);
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            Stopwatch timer = new();
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var personal_informations = new List<Personal_Information>();
            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            {
                excelfile.CopyTo(fs);
                fs.Flush();
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                StringBuilder sb = new StringBuilder();
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                List<Obligation> obligations = new();
                timer.Start();
                for (int row = 3; row <= rowCount; row++)
                {
                    if (!string.IsNullOrEmpty(worksheet.Cells[row, 9].Text))
                    {
                        var fundSrcTxt = worksheet.Cells[row, 12].Text.Trim() ?? "";
                        var fundSrcId = _MyDbContext.FundSources.FirstOrDefault(x => x.FundSourceTitle == fundSrcTxt)?.FundSourceId;
                        var subAlltId = _MyDbContext.SubAllotment.FirstOrDefault(x => x.Suballotment_title == fundSrcTxt)?.SubAllotmentId;
                        List<ObligationAmount> OAs = new();
                        List<FundSource> fs = new();
                        string obligationToken = Guid.NewGuid().ToString();
                        int start = 14;

                        for (int x = 0; x < 12; x++)
                        {
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, start].Text))
                            {
                                Console.WriteLine(worksheet.Cells[row, start].Text);
                                var uacs = _MyDbContext.Uacs.FirstOrDefault(x => x.Expense_code == worksheet.Cells[row, start].Text).UacsId;
                                var amount = worksheet.Cells[row, (start + 1)].Value?.ToString();

                                OAs.Add(new ObligationAmount()
                                {
                                    UacsId = uacs,
                                    Amount = Convert.ToDecimal(amount),
                                    obligation_token = obligationToken,
                                    obligation_amount_token = Guid.NewGuid().ToString(),
                                    Expense_code = long.Parse(worksheet.Cells[row, start].Text),
                                    status = "activated",
                                });
                            }
                            start += 2;
                        }
                        obligations.Add(new Obligation()
                        {
                            FundSourceId = fundSrcId is not null? fundSrcId : subAlltId is null? 51 : null,
                            SubAllotmentId = subAlltId,
                            source_type = fundSrcId is not null ? "fund_source" : subAlltId is null ? "fund_source" : "sub_allotment",
                            obligation_token = obligationToken,
                            status = "activated",
                            yearAdded = DateTime.Now,
                            Date = ToDateTime(worksheet.Cells[row, 3].Text, "MM/dd/yy"),
                            Dv = worksheet.Cells[row, 4].Text,
                            Pr_no = worksheet.Cells[row, 5].Text,
                            Po_no = worksheet.Cells[row, 6].Text,
                            Payee = worksheet.Cells[row, 7].Text,
                            Address = worksheet.Cells[row, 8].Text,
                            Particulars = worksheet.Cells[row, 9].Text,
                            Ors_no = worksheet.Cells[row, 11].Text,
                            ObligationAmounts = OAs
                        });

                        
                    }
                    //if (row == 1000) break;
                }
                timer.Stop();

            
                await _MyDbContext.AddRangeAsync(obligations);
                var water = await _MyDbContext.SaveChangesAsync();
                var test = sb.ToString();
                return Ok();
                //return Content(sb.ToString());
            }
        }

        [HttpPost]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveObligation(List<ObligationData> data)
        {
            string year = _MyDbContext.Yearly_reference.FirstOrDefault(x => x.YearlyReferenceId == YearlyRefId).YearlyReference;
            DateTime next_year = DateTime.ParseExact(year, "yyyy", null);
            next_year.ToString("yyyy-MM-dd 00:00:00");
            var res = next_year.AddYears(-1);
            var result = res.Year.ToString();


            var data_holder = _context.Obligation.Where(x => x.status == "activated");
            var retObligation = new List<Obligation>();

            foreach (var item in data)
            {
               var obligation = new Obligation(); //CLEAR OBJECT

               if (await data_holder.AnyAsync(s => s.obligation_token == item.obligation_token)) //CHECK IF EXIST
               {
                   obligation = await data_holder.Where(s => s.obligation_token == item.obligation_token).FirstOrDefaultAsync();
               }

               if (item.source_type.Equals("fund_source"))
                   obligation.FundSourceId = item.source_id;
               else if (item.source_type.Equals("sub_allotment"))
                   obligation.SubAllotmentId = item.source_id;

                obligation.source_type = item.source_type;
                obligation.Date = ToDateTime(item.Date);
                obligation.Dv = item.Dv;
                obligation.Pr_no = item.Pr_no;
                obligation.Po_no = item.Po_no;
                obligation.Payee = item.Payee;
                obligation.Address = item.Address;
                obligation.Particulars = item.Particulars;
                obligation.Created_by = item.Created_by;
                obligation.yearAdded = next_year;
                obligation.Gross = item.Gross;
                obligation.Ors_no = item.Ors_no;
                obligation.status = "activated";
                obligation.obligation_token = item.obligation_token;
               _context.Update(obligation);
                await _context.SaveChangesAsync();

                if (item.source_type == "fund_source")
                   obligation.FundSource = await _MyDbContext.FundSources.FirstOrDefaultAsync(x => x.FundSourceId == obligation.FundSourceId);
               else 
                   obligation.SubAllotment = await _MyDbContext.SubAllotment.FirstOrDefaultAsync(x => x.SubAllotmentId == obligation.SubAllotmentId);
                retObligation.Add(obligation);

                Console.WriteLine(@"saved obligation {0}", item.source_id);
            }
            return Json(retObligation.FirstOrDefault());
        }

        public string SetORSNo(string lastORSNo)
        {
            var no = int.Parse(lastORSNo.Substring(0, 4)) + 1;

            return no.ToString().PadLeft(4, '0');
        }

        // POST: Obligations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Dv,Pr_no,Po_no,Payee,Address,Particulars,Ors_no,CreatedBy,Fund_source,Gross,Date_recieved,Time_recieved,Date_released,Time_released")] Obligation obligation)
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

            foreach (var many in data.many_token)
                setUpDeleteData(many.many_token);
            /*
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                    setUpDeleteData(many.many_token);
            }
            else
                setUpDeleteData(data.single_token);*/

            return Json(data);
        }

        public void setUpDeleteData(string obligation_token)
        {
            var obligation = new Obligation(); //CLEAR OBJECT
            obligation = _context.Obligation.Where(s => s.obligation_token == obligation_token && s.status != "deactivated").AsNoTracking().FirstOrDefault();
            if (obligation is not null)
            {
                obligation.status = "deactivated";
                _context.Update(obligation);
                _context.SaveChanges();
            }
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

                    Image logo = Image.GetInstance("wwwroot/assets/images/doh_logo_updated.png");
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
                                      on fundsource.FundSourceId equals obligation.FundSourceId
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

                    var allotmentsAA = (from sub_allotment in _MyDbContext.SubAllotment
                                        join obligation in _MyDbContext.Obligation
                                      on sub_allotment.SubAllotmentId equals obligation.SubAllotmentId
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


                    if (allotments.FirstOrDefault()?.fundsource == 1 && allotments.FirstOrDefault()?.obligation == "fund_source")
                    {

                        Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                        table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundCurrent, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

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


                    if (allotments.FirstOrDefault()?.fundsource == 2 && allotments.FirstOrDefault()?.obligation == "fund_source")
                    {



                        Font column3_font = FontFactory.GetFont("Times New Roman", 8, Font.BOLD, BaseColor.BLACK);

                        table3.AddCell(new PdfPCell(new Paragraph("Serial No.", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundConap + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundConap, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                        /*table3.AddCell(new PdfPCell(new Paragraph("Fund Cluster :", arial_font_10)) { Padding = 6f, Border = 0 });
                        table3.AddCell(new PdfPCell(new Paragraph(budget_allotments.Allotment_series + "-01101101", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });*/

                        table.AddCell(table3);

                        doc.Add(table);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 2 && allotmentsAA.FirstOrDefault()?.obligation == "sub_allotment")
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
                    table_row_4.AddCell(new PdfPCell(new Paragraph(ors.Address.ToString(), arial_font_10)));
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
                                       on fundsource.FundSourceId equals obligation.FundSourceId
                                       join prexc in _MyDbContext.Prexc
                                       on fundsource.PrexcId equals prexc.Id
                                       join respo in _MyDbContext.RespoCenter
                                       on fundsource.RespoId equals respo.RespoId
                                       where obligation.obligation_token == tok
                                       select new
                                       {
                                           pap = prexc.pap_code1,
                                           obligation_id = obligation.FundSourceId,
                                           source_type = obligation.source_type,
                                           fundsource_id = fundsource.FundSourceId,
                                           fundsource_code = fundsource.FundSourceTitle,
                                           respo = respo.RespoCode,                     
                                           signatory = respo.RespoHead,
                                           position = respo.RespoHeadPosition,
                                           particulars = obligation.Particulars
                                       }).ToList();

                    var saa = (from SAA in _MyDbContext.SubAllotment
                               join obligation in _MyDbContext.Obligation
                               on SAA.SubAllotmentId equals obligation.SubAllotmentId
                               join prexc in _MyDbContext.Prexc
                               on SAA.prexcId equals prexc.Id
                               join respo in _MyDbContext.RespoCenter
                               on SAA.RespoId equals respo.RespoId
                               where obligation.obligation_token == tok
                               select new
                               {
                                   pap = prexc.pap_code1,
                                   obligation_id = obligation.SubAllotmentId,
                                   source_type = obligation.source_type,
                                   fundsource_id = SAA.SubAllotmentId,
                                   fundsource_code = SAA.Suballotment_title,
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

                    if (fundsources.FirstOrDefault()?.source_type == "fund_source")
                    {
                        table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + fundsources.FirstOrDefault()?.fundsource_code + "\n\n" + fundsources.FirstOrDefault()?.respo, FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                    }
                    else if (saa.FirstOrDefault()?.source_type == "sub_allotment")
                    {
                        table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + saa.FirstOrDefault()?.fundsource_code + "\n\n" + saa.FirstOrDefault()?.respo, FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                    }

                    //table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + fundsources.FirstOrDefault()?.fundsource_code + "\n\n" + fundsources.FirstOrDefault()?.respo, FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + ors.Particulars, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_LEFT });
                    table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + fundsources.FirstOrDefault()?.pap, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
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

                    var saasignatory = (from rc in _MyDbContext.RespoCenter
                                        join sa in _MyDbContext.SubAllotment
                                        on rc.RespoId equals sa.RespoId
                                        select new
                                        {
                                            respoHead = rc.RespoHead,
                                            respoPosition = rc.RespoHeadPosition

                                        }).ToList();


                    if (allotments.FirstOrDefault()?.obligation == "fund_source")
                    {
                        //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                        table_row_8.AddCell(new PdfPCell(new Paragraph(fundsources.FirstOrDefault()?.signatory, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                        table_row_8.AddCell(new PdfPCell(new Paragraph("LEONORA A. ANIEL", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                        table_row_8.AddCell(new PdfPCell(new Paragraph("Position :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                        //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                        table_row_8.AddCell(new PdfPCell(new Paragraph(fundsources.FirstOrDefault()?.position, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
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
                    }
                    else
                    {
                        //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE TEST CHANGES
                        table_row_8.AddCell(new PdfPCell(new Paragraph(saasignatory.FirstOrDefault()?.respoHead, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                        table_row_8.AddCell(new PdfPCell(new Paragraph("LEONORA A. ANIEL", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                        table_row_8.AddCell(new PdfPCell(new Paragraph("Position :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                        //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                        table_row_8.AddCell(new PdfPCell(new Paragraph(saasignatory.FirstOrDefault()?.respoPosition, new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
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
                    }

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




                    if (allotments.FirstOrDefault()?.fundsource == 1 && allotments.FirstOrDefault()?.obligation == "fund_source")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundCurrent + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 1 && allotmentsAA.FirstOrDefault()?.obligation == "sub_allotment")
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

                    if (allotments.FirstOrDefault()?.fundsource == 2 && allotments.FirstOrDefault()?.obligation == "fund_source")
                    {
                        PdfPTable table_row_12 = new PdfPTable(8);
                        table_row_12.WidthPercentage = 100f;
                        table_row_12.SetWidths(new float[] { 15, 20, 20, 15, 15, 15, 15, 15 });

                        table_row_12.AddCell(new PdfPCell(new Paragraph(ors.Date.ToShortDateString(), FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(allotments.FirstOrDefault()?.allotment + "-" + allotments.FirstOrDefault()?.fundConap + "-" + ors.Date.ToString("yyyy-MM") + "-" + "000" + ors.Id, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 150, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("C", new CultureInfo("en-PH")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });
                        table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 14 });

                        doc.Add(table_row_12);

                    }

                    if (allotmentsAA.FirstOrDefault()?.sub_allotment == 2 && allotmentsAA.FirstOrDefault()?.obligation == "sub_allotment")
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