using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fmis.Data;
using fmis.Models;
using fmis.Models.Accounting;
using fmis.Filters;
using Microsoft.AspNetCore.Identity;

using fmis.Data.Accounting;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using fmis.Models.John;
using DinkToPdf;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using MySqlX.XDevAPI.Common;

namespace fmis.Controllers.Accounting
{
    [Authorize(Policy = "AccountingAdmin")]
    public class IndexOfPaymentController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        private readonly CategoryContext _CategoryContext;
        private readonly DeductionContext _DeductionContext;
        private readonly DvContext _DvContext;
        private readonly IndexofpaymentContext _IndexofpaymentContext;

        public IndexOfPaymentController(MyDbContext MyDbContext, CategoryContext categoryContext, DeductionContext deductionContext, DvContext dvContext, IndexofpaymentContext indexofpaymentContext)
        {
            _MyDbContext = MyDbContext;
            _CategoryContext = categoryContext;
            _DeductionContext = deductionContext;
            _DvContext = dvContext;
            _IndexofpaymentContext = indexofpaymentContext;
        }


        [Route("Accounting/IndexOfPayment")]
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            /*var indexData = await _MyDbContext.Indexofpayment
                .Include(x => x.Category)
                .Include(x => x.Dv)
                    .ThenInclude(x => x.Payee)
                .Include(x => x.indexDeductions)
                    .ThenInclude(x=>x.Deduction)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                ViewBag.Search = searchString;
                indexData = indexData.Where(x => x.Category.CategoryDescription.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) || x.Dv.DvNo.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) || x.Dv.PayeeDesc.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }*/

