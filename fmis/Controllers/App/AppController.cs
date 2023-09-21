using DocumentFormat.OpenXml.Spreadsheet;
using fmis.Data;
using fmis.Data.MySql;
using fmis.Filters;
using fmis.Models.Accounting;
using fmis.Models.ppmp;
using fmis.Models.UserModels;
using fmis.Services;
using fmis.ViewModel;
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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Item = fmis.Models.ppmp.Item;

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

        //[Authorize(AuthenticationSchemes = "Scheme3", Roles = "app_admin")]
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
                expenses = _ppmpContext.expense.Include(x=>x.Items).Take(2).ToList();
            }

            return View(expenses);
        }

        [HttpPost]
        public IActionResult CreateOrUpdateExpense(List<Expense> updatedExpenses)
        {
            foreach (var updatedExpense in updatedExpenses)
            {
                if (updatedExpense.Id == 0)
                {
                    _context.Expense.Add(updatedExpense);
                }
                else
                {
                    var existingExpense = _context.Expense.Include(x => x.AppModels).FirstOrDefault(e => e.Id == updatedExpense.Id);
                    if (existingExpense != null)
                    {

                        foreach(var updatedAppModel in updatedExpense.AppModels)
                        {
                            var existingAppModel = existingExpense.AppModels.FirstOrDefault(am => am.Id == updatedAppModel.Id);

                            if (existingAppModel != null)
                            {
                                if (existingAppModel.ProcurementProject != updatedAppModel.ProcurementProject)
                                {
                                    existingAppModel.ProcurementProject = updatedAppModel.ProcurementProject;
                                }

                            }
                            else
                            {
                                existingExpense.AppModels.Add(updatedAppModel);
                            }

                        }

                        _context.Entry(existingExpense).State = EntityState.Detached;

                        _context.Entry(updatedExpense).State = EntityState.Modified;
                    }
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
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
