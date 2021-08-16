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
    public class UploadFileController : Controller
    {
        private PersonalInformationContext Context { get; }

        public UploadFileController(PersonalInformationContext _context)
        {
            this.Context = _context;
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

        public Int16 checkExcelInt(string excel_data, ExcelWorksheet worksheet, int row, int col)
        {
            if (!string.IsNullOrWhiteSpace(excel_data) && excel_data != "NULL")
            {
                try
                {
                    return Int16.Parse(worksheet.Cells[row, col].Value.ToString());
                }
                catch
                {
                    return 0;
                }
            }

            return 0;
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
                            personal_information.id = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
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
                            personal_information.date_of_birth = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 11)
                            personal_information.place_of_birth = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 12)
                            personal_information.sex = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 13)
                            personal_information.civil_status = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 14)
                            personal_information.citizenship = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 15)
                            personal_information.indicate_country = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 16)
                            personal_information.height = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 17)
                            personal_information.weight = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 18)
                            personal_information.blood_type = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 19)
                            personal_information.gsis_idno = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 20)
                            personal_information.gsis_polnno = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 21)
                            personal_information.pagibig_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 22)
                            personal_information.phic_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 23)
                            personal_information.sss_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 24)
                            personal_information.tin_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 25)
                            personal_information.residential_address = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 26)
                            personal_information.residential_municipality = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 27)
                            personal_information.residential_province = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 28)
                            personal_information.RHouse_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 29)
                            personal_information.RStreet = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 30)
                            personal_information.RSubdivision = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 31)
                            personal_information.RBarangay = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 32)
                            personal_information.RMunicipality = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 33)
                            personal_information.RProvince = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 34)
                            personal_information.Phouse_no = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 35)
                            personal_information.PStreet = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 36)
                            personal_information.PSubdivision = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 37)
                            personal_information.PBarangay = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 38)
                            personal_information.PMunicipality = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 39)
                            personal_information.PProvince = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 40)
                            personal_information.RZip_code = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 41)
                            personal_information.PZip_code = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 42)
                            personal_information.region_zip = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 43)
                            personal_information.telno = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 44)
                            personal_information.emall_address = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 45)
                            personal_information.cellno = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 46)
                            personal_information.employee_status = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 47)
                            personal_information.job_status = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 48)
                            personal_information.inactive_area = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 49)
                            personal_information.case_name = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 50)
                            personal_information.case_address = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 51)
                            personal_information.case_contact = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 52)
                            personal_information.designation_id = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 53)
                            personal_information.division_id = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 54)
                            personal_information.section_id = checkExcelInt(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 56)
                            personal_information.disbursement_type = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 57)
                            personal_information.salary_charge = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 58)
                            personal_information.bbalance_cto = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 59)
                            personal_information.vacation_balance = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 60)
                            personal_information.sick_balance = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 61)
                            personal_information.sched = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 63)
                            personal_information.account_number = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 64)
                            personal_information.region = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 65)
                            personal_information.field_status = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 69)
                            personal_information.Rsitio = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 70)
                            personal_information.resigned_effectivity = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 71)
                            personal_information.Rsitio = checkExcel(worksheet.Cells[row, col].Text as string);
                        else if (col == 67)
                            personal_information.created_at = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);
                        else if (col == 68)
                            personal_information.updated_at = checkExcelDate(worksheet.Cells[row, col].Text as string, worksheet, row, col);

                    }


                    personal_informations.Add(personal_information);

                    if (!string.IsNullOrWhiteSpace(personal_information.userid))
                    {
                        this.Context.Personal_Information.Add(personal_information);
                        this.Context.SaveChanges();
                    }

                    

                    //sb.Append(Environment.NewLine);

                }
                //var test = sb.ToString();
                return Json(personal_informations);
                //return Content(sb.ToString());
            }

        }

    }
}