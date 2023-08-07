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

namespace fmis.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;

        public EmployeeController(PpmpContext ppmpContext, DtsContext dts)
        {
            _ppmpContext = ppmpContext;
            _dts = dts;
        }

        public IActionResult Pr()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var item = from x in _ppmpContext.item
                       orderby x.Id
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

            return View(viewModel);
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

        /*[HttpGet]
        public IActionResult LoadItem(int expenseId)
        {
            var childOptions = _ppmpContext.item
                .Where(x => x.Expense_id == 1)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Description })
                .ToList();

            Console.WriteLine(childOptions);

            return Json(childOptions);
        }*/

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


    }
}
