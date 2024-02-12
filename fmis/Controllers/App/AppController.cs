﻿using DocumentFormat.OpenXml.Spreadsheet;
using fmis.Data;
using fmis.Data.MySql;
using fmis.Filters;
using fmis.Models.Accounting;
using fmis.Models.ppmp;
using fmis.Models.UserModels;
using fmis.Services;
using fmis.ViewModel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Item = fmis.Models.ppmp.Item;
using Font = iTextSharp.text.Font;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace fmis.Controllers.App
{
    
    public class AppController : Controller
    {
        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AppController> _logger;

        public AppController(MyDbContext context, IUserService userService, PpmpContext ppmpContext, DtsContext dts, IHttpContextAccessor httpContextAccessor, ILogger<AppController> logger)
        {
            _context = context;
            _userService = userService;
            _ppmpContext = ppmpContext;
            _dts = dts;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IActionResult GetItems()
        {
            return Json(_dts.section.ToList());
        }

        [Authorize(AuthenticationSchemes = "Scheme3", Roles = "app_admin")]
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var appExpenses = new List<AppExpense>();
            if (_context.AppExpense.Count() > 0)
            {
                appExpenses = _context.AppExpense
                    .Where(appExpense => appExpense.AppModels.Any())
                    .Include(x => x.AppModels)
                    .OrderBy(x => x.Uacs)
                    .ToList();

                return PartialView("_AppPartialView", appExpenses);
            }
            else
            {
                var expenses = new List<Expense>();
                expenses = _ppmpContext.expense
                    .Where(expenses => expenses.Items.Any())
                    .Include(x => x.Items.Where(x => x.Yearly_ref_id == int.Parse(YearId) && x.Status == null))
                        .ThenInclude(x => x.Item_Daily)
                    .OrderBy(x => x.Uacs)
                    .ToList();

                ViewBag.section = _dts.section.ToList();

                return PartialView("_ExpensePartialView", expenses);
            }
        }

        //Update Controller method
        [HttpPost]
        public async Task<IActionResult> SaveApp(List<AppExpense> AppExpenses)
        {
            if (ModelState.IsValid)
            {
                foreach (var appExpense in AppExpenses)
                {
                    var expense = appExpense.Description;

                    switch (expense)
                    {
                        case "XLVIII. RENT - ICT  EQUIPMENT":
                            appExpense.Uacs = "5029905008";
                            break;
                        case "IX. SEMI-EXPENDABLE - DISASTER RESPONSE AND RESCUE EQUIPMENT":
                            appExpense.Uacs = "5020321008";
                            break;
                        case "LIII. OTHER MAINTENANCE AND OPERATING EXPENSES":
                            appExpense.Uacs = "5029999099";
                            break;
                        case "XXIX. REPAIR MAINTENANCE - ICT EQUIPMENT":
                            appExpense.Uacs = "5021305003";
                            break;
                        case "XXIV. CONSULTANCY SERVICES (One Line - Amount Only - for the Whole Division)":
                            appExpense.Uacs = "5021103002";
                            break;
                        case "XXXVII. REPAIR MAINTENANCE - SEMI-EXPENDABLE - ICT EQUIPMENT":
                            appExpense.Uacs = "5021321003";
                            break;
                        case "XXXV. REPAIR MAINTENANCE : OTHER PROPERTY, PLANT AND EQUIPMENT":
                            appExpense.Uacs = "5021399099";
                            break;
                        case "XLII.  ADVERTISING EXPENSES":
                            appExpense.Uacs = "5029901000";
                            break;
                        case "XLIX. RENT - ICT MACHINERY AND EQUIPMENT":
                            appExpense.Uacs = "5029905008";
                            break;
                        default:
                            appExpense.Uacs = null;
                            break;
                    }


                    if (appExpense?.AppModels != null)
                    {

                        foreach (var appModel in appExpense.AppModels)
                        {
                            if (appModel != null)
                            {
                                appExpense.Uacs = appModel.Uacs;
                            }
                        }
                    }
                    _context.AppExpense.Add(appExpense);
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppExpense(List<AppExpense> appExpenses)
        {
            foreach (var appExpense in appExpenses)
            {
                var existingAppExpense = _context.AppExpense.Find(appExpense.Id);

                if (existingAppExpense == null)
                {
                    continue;
                }

                existingAppExpense.Description = appExpense.Description;

                if (appExpense.AppModels != null)
                {
                    foreach (var appModel in appExpense.AppModels)
                    {
                        var existingAppModel = _context.AppModel.Find(appModel.Id);

                        if (existingAppModel != null)
                        {
                            existingAppModel.Uacs = appModel.Uacs;
                            existingAppModel.ProcurementProject = appModel.ProcurementProject;
                            existingAppModel.EndUser = appModel.EndUser;
                            existingAppModel.EarlyProcured = appModel.EarlyProcured;
                            existingAppModel.NotEarlyProcurered = appModel.NotEarlyProcurered;
                            existingAppModel.ModeOfProcurement = appModel.ModeOfProcurement;
                            existingAppModel.Advertising = appModel.Advertising;
                            existingAppModel.Submission = appModel.Submission;
                            existingAppModel.NoticeOfAward = appModel.NoticeOfAward;
                            existingAppModel.ContractSigning = appModel.ContractSigning;
                            existingAppModel.FundSource = appModel.FundSource;
                            existingAppModel.Total = appModel.Total;
                            existingAppModel.Mooe = appModel.Mooe;
                            existingAppModel.Co = appModel.Co;
                            existingAppModel.Remarks = appModel.Remarks;

                            _context.Update(existingAppModel); // Mark AppModel as modified
                        }
                    }
                }

                _context.Update(existingAppExpense); // Mark AppExpense as modified
            }

            await _context.SaveChangesAsync();

            /*if (appExpenses != null)
            {
                foreach (var updatedExpense in appExpenses)
                {
                    var existingExpense = _context.AppExpense.FirstOrDefault(x => x.Id == updatedExpense.Id);

                    if (existingExpense != null)
                    {
                        existingExpense.Description = updatedExpense.Description;

                        if (updatedExpense.AppModels != null)
                        {
                            if (existingExpense.AppModels == null)
                            {
                                existingExpense.AppModels = new List<AppModel>();
                            }
                            var appModelsToRemove = existingExpense.AppModels
                                .Where(existingAppModel => !updatedExpense.AppModels.Any(newAppModel => newAppModel.Id == existingAppModel.Id))
                                .ToList();

                            foreach (var appModelToRemove in appModelsToRemove)
                            {
                                _context.AppModel.Remove(appModelToRemove);
                            }
                            foreach (var appModel in updatedExpense.AppModels)
                            {
                                var existingAppModel = existingExpense.AppModels.FirstOrDefault(x => x.Id == appModel.Id);

                                if (existingAppModel != null)
                                {
                                    existingAppModel.Mooe = appModel.Mooe;
                                }
                                else
                                {
                                    existingExpense.AppModels.Add(appModel);
                                }
                            }
                        }

                        _context.Update(existingExpense);
                    }
                }

                await _context.SaveChangesAsync();
            }*/

            return RedirectToAction("Index");
        }

        #region APP REPORTS

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Scheme3", Roles = "app_admin")]
        public async Task<IActionResult> GenerateApp()
        {

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.LEGAL.Rotate());

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                string year = User.FindFirstValue("YearlyRef");

                document.Add(new Paragraph("Department of Health-Central Visayas-Center for Health Development Revised Annual Procurement Plan for FY " + year + "  \n\n"));

                Font headerFont = new Font(Font.FontFamily.COURIER, 9, Font.BOLD, BaseColor.BLACK);
                Font cellFont = new Font(Font.FontFamily.COURIER, 9, Font.NORMAL, BaseColor.BLACK);

                PdfPTable table = new PdfPTable(15);
                table.WidthPercentage = 100;

                float[] columnWidths = new float[] { 90f, 250f, 90f, 60f, 40f, 80f, 90f, 90f, 90f, 90f, 70f, 130f, 130f, 80f, 80f };
                table.SetWidths(columnWidths);


                PdfPCell cell = new PdfPCell(new Phrase("Code", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Procurement Project", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("PMO/End User", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Early Procurement", headerFont));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Mode of Procurement", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Schedule for Each Procurement Activity", headerFont));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Source of Funds", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Estimated Budget (PhP)", headerFont));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Remarks (Brief description of the Project", headerFont));
                cell.Rowspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                // Row 2
                cell = new PdfPCell(new Phrase("Yes", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("No", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);


                cell = new PdfPCell(new Phrase("Advertising/Posting IB/REI", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Submission/ Opening of Bids", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Notice of Award", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Contract Signing", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Total", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("MOOE", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("CO", headerFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);


                var expenses = _context.AppExpense
                    .Where(expenses => expenses.AppModels.Any())
                    .Include(x=>x.AppModels).OrderBy(x => x.Uacs)
                    .ToList();
                var items = _context.AppModel.OrderBy(x => x.Uacs).ToList();
                decimal total = 0;


                foreach (var expense in expenses)
                {
                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(expense.Description, headerFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    foreach (var item in expense.AppModels)
                    {
                        cell = new PdfPCell(new Phrase(item.Uacs, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.ProcurementProject, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.EndUser, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.EarlyProcured, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.NotEarlyProcurered, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.ModeOfProcurement, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Advertising?.ToString("MM-dd-yyyy"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Submission?.ToString("MM-dd-yyyy"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.NoticeOfAward?.ToString("MM-dd-yyyy"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.ContractSigning?.ToString("MM-dd-yyyy"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.FundSource, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Total.ToString("##,#00.00"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Mooe.ToString("##,#00.00"), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Co.ToString(), cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Remarks, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        total += item.Total;
                    }
                }


                PdfPTable footer = new PdfPTable(15);
                footer.WidthPercentage = 100;

                float[] footerWidths = new float[] { 90f, 250f, 90f, 60f, 40f, 80f, 90f, 90f, 90f, 90f, 70f, 130f, 130f, 80f, 80f };
                footer.SetWidths(footerWidths);

                for (int i = 1; i <= 12; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    footer.AddCell(footerCell);

                    if (i == 1)
                    {
                        footerCell = new PdfPCell(new Phrase("GRAND TOTAL", cellFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                    if (i == 10)
                    {
                        footerCell = new PdfPCell(new Phrase(@total.ToString("##,#00.00"), cellFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                    if (i == 10)
                    {
                        footerCell = new PdfPCell(new Phrase(@total.ToString("##,#00.00"), cellFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                for (int i = 1; i <= 15; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" "));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 15)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                //2nd Row


                for (int i = 1; i <= 9; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" "));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 9)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }


                    if (i == 1)
                    {
                        footerCell = new PdfPCell(new Phrase("Prepared by:", cellFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if(i == 3)
                    {
                        footerCell = new PdfPCell(new Phrase("Recommending Approval:", cellFont));
                        footerCell.Colspan = 3;
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 7)
                    {
                        footerCell = new PdfPCell(new Phrase("Approved by:", cellFont));
                        footerCell.Colspan = 2;
                        footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                }

                for (int i = 1; i <= 15; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 15)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                for (int i = 1; i <= 15; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 15)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                for (int i = 1; i <= 5; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 5)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 1)
                    {
                        footerCell = new PdfPCell(new Phrase("MS. LUCHIE QUIRANTE", headerFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 3)
                    {
                        footerCell = new PdfPCell(new Phrase("DR. JONATHAN NEIL V. ERASMO, MD, MPH, FPSMS", headerFont));
                        footerCell.Colspan = 5;
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 4)
                    {
                        footerCell = new PdfPCell(new Phrase("DR. JAIME S. BERNADAS, MGM, CESO III", headerFont));
                        footerCell.Colspan = 4;
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                for (int i = 1; i <= 5; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 5)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 1)
                    {
                        footerCell = new PdfPCell(new Phrase("BAC Secretariat\n\n\n\n\n", cellFont));
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 3)
                    {
                        footerCell = new PdfPCell(new Phrase("\nBAC Chairperson\nInfrastructure/Civil Works, Drugs and Medicines,\nMedical and Dental Supplies, Materials, Reagents \nand Active Ingredients\nGoods, Equipment and Consulting Services", cellFont));
                        footerCell.Colspan = 5;
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }

                    if (i == 4)
                    {
                        footerCell = new PdfPCell(new Phrase("Director IV\nHead of the Procuring Agency\n\n\n\n", cellFont));
                        footerCell.Colspan = 4;
                        footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        footerCell.Border = PdfPCell.NO_BORDER;
                        footer.AddCell(footerCell);
                    }
                }

                for (int i = 1; i <= 15; i++)
                {
                    PdfPCell footerCell = new PdfPCell(new Phrase(" ", cellFont));
                    footerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    if (i == 1)
                    {
                        footerCell.Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else if (i == 15)
                    {
                        footerCell.Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER;
                        //footerCell.Border = PdfPCell.BOTTOM_BORDER;
                        footer.AddCell(footerCell);
                    }
                    else
                    {
                        footerCell.Border = PdfPCell.BOTTOM_BORDER;
                        footer.AddCell(footerCell);
                    }
                }




                document.Add(table);
                document.Add(footer);

                document.Close();

                Response.Headers.Add("Content-Disposition", "inline; filename=sample.pdf");
                Response.ContentType = "application/pdf";

                await Response.Body.WriteAsync(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
            }

            return new EmptyResult();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Scheme3", Roles = "app_admin")]
        public async Task<IActionResult> GenerateAppPdf()
        {

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.LEGAL.Rotate());

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);


                document.Open();

                string year = User.FindFirstValue("YearlyRef");

                document.Add(new Paragraph("Department of Health-Central Visayas-Center for Health Development Revised Annual Procurement Plan for FY " + year + "  \n\n"));

                

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

                float[] columnWidths = new float[] { 90f, 250f, 90f, 40f, 40f, 80f, 90f, 90f, 90f, 90f, 70f, 80f, 80f, 80f, 150f };
                table.SetWidths(columnWidths);

                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerCell.FixedHeight = 80f;
                    table.AddCell(headerCell);
                }

                var items = _context.AppModel.OrderBy(x => x.Uacs).ToList();

                for (int row = 0; row < items.Count; row++)
                {
                    string itemUacs = items[row].Uacs;
                    string itemDescription = items[row].ProcurementProject;
                    string endUser = items[row].EndUser;
                    string earlyProcured = items[row].EarlyProcured;
                    string notEarlyProcured = items[row].NotEarlyProcurered;
                    string modeofProcurement = items[row].ModeOfProcurement;
                    string advertising = items[row].Advertising?.ToString("MM-dd-yyyy");
                    string submission = items[row].Submission?.ToString("MM-dd-yyyy");
                    string noticeOfAward = items[row].NoticeOfAward?.ToString("MM-dd-yyyy");
                    string contractSigning = items[row].ContractSigning?.ToString("MM-dd-yyyy");
                    string fundSource = items[row].FundSource;
                    string total = string.Format("{0:N0}", items[row].Total);
                    string mooe = string.Format("{0:N0}", items[row].Mooe);
                    string co = items[row].Co.ToString();
                    string remarks = items[row].Remarks;

                    for (int col = 0; col < headers.Length; col++)
                    {
                        PdfPCell cell;

                        if (col == 0)
                        {
                            cell = new PdfPCell(new Phrase(itemUacs, cellFont));
                        }
                        else if (col == 1)
                        {
                            cell = new PdfPCell(new Phrase(itemDescription, cellFont));
                        }
                        else if (col == 2)
                        {
                            cell = new PdfPCell(new Phrase(endUser, cellFont));
                        }
                        else if (col == 3)
                        {
                            cell = new PdfPCell(new Phrase(earlyProcured, cellFont));
                        }
                        else if (col == 4)
                        {
                            cell = new PdfPCell(new Phrase(notEarlyProcured, cellFont));
                        }
                        else if (col == 5)
                        {
                            cell = new PdfPCell(new Phrase(modeofProcurement, cellFont));
                        }
                        else if (col == 6)
                        {
                            cell = new PdfPCell(new Phrase(advertising, cellFont));
                        }
                        else if (col == 7)
                        {
                            cell = new PdfPCell(new Phrase(submission, cellFont));
                        }
                        else if (col == 8)
                        {
                            cell = new PdfPCell(new Phrase(noticeOfAward, cellFont));
                        }
                        else if (col == 9)
                        {
                            cell = new PdfPCell(new Phrase(contractSigning, cellFont));
                        }
                        else if (col == 10)
                        {
                            cell = new PdfPCell(new Phrase(fundSource, cellFont));
                        }
                        else if (col == 11)
                        {
                            cell = new PdfPCell(new Phrase(total, cellFont));
                        }
                        else if (col == 12)
                        {
                            cell = new PdfPCell(new Phrase(mooe, cellFont));
                        }
                        else if (col == 13)
                        {
                            cell = new PdfPCell(new Phrase(co, cellFont));
                        }
                        else if (col == 14)
                        {
                            cell = new PdfPCell(new Phrase(remarks, cellFont));
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase("", cellFont));
                        }

                        cell.BorderWidthLeft = 1f;
                        cell.BorderWidthRight = col == headers.Length - 1 ? 1f : 0f;
                        cell.BorderWidthTop = 1f;
                        cell.BorderWidthBottom = 0f;
                        cell.BorderWidthBottom = 1f;

                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                document.Close();

                Response.Headers.Add("Content-Disposition", "inline; filename=sample.pdf");
                Response.ContentType = "application/pdf";

                //Response.Body.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);

                await Response.Body.WriteAsync(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
            }

            return new EmptyResult();
        }

        #endregion

        #region LOGIN
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewData["Year"] = _context.Yearly_reference.OrderByDescending(x => x.YearlyReference).ToList();
            bool isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "app_admin":
                        return RedirectToAction("Index", "BudgetAllotment");
                    default:
                        return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, int Year)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);
                if (user is not null)
                {
                    var yearlyReference = _ppmpContext.yearly_reference.FirstOrDefault(x => x.Year == Year);

                    if (yearlyReference != null)
                    {
                        user.Year = yearlyReference.Year.ToString();
                        user.YearId = yearlyReference.Id;
                        await LoginAsync(user, model.RememberMe);

                        if (user.Username == "hr_admin")
                        {
                            return RedirectToAction("Index", "App");
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Year", "Year does not exist in the database.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Username or Password is Incorrect");
                }

            }
            return View(model);
        }
        #endregion

        #region LOGOUT
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return await Logout();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Scheme3");
            return RedirectToAction("Login", "App");
        }
        #endregion

        #region NOT FOUND
        public new IActionResult NotFound()
        {
            return View();
        }
        #endregion

        #region HELPERS

        private async Task LoginAsync(FmisUser user, bool rememberMe)
        {
            var properties = new AuthenticationProperties
            {
                AllowRefresh = false,
                IsPersistent = rememberMe
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Username.Equals("hr_admin") ? "app_admin" : null),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname),
                new Claim("YearlyRef", user.Year),
                new Claim("YearlyRefId", user.YearId.ToString())
            };

            var identity1 = new ClaimsIdentity(claims, "Scheme3");
            var principal1 = new ClaimsPrincipal(identity1);

            await HttpContext.SignInAsync("Scheme3", principal1);
        }
        #endregion

        #region COOKIES
        public string YearId { get { return User.FindFirstValue("YearlyRefId"); } }
        public string Year { get { return User.FindFirstValue("YearlyRef"); } }



        #endregion
    }
}
