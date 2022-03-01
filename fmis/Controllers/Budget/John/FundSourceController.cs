using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data.John;
using fmis.Models.John;
using fmis.Models;
using fmis.Data;
using fmis.ViewModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using fmis.Filters;
using fmis.Models.silver;

namespace fmis.Controllers.Budget.John
{
    public class FundSourceController : Controller
    {
        private readonly FundSourceContext _FundSourceContext;
        private readonly UacsContext _uContext;
        private readonly BudgetAllotmentContext _bContext;
        private readonly PrexcContext _pContext;
        private readonly RespoCenterContext _rContext;
        private readonly MyDbContext _MyDbContext;

        public FundSourceController(FundSourceContext FundSourceContext, UacsContext uContext, BudgetAllotmentContext bContext, PrexcContext pContext, MyDbContext MyDbContext, BudgetAllotmentContext BudgetAllotmentContext, RespoCenterContext rContext)
        {
            _FundSourceContext = FundSourceContext;
            _uContext = uContext;
            _bContext = bContext;
            _pContext = pContext;
            _MyDbContext = MyDbContext;
            _rContext = rContext;
        }

        public class FundsourceamountData
        {
            public int FundSourceId { get; set; }
            public int UacsId { get; set; }
            public decimal Amount { get; set; }
            public int Id { get; set; }
            public string fundsource_amount_token { get; set; }
            public string fundsource_token { get; set; }
            public int BudgetAllotmentId { get; set; }
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

        public async Task<IActionResult> Index(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;

            var budget_allotment = await _MyDbContext.Budget_allotments
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.RespoCenter)
            .Include(x => x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x => x.Appropriation)
            .Include(x=>x.FundSources.Where(x => x.AllotmentClassId == AllotmentClassId && x.AppropriationId == AppropriationId))
                .ThenInclude(x=>x.AllotmentClass)
            .Include(x=>x.Yearly_reference)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BudgetAllotmentId == BudgetAllotmentId);

            return View(budget_allotment);
        }


        // GET: FundSource/Create
        public async Task<IActionResult> Create(int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;

            string ac = AllotmentClassId.ToString();

            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.Where(x=>x.uacs_type == AllotmentClassId).ToListAsync());
            ViewBag.uacs = uacs_data;


