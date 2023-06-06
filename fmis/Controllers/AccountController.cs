﻿using fmis.Data;
using fmis.Models;
using fmis.Models.UserModels;
using fmis.Models.Budget;
using fmis.Models.silver;
using fmis.Services;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Filters;

namespace fmis.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly MyDbContext _context;
        
        public AccountController(MyDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            ViewBag.filter = new FilterSidebar("user_main", "users", "");
            ViewBag.layout = "_Layout";
            return View(_context.FmisUsers.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.filter = new FilterSidebar("user_main", "new_user", "");
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {

            ViewBag.filter = new FilterSidebar("user_main", "new_user", "");
            ViewBag.layout = "_Layout";
            return View("~/Views/Account/CreateUser.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(FmisUser fmisUser)
        {

            fmisUser.Password = _userService.HashPassword(fmisUser, fmisUser.Password);
            _context.Add(fmisUser);
            _context.SaveChanges();
            await Task.Delay(500);
            ViewBag.filter = new FilterSidebar("user_main", "user_sub");
            ViewBag.layout = "_Layout";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.FmisUsers.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FmisUser user)
        {
            user.Password = _userService.HashPassword(user, user?.Password);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.FmisUsers.FirstOrDefaultAsync(x=>x.Id == id);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        #region LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Year"] = _context.Yearly_reference.OrderByDescending(x => x.YearlyReference).ToList();
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "accounting_admin":
                        return RedirectToAction("Dashboard", "Home");
                    default:
                        return RedirectToAction("Index", "Dv");
                }
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
                    user.Year = (await _context.Yearly_reference.FirstOrDefaultAsync(x => x.YearlyReferenceId == model.Year))?.YearlyReference;
                    user.YearId = model.Year;
                    await LoginAsync(user, model.RememberMe);

                    
                    if (user.Username == "201700272" || user.Username == "0623" || user.Username =="0437" || user.Username == "1731")
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Dv");
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (User.IsInRole("accounting_admin"))
            {
                return RedirectToAction("Login", "Index");
            }
            else if(User.IsInRole("user"))
            {
                return RedirectToAction("Login", "Index");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
                new Claim(ClaimTypes.Role, user.Username.Equals("201700272")||user.Username.Equals("0623")||user.Username.Equals("0437")||user.Username.Equals("hr_admin")?"accounting_admin" : "budget_admin"),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname),
                new Claim("YearlyRef", user.Year),
                new Claim("YearlyRefId", user.YearId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, properties);
        }
        #endregion

        #region COOKIES
        public string UserRole { get { return User.FindFirstValue(ClaimTypes.Role); } }
        #endregion
    }
}
