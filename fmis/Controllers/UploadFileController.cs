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

namespace fmis.Controllers
{
    public class UploadFileController : Controller
    {
        private IHostEnvironment _hostingEnvironment;

        public UploadFileController(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

     /*   #region TEST MODEL
        public class Yoh
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }
        #endregion*/

        public IActionResult UploadFile()
        {
            return View("~/Views/Budget/UploadFile.cshtml");
        }

        public string checkExcel(string excel_data) {
            if (!string.IsNullOrWhiteSpace(excel_data))
                return excel_data;

            return "";
        }

        public DateTime CheckExcelDate(string excel_data)
        {
            string dateString = @"20/05/2012";

            DateTime date3 = DateTime.ParseExact(dateString, @"d/M/yyyy",
            System.Globalization.CultureInfo.InvariantCulture);
            if (dateString == null)
                return DateTime.ParseExact(dateString, @"d/M/yyyy",
            System.Globalization.CultureInfo.InvariantCulture);

            return (DateTime)date3;
        }


       /* public DateTime CheckExcelDate(string excel_data)
        {
            DateTime date3;
            string dateString = @"20/05/2012";

            DateTime date3 = DateTime.ParseExact(dateString, @"d / M / yyyy",
            System.Globalization.CultureInfo.InvariantCulture);
            if (dateString == null)
                return DateTime.ParseExact(dateString, @"d / M / yyyy",
            System.Globalization.CultureInfo.InvariantCulture);

            return (DateTime)date3;
        }*/

        public DateTime checkExcelDate(string excel_data)
        {
            if (!string.IsNullOrWhiteSpace(excel_data))
                return DateTime.Parse(excel_data);

            return DateTime.Parse("0001-01-01T00:00:00");
        }

      
        [HttpPost]
        public IActionResult Import()
        {
            IFormFile excelfile = Request.Form.Files[0];
            string sWebRootFolder = Directory.GetCurrentDirectory() + @"\UploadFile";
            if (!Directory.Exists(sWebRootFolder)) Directory.CreateDirectory(sWebRootFolder);
                string sFileName = $"{Guid.NewGuid()}.xlsx";
            
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
                for (int row = 2; row <= rowCount; row++)
                {
                    var personal_information = new Personal_Information();
                    for (int col = 1; col <= ColCount; col++)
                    {
                        if (col == 1)
                            personal_information.id = int.Parse(worksheet.Cells[row, col].Value.ToString());
                        else if (col == 2)
                            personal_information.userid = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 3)
                            personal_information.picture = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 4)
                            personal_information.signature = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 5)
                            personal_information.fname = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 6)
                            personal_information.lname = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 7)
                            personal_information.mname = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 8)
                            personal_information.name_ext = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 10)
                            personal_information.date_of_birth = CheckExcelDate(worksheet.Cells[row, col].Text as string);

                    }
                    personal_informations.Add(personal_information);
                    sb.Append(Environment.NewLine);

                
                }
                //var test = sb.ToString();
                return Json(personal_informations);
                //return Content(sb.ToString());
            }
            
        }

    }

    

}
