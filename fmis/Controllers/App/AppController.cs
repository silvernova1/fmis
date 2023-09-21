using DocumentFormat.OpenXml.Spreadsheet;
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

namespace fmis.Controllers.App
{
    
    public class AppController : Controller
    {
        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppController(MyDbContext context, IUserService userService, PpmpContext ppmpContext, DtsContext dts, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
            _ppmpContext = ppmpContext;
            _dts = dts;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(AuthenticationSchemes = "Scheme3", Roles = "app_admin")]
        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var expenses = new List<Expense>();
            if (_context.Expense.Count() > 0)
            {
                expenses = _context.Expense.Include(x=>x.Items).Include(x => x.AppModels).ToList();
                ViewBag.expenses = expenses;

                ViewBag.appModels = _context.AppModel.ToList();
            }
            else
            {
                expenses = _ppmpContext.expense.Include(x=>x.Items.Where(x=>x.Yearly_ref_id == 4 && x.Status != null)).ToList();

                ViewBag.expenses = expenses;
            }

           /* var matchingExpenseCodes = _context.Uacs
            .Where(uac => _context.Expense.Any(expense =>
                uac.Account_title.Contains(expense.Description.Replace("I. ", "").Replace("II. ", ""))))
            .Select(uac => uac.Expense_code)
            .ToList();*/

            return View(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateExpense(List<Expense> expenses)
        {
            foreach(var expense in expenses)
            {
                var existingExpenseId = _context.Expense.FirstOrDefault(x=>x.Id == expense.Id)?.Id ?? null;

                if(expense.Id != existingExpenseId)
                {
                    expense.Id = 0;
                    await _context.Expense.AddAsync(expense);
                }
                else
                {
                    var existingExpense = _context.Expense.Include(x => x.AppModels).FirstOrDefault(e => e.Id == expense.Id);
                    if (existingExpense != null)
                    {
                        if (expense.AppModels != null)
                        {
                            foreach (var updatedAppModel in expense.AppModels)
                            {
                                if (updatedAppModel == null)
                                {
                                    continue;
                                }
                                var existingAppModel = existingExpense.AppModels.FirstOrDefault(am => am.Id == updatedAppModel.Id);

                                if (existingAppModel != null)
                                {
                                    if (existingAppModel.ProcurementProject != updatedAppModel.ProcurementProject
                                        || existingAppModel.EndUser != updatedAppModel.EndUser
                                        || existingAppModel.ModeOfProcurement != updatedAppModel.ModeOfProcurement
                                        || existingAppModel.Advertising != updatedAppModel.Advertising
                                        || existingAppModel.Submission != updatedAppModel.Submission
                                        || existingAppModel.NoticeOfAward != updatedAppModel.NoticeOfAward
                                        || existingAppModel.ContractSigning != updatedAppModel.ContractSigning
                                        || existingAppModel.FundSource != updatedAppModel.FundSource
                                        || existingAppModel.Total != updatedAppModel.Total
                                        || existingAppModel.Mooe != updatedAppModel.Mooe
                                        || existingAppModel.Co != updatedAppModel.Co
                                        || existingAppModel.Remarks != updatedAppModel.Remarks)
                                    {
                                        existingAppModel.ProcurementProject = updatedAppModel.ProcurementProject;
                                        existingAppModel.EndUser = updatedAppModel.EndUser;
                                        existingAppModel.ModeOfProcurement = updatedAppModel.ModeOfProcurement;
                                        existingAppModel.Advertising = updatedAppModel.Advertising;
                                        existingAppModel.Submission = updatedAppModel.Submission;
                                        existingAppModel.NoticeOfAward = updatedAppModel.NoticeOfAward;
                                        existingAppModel.ContractSigning = updatedAppModel.ContractSigning;
                                        existingAppModel.FundSource = updatedAppModel.FundSource;
                                        existingAppModel.Total = updatedAppModel.Total;
                                        existingAppModel.Mooe = updatedAppModel.Mooe;
                                        existingAppModel.Co = updatedAppModel.Co;
                                        existingAppModel.Remarks = updatedAppModel.Remarks;
                                    }

                                }
                                else
                                {
                                    existingExpense.AppModels.Add(updatedAppModel);
                                }

                            }
                        }
                        

                        _context.Entry(existingExpense).State = EntityState.Detached;

                        _context.Entry(expense).State = EntityState.Modified;
                    }
                }

            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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

                var items = _context.AppModel.OrderBy(x=>x.Expense_id).ToList();

                for (int row = 0; row < items.Count; row++)
                {
                    string itemDescription = items[row].ProcurementProject;
                    string endUser = items[row].EndUser;
                    string modeofProcurement = items[row].ModeOfProcurement;
                    string advertising = items[row].Advertising?.ToString("MM-dd-yy");
                    string submission = items[row].Submission?.ToString("MM-dd-yy");
                    string noticeOfAward = items[row].NoticeOfAward?.ToString("MM-dd-yy");
                    string contractSigning = items[row].ContractSigning?.ToString("MM-dd-yy");
                    string fundSource = items[row].FundSource;
                    string total = items[row].Total.ToString();
                    string mooe = items[row].Mooe.ToString();
                    string co = items[row].Co.ToString();
                    string remarks = items[row].Remarks;

                    for (int col = 0; col < headers.Length; col++)
                    {
                        PdfPCell cell;

                        if (col == 1)
                        {
                            cell = new PdfPCell(new Phrase(itemDescription, cellFont));
                        }
                        else if(col == 2)
                        {
                            cell = new PdfPCell(new Phrase(endUser, cellFont));
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
                            cell = new PdfPCell(new Phrase(/*$"Row {row + 1}, Cell {col + 1}"*/"", cellFont));
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["Year"] = _context.Yearly_reference.ToList();
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);
                if (user is not null)
                {
                    user.Year = model.Year.ToString();
                    user.YearId = _context.Yearly_reference.FirstOrDefault(x => x.YearlyReference == user.Year).YearlyReferenceId;
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
        public string year { get { return User.FindFirstValue("Year"); } }
        #endregion
    }
}
