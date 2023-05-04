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
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using fmis.Models.John;
using fmis.Models.silver;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Rotativa.AspNetCore;
using System.IO;
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

namespace fmis.Controllers.Accounting
{
    [Authorize(Roles = "accounting_admin , accounting_user")]
    public class PayeeController : Controller
    {
        private readonly MyDbContext _MyDbContext;

        public PayeeController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }

        public class PayeeDatas
        {
            public List<PayeeData> Payees { get; set; }
        }

        public class PayeeData
        {
            public int Id { get; set; }
            public string PayeeDescription { get; set; }
            public string TinNo { get; set; }
            public string token { get; set; }
        }

        public class Many
        {
            public string many_token { get; set; }
        }

        public class DeleteData
        {
            public string single_token { get; set; }
            public List<Many> many_token { get; set; }
        }


        public async Task<IActionResult> HrmoPermanent()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "hrmo_permanent");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "A").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/HrmoPermanent.cshtml");
        }

        public async Task<IActionResult> HrmoJobOrder()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "hrmo_job_order");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "B").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/HrmoJobOrder.cshtml");
        }

        public async Task<IActionResult> HrhContractual()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "hrh_contractual");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "C").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/HrhContractual.cshtml");
        }

        public async Task<IActionResult> HrhJobOrder()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "hrh_job_order");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "D").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/HrhJobOrder.cshtml");
        }

        public async Task<IActionResult> Contractors()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "contractors");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "E").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/Contractors.cshtml");
        }


        public async Task<IActionResult> Supplier()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "payee", "supplier");
            ViewBag.layout = "_Layout";
            var json = JsonSerializer.Serialize(await _MyDbContext.Payee.Where(s => s.status == "activated" && s.payee_type == "F").OrderBy(x => x.CreatedAt).ToListAsync());
            ViewBag.temp = json;

            return View("~/Views/Payee/Supplier.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveHrmoPermanent(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "A";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveHrmoJobOrder(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "B";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveHrhContractual(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "C";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveHrhJobOrder(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "D";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveContractors(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "E";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> SaveSupplier(List<PayeeData> data)
        {
            var data_holder = _MyDbContext.Payee.Where(x => x.status == "activated");

            foreach (var item in data)
            {
                var payee = new Payee();
                if (await data_holder.AnyAsync(s => s.token == item.token)) //CHECK IF EXIST
                {
                    payee = await data_holder.Where(s => s.token == item.token).FirstOrDefaultAsync();
                }
                payee.PayeeDescription = item.PayeeDescription.ToUpper();
                payee.TinNo = item.TinNo;
                payee.status = "activated";
                payee.token = item.token;
                payee.payee_type = "F";

                _MyDbContext.Payee.Update(payee);
                await _MyDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        public void setUpDeleteData(string token)
        {
            var payee = new Payee(); //CLEAR OBJECT
            payee = _MyDbContext.Payee.Where(s => s.token == token && s.status != "deactivated").AsNoTracking().FirstOrDefault();
            if (payee is not null)
            {
                payee.status = "deactivated";
                _MyDbContext.Update(payee);
                _MyDbContext.SaveChanges();
            }
        }


        // POST: Obligations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePayees(DeleteData data)
        {

            foreach (var many in data.many_token)
                setUpDeleteData(many.many_token);

            return Json(data);
        }

        public IActionResult UploadPayee()
        {
            ViewBag.filter = new FilterSidebar("Accounting", "UploadPayee", "");
            return View("~/Views/Payee/UploadPayee.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> ImportPayee()
        {
            IFormFile excelfile = Request.Form.Files[0];
            string sWebRootFolder = Directory.GetCurrentDirectory() + @"\UploadFile";
            if (!Directory.Exists(sWebRootFolder)) Directory.CreateDirectory(sWebRootFolder);
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            Stopwatch timer = new();
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
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
                List<Payee> payee = new();



                timer.Start();
                for (int row = 1; row <= rowCount; row++)
                {
                    string payeeToken = Guid.NewGuid().ToString();
                    var payee_Type = worksheet.Cells[row, 3].Text;

                    string payeeDesc = worksheet.Cells[row, 1].Text;
                    char[] chartoTrim = { ' ', '\t' };
                    string trimmedPayeeDesc = payeeDesc.TrimEnd(chartoTrim);

                    payee.Add(new Payee()
                    {
                        PayeeDescription = trimmedPayeeDesc,
                        TinNo = worksheet.Cells[row, 2].Text,
                        payee_type = payee_Type,
                        token = payeeToken,
                        status = "activated",

                    });

                    //if (row == 1000) break;
                }
                timer.Stop();

                Console.WriteLine("ellapsed: " + timer.ElapsedMilliseconds + "ms");
                await _MyDbContext.AddRangeAsync(payee);
                var water = await _MyDbContext.SaveChangesAsync();
                Console.WriteLine(water);
                var test = sb.ToString();
                return Ok();
                //return Content(sb.ToString());
            }
        }



    }
}