            var indexData = from c in _MyDbContext.Indexofpayment
                            .Include(x => x.Category)
                            .Include(x => x.Dv)
                                .ThenInclude(x => x.Payee)
                            .Include(x => x.indexDeductions)
                                .ThenInclude(x => x.Deduction)
                            select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                indexData = indexData.Where(x => x.Category.CategoryDescription.Contains(searchString) || x.Dv.DvNo.Contains(searchString) || x.Dv.PayeeDesc.Contains(searchString));
            }

            ViewBag.indexCategory = indexData.Where(x => x.Category.CategoryDescription.Contains(searchString));
            ViewBag.indexDv = indexData;
            ViewBag.indexPayee = indexData.Where(x => x.Dv.PayeeDesc.Contains(searchString));
            ViewBag.searchString = searchString;


            return View(await indexData.ToListAsync());


        }

        public IActionResult selectAT(int id)
        {
            var branches = _MyDbContext.Dv.Include(x => x.Payee).ToList();
            return Json(branches.Where(x => x.DvId == id).ToList());
        }

        // GET: Create

        public IActionResult Create(int CategoryId, int DeductionId, int DvId)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");

            ViewBag.CategoryId = CategoryId;
            ViewBag.DeductionId = DeductionId;
            ViewBag.DvId = DvId;

            PopulateCategoryDropDownList();
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();

            IndexOfPayment newDv = new() { indexDeductions = new List<IndexDeduction>(7) };
            for (int x = 0; x < 7; x++)
            {
                newDv.indexDeductions.Add(new IndexDeduction());
            }
            return View(newDv);
        }
        // POST: Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IndexOfPayment indexOfPayment)
        {
            indexOfPayment.CreatedAt = DateTime.Now;
            indexOfPayment.UpdatedAt = DateTime.Now;
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");
            indexOfPayment.TotalDeduction = indexOfPayment.indexDeductions.Sum(x => x.Amount);
            indexOfPayment.NetAmount = indexOfPayment.GrossAmount - indexOfPayment.TotalDeduction;

            if (ModelState.IsValid)
            {
                indexOfPayment.indexDeductions = indexOfPayment.indexDeductions.Where(x => x.DeductionId != 0 && x.Amount != 0).ToList();
                _MyDbContext.Add(indexOfPayment);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(indexOfPayment);
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");
            PopulateCategoryDropDownList();
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();
            if (id == null)
            {
                return NotFound();
            }

            var Index = await _MyDbContext.Indexofpayment.FindAsync(id);
            if (Index == null)
            {
                return NotFound();
            }
            return View(Index);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IndexOfPayment index)
        {

            var indexes = await _MyDbContext.Indexofpayment.Where(x => x.IndexOfPaymentId == index.IndexOfPaymentId).AsNoTracking().FirstOrDefaultAsync();

            PopulateCategoryDropDownList();
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();

            _MyDbContext.Update(indexes);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(String id)
        {
            Int32 ID = Convert.ToInt32(id);
            var dvs = await _MyDbContext.Indexofpayment
                .Include(x => x.Category)
                .Include(x => x.Dv)
                    .ThenInclude(x => x.Payee)
                .Include(x => x.indexDeductions)
                    .ThenInclude(x => x.Deduction)
                .FirstOrDefaultAsync(x => x.IndexOfPaymentId == ID);

            _MyDbContext.Indexofpayment.Remove(dvs);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void PopulateCategoryDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Category
                        orderby d.CategoryId
                        select d;
            ViewBag.CategoryId = new SelectList(Query, "CategoryId", "CategoryDescription", selected);
        }
        private void PopulateDvDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Dv
                        orderby d.DvId
                        select d;
            ViewBag.DvId = new SelectList(Query, "DvId", "DvNo", selected);
        }
        private void PopulateDeductionDropDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Deduction
                        orderby d.DeductionId
                        select d;
            ViewBag.DeductionId = new SelectList(Query, "DeductionId", "DeductionDescription", selected);
        }

        public IActionResult Export(string searchString)
        {


            DataTable dt = new DataTable("Saob Report");
            using (XLWorkbook wb = new XLWorkbook())
            {

                


                var ws = wb.Worksheets.Add(dt);

                var currentRow = 2;

                var indexData = _MyDbContext.Indexofpayment
                                            .Include(x => x.Category)
                                            .Include(x => x.Dv)
                                                .ThenInclude(x => x.Payee)
                                            .Include(x => x.indexDeductions)
                                                .ThenInclude(x => x.Deduction)
                                            .Where(x => x.Dv.DvNo == searchString).ToList();


                ws.Cell("A1").Style.Font.FontSize = 10;
                ws.Cell("A1").Style.Font.FontName = "Calibri Light";
                ws.Cell("A1").Value = "Category";
                ws.Columns(1, 2).Width = 15;
                ws.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("B1").Style.Font.FontSize = 10;
                ws.Cell("B1").Style.Font.FontName = "Calibri Light";
                ws.Cell("B1").Value = "Dv #";
                ws.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("C1").Style.Font.FontSize = 10;
                ws.Cell("C1").Style.Font.FontName = "Calibri Light";
                ws.Cell("C1").Value = "Payee";
                ws.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("D1").Style.Font.FontSize = 10;
                ws.Cell("D1").Style.Font.FontName = "Calibri Light";
                ws.Cell("D1").Value = "Date";
                ws.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("E1").Style.Font.FontSize = 10;
                ws.Cell("E1").Style.Font.FontName = "Calibri Light";
                ws.Cell("E1").Value = "Particulars";
                ws.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("F1").Style.Font.FontSize = 10;
                ws.Cell("F1").Style.Font.FontName = "Calibri Light";
                ws.Cell("F1").Value = "PO #";
                ws.Cell("F1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("G1").Style.Font.FontSize = 10;
                ws.Cell("G1").Style.Font.FontName = "Calibri Light";
                ws.Cell("G1").Value = "Invoice #";
                ws.Cell("G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("H1").Style.Font.FontSize = 10;
                ws.Cell("H1").Style.Font.FontName = "Calibri Light";
                ws.Cell("H1").Value = "Project Id";
                ws.Cell("H1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("I1").Style.Font.FontSize = 10;
                ws.Cell("I1").Style.Font.FontName = "Calibri Light";
                ws.Cell("I1").Value = "# of Billing";
                ws.Cell("I1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("J1").Style.Font.FontSize = 10;
                ws.Cell("J1").Style.Font.FontName = "Calibri Light";
                ws.Cell("J1").Value = "Period Covered";
                ws.Cell("J1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("K1").Style.Font.FontSize = 10;
                ws.Cell("K1").Style.Font.FontName = "Calibri Light";
                ws.Cell("K1").Value = "From-To";
                ws.Cell("K1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("L1").Style.Font.FontSize = 10;
                ws.Cell("L1").Style.Font.FontName = "Calibri Light";
                ws.Cell("L1").Value = "Travel Period";
                ws.Cell("L1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("M1").Style.Font.FontSize = 10;
                ws.Cell("M1").Style.Font.FontName = "Calibri Light";
                ws.Cell("M1").Value = "SO #";
                ws.Cell("M1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("N1").Style.Font.FontSize = 10;
                ws.Cell("N1").Style.Font.FontName = "Calibri Light";
                ws.Cell("N1").Value = "Account #";
                ws.Cell("N1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("O1").Style.Font.FontSize = 10;
                ws.Cell("O1").Style.Font.FontName = "Calibri Light";
                ws.Cell("O1").Value = "Gross Amount";
                ws.Cell("O1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("P1").Style.Font.FontSize = 10;
                ws.Cell("P1").Style.Font.FontName = "Calibri Light";
                ws.Cell("P1").Value = "Deduction";
                ws.Cell("P1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("Q1").Style.Font.FontSize = 10;
                ws.Cell("Q1").Style.Font.FontName = "Calibri Light";
                ws.Cell("Q1").Value = "Total Deductions";
                ws.Cell("Q1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("R1").Style.Font.FontSize = 10;
                ws.Cell("R1").Style.Font.FontName = "Calibri Light";
                ws.Cell("R1").Value = "Net Amount";
                ws.Cell("R1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                foreach (var item in indexData)
                {

                    ws.Cell(currentRow, 1).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 1).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 1).Value = item.Category.CategoryDescription;
                    ws.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 2).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 2).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 2).Value = item.Dv.DvNo;
                    ws.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                    ws.Cell(currentRow, 3).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 3).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 3).Value = item.Dv.PayeeDesc;
                    ws.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 4).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 4).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 4).Value = item.DvDate;
                    ws.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 5).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 5).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 5).Value = item.Particulars;
                    ws.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 6).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 6).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 6).Value = item.PoNumber;
                    ws.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 7).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 7).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 7).Value = item.InvoiceNumber;
                    ws.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 8).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 8).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 8).Value = item.ProjectId;
                    ws.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 9).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 9).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 9).Value = item.NumberOfBill;
                    ws.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 10).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 10).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 10).Value = item.PeriodCover;
                    ws.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 11).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 11).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 11).Value = item.date;
                    ws.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 12).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 12).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 12).Value = item.travel_period;
                    ws.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 13).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 13).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 13).Value = item.SoNumber;
                    ws.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 14).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 14).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 14).Value = item.AccountNumber;
                    ws.Cell(currentRow, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 15).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 15).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 15).Value = item.GrossAmount;
                    ws.Cell(currentRow, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 17).Value = item.TotalDeduction;
                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 18).Value = item.NetAmount;
                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    foreach (var deduction in item.indexDeductions)
                    {
                        ws.Cell(currentRow, 16).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 16).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 16).Value = deduction.Deduction.DeductionDescription + "--" + deduction.Amount;
                        ws.Cell(currentRow, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        currentRow++;
                    }
                }





                ws.Columns().AdjustToContents();
                //ws.Rows().AdjustToContents();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    ws.Rows().AdjustToContents();
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports" + ".xlsx");
                }
            }
        }

    }
}