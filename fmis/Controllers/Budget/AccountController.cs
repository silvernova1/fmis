using fmis.Data;
using fmis.Models.Budget;
using fmis.Models.silver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace fmis.Controllers.Budget
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;
        public AccountController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(ManageUsers user)
        {
            if (ModelState.IsValid)
            {
                var userobj = _context.ManageUsers.Where(a => a.Username.Equals(user.Username) && a.Password.Equals(user.Password)).FirstOrDefault();
                if (userobj !=null)
                {
                    /*return RedirectToAction("dashboard", "home");*/
                    return RedirectToAction("SetYear", "Account");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(user);
        }

        public IActionResult Dashboard()
        {
            return RedirectToAction("~/Views/Budget_allotments/Index.cshtml");
        }

        public IActionResult SetYear()
        {
            return View();
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
