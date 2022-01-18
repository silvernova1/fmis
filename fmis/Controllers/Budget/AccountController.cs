using fmis.Areas.Identity.Data;
using fmis.Data;
using fmis.Models;
using fmis.Models.Budget;
using fmis.Models.silver;
using fmis.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers.Budget
{

    public class AccountController : Controller
    {
        
        private readonly MyDbContext _context;
        private readonly SignInManager<fmisUser> signInManager;

        public AccountController(MyDbContext context, SignInManager<fmisUser> signInManager)
        {
            _context = context;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("dashboard", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ManageUsers model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Year", "Account");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(ManageUsers user)
        {
            if (ModelState.IsValid)
            {
                var userobj = _context.ManageUsers.Where(a => a.Username.Equals(user.Username) && a.Password.Equals(user.Password)).FirstOrDefault();
                if (userobj !=null)
                {
                    *//*return RedirectToAction("dashboard", "home");*//*
                    return RedirectToAction("SetYear", "Account");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(user);
        }*/

        public IActionResult Dashboard()
        {
            return RedirectToAction("~/Views/Budget_allotments/Index.cshtml");
        }
        public IActionResult Year()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Year([Bind("YearlyReferenceId,YearlyReference,Created_at,Created_At,Updated_at")] Yearly_reference year)
        {

            
            if (ModelState.IsValid)
            {
                var data = _context.Yearly_reference.FirstOrDefault(s=>s.YearlyReference == year.YearlyReference).YearlyReference;
                if (data == year.YearlyReference)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                ModelState.AddModelError(string.Empty, "Please input year");
            }
            return View(year);
        }

        /*[Route("")]
        [Route("index")]
        [Route("~/")]*//*
        public IActionResult Index()
        {
            return View("~/Views/Budget/Account/Index.cshtml");
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var uname = "123";
            var pass = "123";

            if (username != null && password != null && username.Equals(uname) && password.Equals(pass))
            {
                HttpContext.Session.SetString("username", uname);
                return View("~/Views/Budget/Account/Success.cshtml");
            }
            else
            {
                ViewBag.error = "Invalid Account";
                return View("~/Views/Budget/Account/Index.cshtml");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("~/Views/Budget/Account/Index.cshtml");
        }*/
    }
}
