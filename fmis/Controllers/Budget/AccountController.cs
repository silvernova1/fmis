using fmis.Areas.Identity.Data;
using fmis.Data;
using fmis.Models;
using fmis.Models.Budget;
using fmis.Models.silver;
using fmis.ViewModel;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget
{

    public class AccountController : Controller
    {
        
        private readonly MyDbContext _context;
        private readonly UserManager<fmisUser> _userManager;
        private readonly SignInManager<fmisUser> _signInManager;
        
        public AccountController(MyDbContext context, SignInManager<fmisUser> _signInManager)
        {
            _context = context;
            this._signInManager = _signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Year", "Account");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        public IActionResult Dashboard()
        {
            return RedirectToAction("~/Views/Budget_allotments/Index.cshtml");
        }

        //GET
        [Authorize]
        public IActionResult Year()
        {
            return View(new ManageUsers());
        }

        //POST
        [HttpPost]
        public IActionResult Year(string year)
        {         
                //var blog = _context.Yearly_reference.Where(s => s.YearlyReference == year).FirstOrDefault().ToString();

                if (year == "2021")
                {  
                    return RedirectToAction("Dashboard", "Home");
                }
                else { 
                    return RedirectToAction("SetYear", "Account");
                }

            



            // return View(result);
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