            PopulatePrexcsDropDownList();
            PopulateRespoDropDownList();
            PopulateFundDropDownList();

            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            return View(); //open create
        }

        // POST: FundSource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FundSource fundSource, int PrexcId, int FundId)
        {
            fundSource.CreatedAt = DateTime.Now;
            fundSource.UpdatedAt = DateTime.Now;
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            var result = _MyDbContext.Prexc.Where(x => x.Id == PrexcId).First();
            var result2 = _MyDbContext.Fund.Where(x => x.FundId == FundId).First();


            var fundsource_amount = _MyDbContext.FundSourceAmount.Where(f => f.fundsource_token == fundSource.token).ToList();

            fundSource.Beginning_balance = fundsource_amount.Sum(x => x.beginning_balance);
            fundSource.Remaining_balance = fundsource_amount.Sum(x => x.beginning_balance);

            _FundSourceContext.Add(fundSource);
            _FundSourceContext.SaveChanges();

            fundsource_amount.ForEach(a => a.FundSourceId = fundSource.FundSourceId);
            this._MyDbContext.SaveChanges();

            return RedirectToAction("index", "FundSource", new
            {
                AllotmentClassId = fundSource.AllotmentClassId,
                AppropriationId = fundSource.AppropriationId,
                BudgetAllotmentId = fundSource.BudgetAllotmentId
            });
        }


        //POST
        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.Prexc.ToList();
            return Json(branches.Where(x => x.Id == id).ToList());
        }

        // GET: FundSource/Edit/5
        public async Task<IActionResult> Edit(int fund_source_id, int AllotmentClassId, int AppropriationId, int BudgetAllotmentId)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");

            ViewBag.AllotmentClassId = AllotmentClassId;
            ViewBag.AppropriationId = AppropriationId;
            ViewBag.BudgetAllotmentId = BudgetAllotmentId;

            var fundsource = _MyDbContext.FundSources.Where(x => x.FundSourceId == fund_source_id)
                .Include(x => x.FundSourceAmounts.Where(x => x.status == "activated"))
                .FirstOrDefault();


            var uacs_data = JsonSerializer.Serialize(await _MyDbContext.Uacs.ToListAsync());
            ViewBag.uacs = uacs_data;

            PopulatePrexcsDropDownList(fundsource.PrexcId);
            PopulateRespoDropDownList();
            PopulateFundDropDownList();

            return View(fundsource);
        }


        // POST: FundSource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FundSource fundSource)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var funsource_amount = await _MyDbContext.FundSourceAmount.Where(f => f.FundSourceId == fundSource.FundSourceId && f.status == "activated").AsNoTracking().ToListAsync();
            var beginning_balance = funsource_amount.Sum(x => x.beginning_balance);
            var remaining_balance = funsource_amount.Sum(x => x.remaining_balance);

            var fundsource_data = await _MyDbContext.FundSources.Where(s => s.FundSourceId == fundSource.FundSourceId).AsNoTracking().FirstOrDefaultAsync();
            fundsource_data.PrexcId = fundSource.PrexcId;
            fundsource_data.FundId = fundSource.FundId;
            fundsource_data.FundSourceTitle = fundSource.FundSourceTitle;
            fundsource_data.FundSourceTitleCode = fundSource.FundSourceTitleCode;
            fundsource_data.PapType = fundSource.PapType;
            fundsource_data.RespoId = fundSource.RespoId;
            fundsource_data.Beginning_balance = beginning_balance;
            fundsource_data.Remaining_balance = remaining_balance;

            _FundSourceContext.Update(fundsource_data);
            await _FundSourceContext.SaveChangesAsync();

            return RedirectToAction("Index", "FundSource", new 
            {
                AllotmentClassId = fundSource.AllotmentClassId,
                AppropriationId = fundSource.AppropriationId,
                BudgetAllotmentId = fundSource.BudgetAllotmentId
            });

        }

        [HttpPost]
        public IActionResult SaveFundsourceamount(List<FundsourceamountData> data)
        {
            var data_holder = _MyDbContext.FundSourceAmount;

            foreach (var item in data)
            {
                var fundsource_amount = new FundSourceAmount(); //CLEAR OBJECT
                if (data_holder.Where(s => s.fundsource_amount_token == item.fundsource_amount_token).FirstOrDefault() != null) //CHECK IF EXIST
                    fundsource_amount = data_holder.Where(s => s.fundsource_amount_token == item.fundsource_amount_token).FirstOrDefault();

                fundsource_amount.FundSourceId = item.FundSourceId == 0 ? null : item.FundSourceId;
                fundsource_amount.BudgetAllotmentId = item.BudgetAllotmentId;
                fundsource_amount.UacsId = item.UacsId;
                fundsource_amount.beginning_balance = item.Amount;
                fundsource_amount.remaining_balance = item.Amount;
                fundsource_amount.status = "activated";
                fundsource_amount.fundsource_amount_token = item.fundsource_amount_token;
                fundsource_amount.fundsource_token = item.fundsource_token;
                _MyDbContext.FundSourceAmount.Update(fundsource_amount);
                this._MyDbContext.SaveChanges();
            }

            return Json(data);
        }

        

        /*DROPDOWN LIST FOR PREXC*/

        private void PopulatePrexcsDropDownList(object selectedDepartment = null)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var departmentsQuery = from d in _pContext.Prexc
                                   orderby d.pap_title
                                   select d;
            ViewBag.PrexcId = new SelectList((from s in _pContext.Prexc.ToList()
                                              select new
                                              {
                                                  PrexcId = s.Id,
                                                  prexc = s.pap_title,
                                                  pap_type = s.pap_type,
                                              }),
                                       "PrexcId",
                                       "prexc",
                                       "pap_type",
                                       null);

        }

        private void PopulateRespoDropDownList()
        {
            ViewBag.RespoId = new SelectList((from s in _MyDbContext.RespoCenter.ToList()
                                              select new
                                              {
                                                  RespoId = s.RespoId,
                                                  respo = s.Respo
                                              }),
                                     "RespoId",
                                     "respo",
                                     null);

        }


        private void PopulateFundDropDownList()
        {
            var departmentsQuery = from d in _MyDbContext.Fund
                                   orderby d.Fund_description
                                   select d;
            ViewBag.FundId = new SelectList((from s in _MyDbContext.Fund.ToList()
                                                select new
                                                {
                                                    FundId = s.FundId,
                                                    FundDescription = s.Fund_description
                                                }),
                                       "FundId",
                                       "FundDescription",
                                       null);

        }

      

        // GET: FundSource/Delete/5
        public async Task<IActionResult> Delete(int? id, int? BudgetId, int budget_id)
        {
            ViewBag.BudgetId = BudgetId;
            ViewBag.budget_id = budget_id;

            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            if (id == null)
            {
                return NotFound();
            }

            var fundSource = await _FundSourceContext.FundSource
                .FirstOrDefaultAsync(m => m.FundSourceId == id);
            if (fundSource == null)
            {
                return NotFound();
            }

            return View(fundSource);
        }

        // POST: FundSource/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteFundsourceamount(DeleteData data)
        {
            if (data.many_token.Count > 1)
            {
                foreach (var many in data.many_token)
                {
                    var fund_source_amount = _MyDbContext.FundSourceAmount.FirstOrDefault(s => s.fundsource_amount_token == many.many_token);
                    fund_source_amount.status = "deactivated";

                    _MyDbContext.FundSourceAmount.Update(fund_source_amount);
                    await _MyDbContext.SaveChangesAsync();

                    var fund_source_update = await _FundSourceContext.FundSource.AsNoTracking().FirstOrDefaultAsync(s => s.token == fund_source_amount.fundsource_token);
                    fund_source_update.Remaining_balance -= fund_source_amount.beginning_balance;

                    //detach para ma calculate ang multiple delete
                    var local = _FundSourceContext.Set<FundSource>()
                            .Local
                            .FirstOrDefault(entry => entry.token.Equals(fund_source_amount.fundsource_token));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _FundSourceContext.Entry(local).State = EntityState.Detached;
                    }
                    // set Modified flag in your entry
                    _FundSourceContext.Entry(fund_source_update).State = EntityState.Modified;
                    //end detach

                    _FundSourceContext.FundSource.Update(fund_source_update);
                    _FundSourceContext.SaveChanges();
                }
            }
            else
            {
                var fund_source_amount = _MyDbContext.FundSourceAmount.FirstOrDefault(s => s.fundsource_amount_token == data.single_token);
                fund_source_amount.status = "deactivated";

                _MyDbContext.FundSourceAmount.Update(fund_source_amount);
                await _MyDbContext.SaveChangesAsync();

                var fund_source_update = await _FundSourceContext.FundSource.AsNoTracking().FirstOrDefaultAsync(s => s.token == fund_source_amount.fundsource_token);
                fund_source_update.Remaining_balance -= fund_source_amount.beginning_balance;
                _FundSourceContext.FundSource.Update(fund_source_update);
                _FundSourceContext.SaveChanges();
            }

            return Json(data);
        }
        // POST: FundSource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            var fundSource = await _FundSourceContext.FundSource.FindAsync(id);
            _FundSourceContext.FundSource.Remove(fundSource);
            await _FundSourceContext.SaveChangesAsync();
            return RedirectToAction("Index", "FundSource", new { budget_id = fundSource.BudgetAllotmentId });
        }

        private bool FundSourceExists(int id)
        {
            ViewBag.filter = new FilterSidebar("master_data", "budgetallotment", "");
            return _FundSourceContext.FundSource.Any(e => e.FundSourceId == id);
        }

        //EXPORTING PDF FILE

 /*       public FileResult Export(String id)
        {
            Int32 Id = Convert.ToInt32(id);
            var ors = _MyDbContext.Obligation.Where(p => p.Id == Id).FirstOrDefault();

            string ExportData = "This is pdf generated";
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader reader = new StringReader(ExportData);
                Document PdfFile = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                PdfFile.Open();

                PdfPTable table = null;

                var titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);


                table = new PdfPTable(1);
                table.TotalWidth = 800f;
                table.LockedWidth = true;
                table.SpacingBefore = 10;
                table.SpacingAfter = 10;
                table.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfFile.Add(table);

                PdfFile.Add(table);

                PdfFile.Open();

                Paragraph header_text = new Paragraph("OBLIGATION REQUEST AND STATUS");

                header_text.Font = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                header_text.Alignment = Element.ALIGN_CENTER;
                PdfFile.Add(header_text);

                Paragraph nextline = new Paragraph("\n");
                PdfFile.Add(nextline);

                table = new PdfPTable(3);
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

                Font arial_font_10 = FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK);

                var table2 = new PdfPTable(1);
                table2.DefaultCell.Border = 0;
                table2.AddCell(new PdfPCell(new Paragraph("Republic of Philippines", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Department Of Health", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Regional Office 7", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                table2.AddCell(new PdfPCell(new Paragraph("Central Visayas, Osmeña Blvd. Cebu City", arial_font_10)) { Padding = 6f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });



                var no_left_bor = new PdfPCell(table2);
                no_left_bor.DisableBorderSide(4);
                table.AddCell(no_left_bor);

                var table3 = new PdfPTable(2);
                float[] table3widths = { 4, 10 };
                table3.SetWidths(table3widths);
                table3.DefaultCell.Border = 0;

                Font column3_font = FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK);

                table3.AddCell(new PdfPCell(new Paragraph("No :", arial_font_10)) { Padding = 6f, Border = 0 });
                table3.AddCell(new PdfPCell(new Paragraph("" + " - 01101101 - " + "2021" + " - " + "09", column3_font)) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                table3.AddCell(new PdfPCell(new Paragraph("Date :", arial_font_10)) { Padding = 6f, Border = 0 });
                table3.AddCell(new PdfPCell(new Paragraph("BOY OTS", column3_font)) { Border = 2, Padding = 6f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                table3.AddCell(new PdfPCell(new Paragraph("Fund :", arial_font_10)) { Padding = 6f, Border = 0 });
                table3.AddCell(new PdfPCell(new Paragraph("321321" + "-01101101", column3_font)) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5 });

                table3.AddCell(new PdfPCell(new Paragraph("", arial_font_10)) { Padding = 6f, Border = 0 });
                table3.AddCell(new PdfPCell(new Paragraph("", column3_font)) { Padding = 6f, Border = 2, HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 5, PaddingBottom = 4 });

                table.AddCell(table3);

                PdfFile.Add(table);

                var table_row_2 = new PdfPTable(3);
                float[] tbt_row2_width = { 5, 15, 10 };
                table_row_2.WidthPercentage = 100f;
                table_row_2.SetWidths(tbt_row2_width);
                table_row_2.AddCell(new PdfPCell(new Paragraph("Payee", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_2.AddCell(new PdfPCell(new Paragraph("PAYEE", arial_font_10)));
                table_row_2.AddCell(new PdfPCell(new Paragraph("", arial_font_10)));


                PdfFile.Add(table_row_2);

                var table_row_3 = new PdfPTable(3);
                float[] tbt_row3_width = { 5, 15, 10 };
                table_row_3.WidthPercentage = 100f;
                table_row_3.SetWidths(tbt_row3_width);
                table_row_3.AddCell(new PdfPCell(new Paragraph("Office", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_3.AddCell(new PdfPCell(new Paragraph("Department of Health", arial_font_10)));
                table_row_3.AddCell(new PdfPCell(new Paragraph()));

                PdfFile.Add(table_row_3);

                var table_row_4 = new PdfPTable(3);
                float[] tbt_row4_width = { 5, 15, 10 };
                table_row_4.WidthPercentage = 100f;
                table_row_4.SetWidths(tbt_row4_width);
                table_row_4.AddCell(new PdfPCell(new Paragraph("Address", arial_font_10)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_4.AddCell(new PdfPCell(new Paragraph("Sambag II", arial_font_10)));
                table_row_4.AddCell(new PdfPCell(new Paragraph()));


                PdfFile.Add(table_row_4);

                Font table_row_5_font = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);

                var table_row_5 = new PdfPTable(5);
                float[] tbt_row5_width = { 5, 10, 5, 5, 5 };
                table_row_5.WidthPercentage = 100f;
                table_row_5.SetWidths(tbt_row5_width);
                table_row_5.AddCell(new PdfPCell(new Paragraph("Responsibility Center", table_row_5_font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_5.AddCell(new PdfPCell(new Paragraph("Particulars", table_row_5_font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_5.AddCell(new PdfPCell(new Paragraph("MFO/PAP", table_row_5_font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_5.AddCell(new PdfPCell(new Paragraph("UACS Code/ Expenditure", table_row_5_font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_5.AddCell(new PdfPCell(new Paragraph("Amount", table_row_5_font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });

                PdfFile.Add(table_row_5);

                var table_row_6 = new PdfPTable(5);
                float[] tbt_ro6_width = { 5, 10, 5, 5, 5 };
                table_row_6.WidthPercentage = 100f;
                table_row_6.SetWidths(tbt_ro6_width);

                Double total_amt = 0.00;
                String str_amt = "";
                String uacs = "";
                Double disbursements = 0.00;

                //Loop the data

                *//*var ors_uacs = (from uacs_list in db.ors_expense_codes
                                join expensecodes in db.uacs on uacs_list.uacs equals expensecodes.Title
                                where uacs_list.ors_obligation == ors.ID
                                select new
                                {
                                    uacs = expensecodes.Code,
                                    amount = uacs_list.amount,
                                    net_amt = uacs_list.NetAmount,
                                    tax_amt = uacs_list.TaxAmount,
                                    others = uacs_list.Others
                                }).ToList();
                var FundSourceDetails = (from details in db.ors
                                         join allotment in db.allotments on details.allotment equals allotment.ID
                                         join ors_fundsource in db.fsh on allotment.ID.ToString() equals ors_fundsource.allotment
                                         where details.ID == ID &&
                                         ors_fundsource.Code == ors.FundSource
                                         select new
                                         {
                                             PrexcCode = ors_fundsource.prexc,
                                             ResponsibilityNumber = ors_fundsource.Responsibility_Number
                                         }).FirstOrDefault();
                foreach (var u in ors_uacs)
                {
                    uacs += u.uacs + "\n";
                    str_amt += u.amount.ToString("N", new CultureInfo("en-US")) + "\n";
                    total_amt += u.amount;
                    disbursements += u.net_amt + u.tax_amt + u.others;
                }*//*

                //

                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "Kabaso Data" + "\n\n" + "Kabaso Data", table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "Kabaso Data", table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_LEFT });
                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "Kabaso Data", table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + uacs, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_CENTER, PaddingBottom = 15f });
                table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + str_amt, table_row_5_font)) { Border = 13, FixedHeight = 150f, HorizontalAlignment = Element.ALIGN_RIGHT, PaddingBottom = 15f });
                PdfFile.Add(table_row_6);


                var table_row_7 = new PdfPTable(5);
                float[] tbt_row7_width = { 5, 10, 5, 5, 5 };

                table_row_7.WidthPercentage = 100f;
                table_row_7.SetWidths(tbt_row7_width);
                table_row_7.AddCell(new PdfPCell(new Paragraph("", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });



                //REMOVE BORDER
                PdfPTable po_dv = new PdfPTable(2);
                po_dv.WidthPercentage = 100f;

                po_dv.AddCell(new PdfPCell(new Paragraph("PO No." + "Kabaso Data", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                po_dv.AddCell(new PdfPCell(new Paragraph("PR No. " + "Kabaso Data", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                po_dv.AddCell(new PdfPCell(new Paragraph("DV No. " + "Kabaso Data", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });
                po_dv.AddCell(new PdfPCell(new Phrase("Total", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                // END OF REMOVE BORDER
                table_row_7.AddCell(new PdfPCell(po_dv) { Border = 14 });



                table_row_7.AddCell(new PdfPCell(new Paragraph("", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_7.AddCell(new PdfPCell(new Paragraph("", table_row_5_font)) { Border = 14, HorizontalAlignment = Element.ALIGN_CENTER });


                PdfPTable tbt_total_amt = new PdfPTable(1);
                float[] tbt_total_amt_width = { 10 };

                tbt_total_amt.WidthPercentage = 100f;
                tbt_total_amt.SetWidths(tbt_total_amt_width);

                tbt_total_amt.AddCell(new PdfPCell(new Paragraph("\n", table_row_5_font)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                tbt_total_amt.AddCell(new PdfPCell(new Paragraph(total_amt.ToString("N", new CultureInfo("en-US")), table_row_5_font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table_row_7.AddCell(new PdfPCell(tbt_total_amt) { Border = 14 });

                PdfFile.Add(table_row_7);


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
                table_row_8.AddCell(new PdfPCell(new Paragraph("Signature :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { Border = 14 });

                table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))) { Border = 14 });

                //SIGNATURE 2
                table_row_8.AddCell(new PdfPCell(new Paragraph("Signature :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))) { Border = 14 });

                table_row_8.AddCell(new PdfPCell(new Paragraph("", new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLD))) { Border = 14 });


                //var ors_requesting_head = db.ors_head_request.Where(p => p.Name == ors.head_requesting_office).FirstOrDefault();

                *//*var ors_fsh = (from list in db.ors
                               join allotment in db.allotments on list.allotment equals allotment.ID
                               join fsh in db.fsh on allotment.ID.ToString() equals fsh.allotment
                               where fsh.Code == ors.FundSource && fsh.allotment == allotment.ID.ToString()
                               && list.ID.ToString() == id
                               select new
                               {
                                   ors_head = fsh.ors_head
                               }).FirstOrDefault();*//*


                //var ors_head = db.ors_head_request.Where(p => p.ID.ToString() == ors_fsh.ors_head.ToString()).FirstOrDefault();

                table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                table_row_8.AddCell(new PdfPCell(new Paragraph("Enero Amalio" != null ? "Enerz" : "", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_8.AddCell(new PdfPCell(new Paragraph("Printed Name", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                table_row_8.AddCell(new PdfPCell(new Paragraph("LEONORA A. ANIEL", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                table_row_8.AddCell(new PdfPCell(new Paragraph("Position :", new Font(Font.FontFamily.HELVETICA, 6f, Font.NORMAL))));
                //HEAD REQUESTING OFFICE / AUTHORIZED REPRESENTATIVE
                table_row_8.AddCell(new PdfPCell(new Paragraph("Enero Amalio" != null ? "Master Programmer" : "", new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
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
                table_row_8.AddCell(new PdfPCell(new Paragraph(DateTime.Now.ToString("MM/dd/yyyy"), new Font(Font.FontFamily.HELVETICA, 7f, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });


                PdfFile.Add(table_row_8);

                PdfPTable table_row_9 = new PdfPTable(2);
                table_row_9.WidthPercentage = 100f;
                table_row_9.SetWidths(new float[] { 10, 90 });
                table_row_9.AddCell(new PdfPCell(new Paragraph("C.", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))));
                table_row_9.AddCell(new PdfPCell(new Paragraph("STATUS OF OBLIGATION", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                PdfFile.Add(table_row_9);

                PdfPTable table_row_10 = new PdfPTable(2);
                table_row_10.WidthPercentage = 100f;
                table_row_10.SetWidths(new float[] { 50, 50 });
                table_row_10.AddCell(new PdfPCell(new Paragraph("Reference", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_10.AddCell(new PdfPCell(new Paragraph("Amount", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                PdfFile.Add(table_row_10);

                PdfPTable table_row_11 = new PdfPTable(7);
                table_row_11.WidthPercentage = 100f;
                table_row_11.SetWidths(new float[] { 12, 20, 28, 15, 15, 10, 20 });

                table_row_11.AddCell(new PdfPCell(new Paragraph("Date", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("Particulars", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("ORS/JEV/RCI/RADAI No.", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("Payment", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("Not Yet Due", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });
                table_row_11.AddCell(new PdfPCell(new Paragraph("Due and \n Demandable", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 35, VerticalAlignment = Element.ALIGN_MIDDLE });

                PdfFile.Add(table_row_11);

                PdfPTable table_row_12 = new PdfPTable(7);
                table_row_12.WidthPercentage = 100f;
                table_row_12.SetWidths(new float[] { 12, 20, 28, 15, 15, 10, 20 });

                table_row_12.AddCell(new PdfPCell(new Paragraph("2021", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 13 });
                table_row_12.AddCell(new PdfPCell(new Paragraph("Obligation", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100, Border = 13 });
                table_row_12.AddCell(new PdfPCell(new Paragraph("Allotments" + " - 01101101 - " + "Ors Date" + " - " + "Ors Row", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100 });
                table_row_12.AddCell(new PdfPCell(new Paragraph(total_amt > 0 ? total_amt.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100 });
                table_row_12.AddCell(new PdfPCell(new Paragraph(disbursements > 0 ? disbursements.ToString("N", new CultureInfo("en-US")) : "", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100 });
                table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100 });
                table_row_12.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 100 });

                PdfFile.Add(table_row_12);

                PdfPTable table_row_13 = new PdfPTable(7);
                table_row_13.WidthPercentage = 100f;
                table_row_13.SetWidths(new float[] { 12, 20, 28, 15, 15, 10, 20 });


                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 12 });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 12 });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_13.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                PdfFile.Add(table_row_13);


                PdfPTable table_row_14 = new PdfPTable(7);
                table_row_14.WidthPercentage = 100f;
                table_row_14.SetWidths(new float[] { 12, 20, 28, 15, 15, 10, 20 });

                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 14 });
                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 14 });
                table_row_14.AddCell(new PdfPCell(new Paragraph("Totals", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table_row_14.AddCell(new PdfPCell(new Paragraph("\n", FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_CENTER });

                PdfFile.Add(table_row_14);


                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                PdfFile.Close(); return File(stream.ToArray(), "application/pdf");


            }
        }*/
    }
}