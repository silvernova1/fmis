using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using fmis.Models;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using fmis.Data;


namespace fmis.Controllers
{
    public class UploadSectionController : Controller
    {
        private SectionContext Context { get; }

        public UploadSectionController(SectionContext _context)
        {
            this.Context = _context;
        }
        public IActionResult UploadSection()
        {
            ViewBag.filter_sidebar = "upload_file/division";
            ViewBag.layout = "_Layout";
            return View("~/Views/Budget/Section.cshtml");
        }
        public string checkExcel(string excel_data)
        {
            if (!string.IsNullOrWhiteSpace(excel_data))
                return @excel_data;

            return "";
        }

        public DateTime CheckExcelDate(string excel_data)
        {
            string dateString = @"d/M/yyyy";

            DateTime date3 = DateTime.ParseExact(dateString, @"d/M/yyyy",
            System.Globalization.CultureInfo.InvariantCulture);
            if (dateString == null)
                return DateTime.ParseExact(dateString, @"d/M/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            return (DateTime)date3;


        }


        public DateTime checkExcelDate(string excel_data, ExcelWorksheet worksheet, int row, int col)
        {
            if (!string.IsNullOrWhiteSpace(excel_data) && excel_data != "NULL")
                return DateTime.Parse(worksheet.Cells[row, col].Value.ToString());


            return DateTime.Parse("0001-01-01T00:00:00");
        }

        public int checkExcelInt(string excel_data, ExcelWorksheet worksheet, int row, int col)
        {
            if (!string.IsNullOrWhiteSpace(excel_data) && excel_data != "NULL")
            {
                try
                {
                    return int.Parse(worksheet.Cells[row, col].Value.ToString());
                }
                catch
                {
                    return 0;
                }
            }

            return 0;
        }


        [HttpPost]
        public IActionResult ImportSection()
        {
            IFormFile excelfile = Request.Form.Files[0];
            string sWebRootFolder = Directory.GetCurrentDirectory() + @"\UploadFile";
            if (!Directory.Exists(sWebRootFolder)) Directory.CreateDirectory(sWebRootFolder);
            string sFileName = $"{Guid.NewGuid()}.xlsx";

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var sections = new List<Section>();
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
                for (int row = 2; row <= rowCount; row++)
                {
                    var section = new Section();

                    for (int col = 1; col <= ColCount; col++)
                    {
                        if (col == 1)
                            section.Id = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 2)
                            section.Division = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 3)
                            section.Description = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 4)
                            section.Head = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 5)
                            section.Code = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 6)
                            section.Remember_Token = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 7)
                            section.Created_At = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 8)
                            section.Updated_At = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);

                    }


                    sections.Add(section);

                    if (!string.IsNullOrWhiteSpace(section.Description))
                    {
                        this.Context.Section.Add(section);
                        this.Context.SaveChanges();
                    }

                    //sb.Append(Environment.NewLine);

                }
                //var test = sb.ToString();
                return Json(sections);
                //return Content(sb.ToString());
            }

        }

    }
}
