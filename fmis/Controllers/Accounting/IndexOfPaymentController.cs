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
using Sitecore.FakeDb;
using System.Text.Json;

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

            var indexData = from c in _MyDbContext.Indexofpayment
                            .Include(x => x.Category)
                            .Include(x => x.Dv)
                                .ThenInclude(x => x.Payee)
                            .Include(x => x.indexDeductions)
                                .ThenInclude(x => x.Deduction)
                            .Include(x=>x.BillNumbers)
                            select c;

            bool check = indexData.Any(a => a == null);

            if (!String.IsNullOrEmpty(searchString))
            {
                indexData = indexData.Where(x => x.Category.CategoryDescription.Contains(searchString) || x.Dv.DvNo.Contains(searchString) || x.Dv.PayeeDesc.Contains(searchString));
            }

            ViewBag.indexCategory = indexData.Where(x => x.Category.CategoryDescription.Contains(searchString));
            ViewBag.indexDv = indexData;
            ViewBag.indexPayee = indexData.Where(x => x.Dv.PayeeDesc.Contains(searchString));
            ViewBag.searchString = searchString;

            //no filter
            var grossAmountTotal = _MyDbContext.Indexofpayment.Sum(x => x.GrossAmount);
            ViewBag.grossTotal = grossAmountTotal;
            var totalDeductionTotal = _MyDbContext.Indexofpayment.Sum(x => x.TotalDeduction);
            ViewBag.totalDeductionTotal = totalDeductionTotal;
            var netAmountTotal = _MyDbContext.Indexofpayment.Sum(x => x.NetAmount);
            ViewBag.netTotal = netAmountTotal;

            //with filter
            var grossAmount = _MyDbContext.Indexofpayment.Where(x=>x.Category.CategoryDescription == searchString || x.Dv.DvNo == searchString || x.Dv.PayeeDesc == searchString).Sum(x => x.GrossAmount);
            ViewBag.gross = grossAmount;
            var totalDeduction = _MyDbContext.Indexofpayment.Where(x => x.Category.CategoryDescription == searchString || x.Dv.DvNo == searchString || x.Dv.PayeeDesc == searchString).Sum(x => x.TotalDeduction);
            ViewBag.totalDeduction = totalDeduction;
            var netAmount = _MyDbContext.Indexofpayment.Where(x => x.Category.CategoryDescription == searchString || x.Dv.DvNo == searchString || x.Dv.PayeeDesc == searchString).Sum(x => x.NetAmount);
            ViewBag.net = netAmount;


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

            indexOfPayment.allotmentClassType = indexOfPayment.allotmentClassType;
            indexOfPayment.fundSource = indexOfPayment.fundSource;
            indexOfPayment.orsNo = indexOfPayment.orsNo;
            indexOfPayment.ObligationId = indexOfPayment.ObligationId;

            if (ModelState.IsValid)
            {
                indexOfPayment.indexDeductions = indexOfPayment.indexDeductions.Where(x => x.DeductionId != 0 && x.Amount != 0).ToList();
                _MyDbContext.Add(indexOfPayment);
                await Task.Delay(500);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(indexOfPayment);
        }

        public IActionResult GetOrs(int cid)
        {

            var ors_List = (from fundsource in _MyDbContext.FundSources
                              join obligation in _MyDbContext.Obligation
                              on fundsource.FundSourceId equals obligation.FundSourceId
                              join allotmentclass in _MyDbContext.AllotmentClass
                              on fundsource.AllotmentClassId equals allotmentclass.Id
                              join fund in _MyDbContext.Fund
                              on fundsource.FundId equals fund.FundId
                              where fundsource.AllotmentClassId == cid
                              select new
                              {
                                  allotment = allotmentclass.Fund_Code,
                                  fundCurrent = fund.Fund_code_current,
                                  fundConap = fund.Fund_code_conap,
                                  fundsource = fundsource.AppropriationId,
                                  obligation = obligation.source_type,
                                  Id = obligation.Id,
                                  Name = allotmentclass.Fund_Code + "-" + fund.Fund_code_current + "-" + obligation.Date.ToString("yyyy-MM") + "-" + "000" + obligation.Id
                              }).ToList();


            return Json(ors_List);

        }

        public IActionResult CheckifExist(int CategoryId, string poNumber)
        {
            var data = _MyDbContext.Indexofpayment.Where(x=>x.PoNumber == poNumber && x.CategoryId == CategoryId).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckInvoiceExist(string invoice)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.InvoiceNumber == invoice).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckPeriodExist(string periodCover)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.PeriodCover == periodCover).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckProjectExist(int project)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.ProjectId == project).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckFromToExist(string fromTo)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.date == fromTo).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckSoExist(int so)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.SoNumber == so).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult CheckAccNoExist(string accNo)
        {
            var data = _MyDbContext.Indexofpayment.Where(x => x.AccountNumber == accNo).SingleOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public IActionResult CheckBillNumberExist(int billnumber, int IndexOfPaymentId)
        {
            var data = _MyDbContext.BillNumber.Include(x=>x.IndexOfPayment).Where(x => x.NumberOfBilling == billnumber && x.IndexOfPaymentId == IndexOfPaymentId || x.IndexOfPayment.NumberOfBill == billnumber).FirstOrDefault();

            if (data != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        // GET: Categoty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine(id);
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "");
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();
            if (id == null)
            {
                return NotFound();
            }

            IndexOfPayment index = await _MyDbContext.Indexofpayment
                .Include(x => x.indexDeductions).ThenInclude(x => x.Deduction)
                .Include(x => x.Category)
                .Include(x => x.Dv)
                .Include(x=>x.BillNumbers)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IndexOfPaymentId == id);

            PopulateCategoryDropDownList(index.CategoryId);
            PopulateallotmentClassTypeList();

            var deductionArr = new List<IndexDeduction>(index.indexDeductions.AsEnumerable());
            for (int x = 0; x < 7 - index.indexDeductions.Count; x++)
            {
                deductionArr.Add(new IndexDeduction());
            }

            index.indexDeductions = deductionArr;

            if (index == null)
            {
                return NotFound();
            }
            /*PopulateAssignedIndexDeductionData(Index);*/
            return View(index);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IndexOfPayment index)
        {
            var indexes = await _MyDbContext.Indexofpayment
                .Include(x=>x.Dv)
                .Include(x=>x.BillNumbers)
                .Where(x => x.IndexOfPaymentId == index.IndexOfPaymentId)
                .FirstOrDefaultAsync();

            indexes.CategoryId = index.CategoryId == 0 ? indexes.CategoryId : index.CategoryId;
            indexes.DvId = index.DvId;
            indexes.DvDate = index.DvDate;
            indexes.Particulars = index.Particulars;
            indexes.PoNumber = index.PoNumber;
            indexes.InvoiceNumber = index.InvoiceNumber;
            indexes.ProjectId = index.ProjectId;
            indexes.NumberOfBill = index.NumberOfBill;            
            indexes.PeriodCover = index.PeriodCover;            
            indexes.date = index.date;
            indexes.travel_period = index.travel_period;
            indexes.SoNumber = index.SoNumber;
            indexes.AccountNumber = index.AccountNumber;
            indexes.GrossAmount = index.GrossAmount;
            indexes.indexDeductions = index.indexDeductions.Where(x => x.DeductionId != null).ToList();
            indexes.TotalDeduction = index.TotalDeduction;
            indexes.NetAmount = index.GrossAmount - index.indexDeductions.Sum(x => x.Amount);
            indexes.ObligationId = index.ObligationId;
            indexes.allotmentClassType = index.allotmentClassType;

            var newBillNumber = new BillNumber
                {
                    IndexOfPaymentId = index.IndexOfPaymentId,
                    NumberOfBilling = index.NumberOfBill
                };
            indexes.BillNumbers.Add(newBillNumber);

            PopulateCategoryDropDownList();
            PopulateDvDropDownList();
            PopulateDeductionDropDownList();
            PopulateOrsNoDownList();
           


            _MyDbContext.Update(indexes);
            await Task.Delay(500);
            await _MyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedIndexDeductionData(IndexOfPayment index)
        {
            var allDeduction = _MyDbContext.IndexDeduction;
            var indexDeduction = new HashSet<int>(index.indexDeductions.Select(c => c.IndexDeductionId));
            var viewModel = new List<IndexDeduction>();
            foreach (var deduction in allDeduction)
            {
                viewModel.Add(new IndexDeduction
                {
                    DeductionId = deduction.DeductionId,
                    Amount = deduction.Amount
                });
            }
            ViewBag.Deductions = viewModel;
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
            await Task.Delay(500);
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
            var Query = from d in _MyDbContext?.Dv
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

        private void PopulateOrsNoDownList(object selected = null)
        {
            var Query = from d in _MyDbContext.Indexofpayment
                        orderby d.ObligationId
                        select d;
            ViewBag.ObligationId = new SelectList(Query, "IndexOfPaymentId", "ObligationId", selected);
        }

        private void PopulateallotmentClassTypeList(object selected = null)
        {
            var Query = from d in _MyDbContext.Indexofpayment
                        orderby d.allotmentClassType
                        select d;
            ViewBag.AllotmentClassType = new SelectList(Query, "IndexOfPaymentId", "allotmentClassType", selected);
        }



        public IActionResult Export(string searchString)
        {
            DataTable dt = new DataTable("Index of Payment");
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

                var subTotalDeduction = _MyDbContext.IndexDeduction.Where(x => x.IndexOfPayment.Dv.DvNo == searchString).Sum(x => x.Amount);
                var totalGross = _MyDbContext.Indexofpayment.Where(x => x.Dv.DvNo == searchString).Sum(x => x.GrossAmount);
                var totalDeduction = _MyDbContext.Indexofpayment.Where(x => x.Dv.DvNo == searchString).Sum(x => x.TotalDeduction);
                var totalnet = _MyDbContext.Indexofpayment.Where(x => x.Dv.DvNo == searchString).Sum(x => x.NetAmount);


                ws.Cell("A1").Style.Font.FontSize = 10;
                ws.Cell("A1").Style.Font.FontName = "Calibri Light";
                ws.Cell("A1").Value = "Category";
                ws.Cell("A1").Style.Font.SetBold();
                ws.Columns(1, 2).Width = 15;
                ws.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("B1").Style.Font.FontSize = 10;
                ws.Cell("B1").Style.Font.FontName = "Calibri Light";
                ws.Cell("B1").Value = "Dv #";
                ws.Cell("B1").Style.Font.SetBold();
                ws.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("C1").Style.Font.FontSize = 10;
                ws.Cell("C1").Style.Font.FontName = "Calibri Light";
                ws.Cell("C1").Value = "Payee";
                ws.Cell("C1").Style.Font.SetBold();
                ws.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("D1").Style.Font.FontSize = 10;
                ws.Cell("D1").Style.Font.FontName = "Calibri Light";
                ws.Cell("D1").Value = "Date";
                ws.Cell("D1").Style.Font.SetBold();
                ws.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("E1").Style.Font.FontSize = 10;
                ws.Cell("E1").Style.Font.FontName = "Calibri Light";
                ws.Cell("E1").Value = "Particulars";
                ws.Cell("E1").Style.Font.SetBold();
                ws.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("F1").Style.Font.FontSize = 10;
                ws.Cell("F1").Style.Font.FontName = "Calibri Light";
                ws.Cell("F1").Value = "PO #";
                ws.Cell("F1").Style.Font.SetBold();
                ws.Cell("F1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("G1").Style.Font.FontSize = 10;
                ws.Cell("G1").Style.Font.FontName = "Calibri Light";
                ws.Cell("G1").Value = "Invoice #";
                ws.Cell("G1").Style.Font.SetBold();
                ws.Cell("G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("H1").Style.Font.FontSize = 10;
                ws.Cell("H1").Style.Font.FontName = "Calibri Light";
                ws.Cell("H1").Value = "Project Id";
                ws.Cell("H1").Style.Font.SetBold();
                ws.Cell("H1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("I1").Style.Font.FontSize = 10;
                ws.Cell("I1").Style.Font.FontName = "Calibri Light";
                ws.Cell("I1").Value = "# of Billing";
                ws.Cell("I1").Style.Font.SetBold();
                ws.Cell("I1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("J1").Style.Font.FontSize = 10;
                ws.Cell("J1").Style.Font.FontName = "Calibri Light";
                ws.Cell("J1").Value = "Period Covered";
                ws.Cell("J1").Style.Font.SetBold();
                ws.Cell("J1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("K1").Style.Font.FontSize = 10;
                ws.Cell("K1").Style.Font.FontName = "Calibri Light";
                ws.Cell("K1").Value = "From-To";
                ws.Cell("K1").Style.Font.SetBold();
                ws.Cell("K1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("L1").Style.Font.FontSize = 10;
                ws.Cell("L1").Style.Font.FontName = "Calibri Light";
                ws.Cell("L1").Value = "Travel Period";
                ws.Cell("L1").Style.Font.SetBold();
                ws.Cell("L1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("M1").Style.Font.FontSize = 10;
                ws.Cell("M1").Style.Font.FontName = "Calibri Light";
                ws.Cell("M1").Value = "SO #";
                ws.Cell("M1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("N1").Style.Font.FontSize = 10;
                ws.Cell("N1").Style.Font.FontName = "Calibri Light";
                ws.Cell("N1").Value = "Account #";
                ws.Cell("N1").Style.Font.SetBold();
                ws.Cell("N1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("O1").Style.Font.FontSize = 10;
                ws.Cell("O1").Style.Font.FontName = "Calibri Light";
                ws.Cell("O1").Value = "Deduction";
                ws.Cell("O1").Style.Font.SetBold();
                ws.Cell("O1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("P1").Style.Font.FontSize = 10;
                ws.Cell("P1").Style.Font.FontName = "Calibri Light";
                ws.Cell("P1").Value = "Deduction Amount";
                ws.Cell("P1").Style.Font.SetBold();
                ws.Cell("P1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("Q1").Style.Font.FontSize = 10;
                ws.Cell("Q1").Style.Font.FontName = "Calibri Light";
                ws.Cell("Q1").Value = "Gross Amount";
                ws.Cell("Q1").Style.Font.SetBold();
                ws.Cell("Q1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("R1").Style.Font.FontSize = 10;
                ws.Cell("R1").Style.Font.FontName = "Calibri Light";
                ws.Cell("R1").Value = "Total Deductions";
                ws.Cell("R1").Style.Font.SetBold();
                ws.Cell("R1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("S1").Style.Font.FontSize = 10;
                ws.Cell("S1").Style.Font.FontName = "Calibri Light";
                ws.Cell("S1").Value = "Net Amount";
                ws.Cell("S1").Style.Font.SetBold();
                ws.Cell("S1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

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

                    foreach (var deduction in item.indexDeductions)
                    {
                        ws.Cell(currentRow, 15).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 15).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 15).Value = deduction.Deduction.DeductionDescription;
                        ws.Cell(currentRow, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        ws.Cell(currentRow, 16).Style.Font.FontSize = 10;
                        ws.Cell(currentRow, 16).Style.Font.FontName = "Calibri Light";
                        ws.Cell(currentRow, 16).Value = deduction.Amount;
                        ws.Cell(currentRow, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        currentRow++;
                    }

                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 17).Value = item.GrossAmount;
                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 18).Value = item.TotalDeduction;
                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 19).Value = item.NetAmount;
                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    //currentRow++;

                    
                    //currentRow++;
                    ws.Cell(currentRow, 15).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 15).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 15).Value = "Total";
                    ws.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(currentRow, 15).Style.Font.SetBold();
                    ws.Cell(currentRow, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 16).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 16).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 16).Value = subTotalDeduction;
                    ws.Cell(currentRow, 16).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(currentRow, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 17).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 17).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 17).Value = totalGross;
                    ws.Cell(currentRow, 17).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(currentRow, 17).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 18).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 18).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 18).Value = totalDeduction;
                    ws.Cell(currentRow, 18).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(currentRow, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(currentRow, 19).Style.Font.FontSize = 10;
                    ws.Cell(currentRow, 19).Style.Font.FontName = "Calibri Light";
                    ws.Cell(currentRow, 19).Value = totalnet;
                    ws.Cell(currentRow, 19).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(currentRow, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
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