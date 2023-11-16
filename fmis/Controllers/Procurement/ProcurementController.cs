using fmis.Models.UserModels;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using fmis.Data;
using fmis.Data.MySql;
using fmis.Filters;
using fmis.Models.Accounting;
using fmis.Models.ppmp;

using fmis.Services;

using iTextSharp.text.pdf;
using iTextSharp.text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Item = fmis.Models.ppmp.Item;
using Font = iTextSharp.text.Font;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace fmis.Controllers.Procurement
{
    
    public class ProcurementController : Controller
    {

        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProcurementController(MyDbContext context, IUserService userService, PpmpContext ppmpContext, DtsContext dts, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
            _ppmpContext = ppmpContext;
            _dts = dts;
            _httpContextAccessor = httpContextAccessor;
        }



        //INDEX
        #region
        public IActionResult Index()
        {

            return View();
        }
        #endregion


        //CHECKLIST1
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist1()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist1");
            return View();
        }
        #endregion

        //CHECKLIST2
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist2()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist2");
            return View();
        }
        #endregion

        //CHECKLIST3
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist3()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist3");
            return View();
        }
        #endregion

        //CHECKLIST4
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist4()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist4");
            return View();
        }
        #endregion

        //CHECKLIST5
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist5()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist5");
            return View();
        }
        #endregion

        //CHECKLIST6
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist6()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist6");
            return View();
        }
        #endregion

        //CHECKLIST7
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist7()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist7");
            return View();
        }
        #endregion

        //CHECKLIST8
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist8()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist8");
            return View();
        }
        #endregion

        //CHECKLIST9
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist9()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist9");
            return View();
        }
        #endregion

        //CHECKLIST10
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist10()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist10");
            return View();
        }
        #endregion

        //CHECKLIST11
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist11()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist11");
            return View();
        }
        #endregion

        //CHECKLIST12
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist12()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist12");
            return View();
        }
        #endregion

        //CHECKLIST13
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist13()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist13");
            return View();
        }
        #endregion

        //CHECKLIST14
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist14()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist14");
            return View();
        }
        #endregion

        //CHECKLIST15
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist15()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist15");
            return View();
        }
        #endregion

        //CHECKLIST16
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist16()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist16");
            return View();
        }
        #endregion

        //CHECKLIST17
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist17()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist17");
            return View();
        }
        #endregion

        //CHECKLIST18
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Checklist18()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Checklist", "Checklist18");
            return View();
        }
        #endregion


        //LOGBOOK BAC RES NO
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult BacResolutionNo()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Bac", "BacResolutionNo");
            return View();
        }
        #endregion


        //LOGBOOK BAC RES NO EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult BacResolutionNoEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("BacResolutionNoEdit");
        }
        #endregion



        //RMOP SIGNATORY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult RmopSignatory()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Signatory", "RmopSignatory");
            return View();
        }
        #endregion

        //RMOP SIGNATORY EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult RmopSignatoryEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("RmopSignatoryEdit");
        }
        #endregion


        //RMOP AGENCY TO AGENCY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AgencyToAgency()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "AgencyToAgency");
            return View();
        }
        #endregion

        //RMOP DIRECT CONTRACTING
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult DirectContracting()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "DirectContracting");
            return View();
        }
        #endregion


        //RMOP DIRECT EMERGENCY CASES
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult EmergencyCases()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "EmergencyCases");
            return View();
        }
        #endregion


        //RMOP LEASE OF VENUE
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult LeaseOfVenue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "LeaseOfVenue");
            return View();
        }
        #endregion

        //RMOP PUBLIC BIDDING
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PublicBidding()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PublicBidding");
            return View();
        }
        #endregion


        //RMOP PS-DBM
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PsDbm()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "PsDbm");
            return View();
        }
        #endregion


        //RMOP SCIENTIFIC SCHOLARLY
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult ScientificScholarly()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "ScientificScholarly");
            return View();
        }
        #endregion

        //RMOP SMALL VALUE
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult SmallValue()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Rmop", "SmallValue");
            return View();
        }
        #endregion


   
        //LOGBOOK CANVASS
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Canvass()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Canvass");
            return View();
        }
        #endregion

        //LOGBOOK CANVASS EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult CanvassEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("CanvassEdit");
        }
        #endregion


        //LOGBOOK ABSTRACT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Abstract()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Abstract");
            return View();
        }
        #endregion

        //LOGBOOK ABSTRACT EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult AbstractEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("AbstractEdit");
        }
        #endregion


        //LOGBOOK PURCHASE ORDER
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrder()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "PurchaseOrder");
            return View();
        }
        #endregion

        //LOGBOOK PURCHASE ORDER EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult PurchaseOrderEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("PurchaseOrderEdit");
        }
        #endregion

        //LOGBOOK TWF
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Twg()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "LogBook", "Twg");
            return View();
        }
        #endregion

        //LOGBOOK PURCHASE ORDER EDIT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult TwgEdit()
        {
            // You can add any necessary logic here before rendering the CanvassEdit view.
            return PartialView("TwgEdit");
        }
        #endregion



        //PRINT
        #region
        [Authorize(AuthenticationSchemes = "Scheme4", Roles = "pu_admin")]
        public IActionResult Print()
        {
            ViewBag.filter = new FilterSidebar("Procurement", "Recommendation", "Print");
            return View();
        }
        #endregion



        #region LOGIN
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            bool isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "pu_admin":
                        return RedirectToAction("Checklist1", "Procurement");
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
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);
                if (user is not null)
                {
                    user.Year = model.Year.ToString();
                    await LoginAsync(user, model.RememberMe);


                    if (user.Username == "hr_admin")
                    {
                        return RedirectToAction("Checklist1", "Procurement");
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
            await HttpContext.SignOutAsync("Scheme4");
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
                new Claim(ClaimTypes.Role, user.Username.Equals("hr_admin") ? "pu_admin" : null),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname),
            };

            var identity1 = new ClaimsIdentity(claims, "Scheme4");
            var principal1 = new ClaimsPrincipal(identity1);

            await HttpContext.SignInAsync("Scheme4", principal1);
        }
        #endregion


    }
}
