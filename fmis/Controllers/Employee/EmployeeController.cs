using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using fmis.Filters;
using fmis.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using fmis.Data.MySql;
using fmis.ViewModel;
using System;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections.Generic;
using fmis.Models.ppmp;
using Microsoft.EntityFrameworkCore;
using fmis.Models.pr;

namespace fmis.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly MyDbContext _context;

        public EmployeeController(PpmpContext ppmpContext, DtsContext dts, MyDbContext context)
        {
            _ppmpContext = ppmpContext;
            _dts = dts;
            _context = context;
        }

        public IActionResult Index()
        {
            var expenses = _ppmpContext.expense.Include(x=>x.Items).ToList();

            var items = expenses.Select(x=> new { 
                ExpensesDescription = x.Description,
                ItemDescription = x.Items.Where(x=>x.Yearly_ref_id == 3).Select(x=>x.Description.ToList())
            });

            return View();
        }

        [HttpPost]
        public IActionResult GenerateApp()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.LEGAL.Rotate());

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                document.Add(new Paragraph("Department of Health-Central Visayas-Center for Health Development Revised Annual Procurement Plan for FY 2023 \n\n"));

                PdfPTable table = new PdfPTable(15);
                table.WidthPercentage = 100;

                string[] headers = new string[]
                {
                "Code", "Procurement Project", "PMO/End User", "Yes", "No",
                "Mode of Procurement", "Advertising/Posting IB/REI", "Submission/Opening of Bids", "Notice of Award", "Contract Signing",
                "Source of Funds", "Total", "MOOE", "CO", "Remarks (Brief Description of the Project)"
                };
                int[] mergedColumns = new int[] { 0, 1, 2, 3, 4, 5 };

                Font headerFont = new Font(Font.FontFamily.COURIER, 9, Font.BOLD, BaseColor.BLACK);
                Font cellFont = new Font(Font.FontFamily.COURIER, 9, Font.NORMAL, BaseColor.BLACK);

                float[] columnWidths = new float[] { 35f, 250f, 60f, 40f, 40f, 80f, 80f, 75f, 70f, 70f, 70f, 80f, 80f, 80f, 150f };
                table.SetWidths(columnWidths);

                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerCell.FixedHeight = 80f;
                    table.AddCell(headerCell);
                }

                var items = _ppmpContext.item.ToList();

                for (int row = 0; row < items.Count; row++)
                {
                    string itemDescription = items[row].Description;

                    for (int col = 0; col < headers.Length; col++)
                    {
                        PdfPCell cell;

                        if (col == 1)
                        {
                            cell = new PdfPCell(new Phrase(itemDescription, cellFont));
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase($"Row {row + 1}, Cell {col + 1}", cellFont));
                        }

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.BorderWidthLeft = 1f;
                        cell.BorderWidthRight = col == headers.Length - 1 ? 1f : 0f;
                        cell.BorderWidthTop = 0f;
                        cell.BorderWidthBottom = 0f;

                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                document.Close();

                Response.Headers.Add("Content-Disposition", "inline; filename=sample.pdf");
                Response.ContentType = "application/pdf";

                Response.Body.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
            }

            return new EmptyResult();
        }

        [HttpGet]
        public IActionResult Pr()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var item = from x in _ppmpContext.item
                       select x;
            ViewBag.Item = new SelectList(item, "Id", "Description");

            var employee = from x in _dts.users
                           orderby x.Id
                           select new
                           {
                               Id = x.Id,
                               Fullname = x.Fname + " " + x.Lname + " " + x.Lname
                           };
            ViewBag.Approval = new SelectList(employee, "Id", "Fullname");


            var viewModel = GetCascadingDropdownData();
            if (viewModel.Expenses == null)
            {
                // If the Countries collection is null, initialize it to an empty list
                viewModel.Expenses = new List<Expense>();
            }
            if (viewModel.Items == null)
            {
                // If the Countries collection is null, initialize it to an empty list
                viewModel.Items = new List<Item>();
            }

            var pr = _ppmpContext.expense.Include(x=>x.Items).ToList();

            ExpenseDropDownList();
            ItemsDropDownList();


            return View();
        }
        [HttpPost]
        public IActionResult Pr(Pr pr)
        {
            _context.Add(pr);
            _context.SaveChanges();
            return View();
        }

        public PrViewModel GetCascadingDropdownData()
        {
            var viewModel = new PrViewModel
            {
                Expenses = _ppmpContext.expense.ToList()
            };

            return viewModel;
        }

        public List<Item> GetItemsId(int expenseId)
        {
            return _ppmpContext.item.Where(c => c.Expense_id == expenseId).ToList();
        }

        [HttpGet]
        public IActionResult GetUnit(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_measurement);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult GetUnitCost(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_cost);
            }
            return NotFound();
        }



        public IActionResult GeneratePdf()
        {
            var pdfGenerator = new PdfGenerator();
            var fileName = "my_generated_pdf.pdf";
            var relativePath = Path.Combine("PdfFiles", fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            pdfGenerator.GeneratePdf(filePath);

            return Content("PDF generated successfully!");
        }

        public void ExpenseDropDownList(object selected = null)
        {
            var Query = from e in _ppmpContext.expense
                        orderby e.Id
                        select e;
            ViewBag.ExpenseId = new SelectList(Query, "Id", "Description", selected);
        }
        public void ItemsDropDownList(object selected = null)
        {
            var Query = from i in _ppmpContext.item
                        orderby i.Id
                        select i;
            ViewBag.ItemId = new SelectList(Query, "Id", "Description", selected);
        }

    }
}
