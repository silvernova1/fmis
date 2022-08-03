
using fmis.Data;
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

        #region CREATE ACCOUNT
        public async Task<ActionResult> CreateBudget()
        {
            var user = new FmisUser()
            {
                Id = 0,
                Username = "doh7budget",
                Email = "doh7budget@gmail.com",
                Role = "budget_admin"
            };

            user.Password = _userService.HashPassword(user, "doh7budget");

            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        public async Task<ActionResult> CreateAccounting()
        {
            var user = new FmisUser()
            {
                Id = 0,
                Username = "doh7accounting",
                Email = "doh7accounting@gmail.com",
                Role = "accounting_admin"
            };

            user.Password = _userService.HashPassword(user, "doh7accounting");

            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        #endregion

        #region LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Year"] = _context.Yearly_reference.ToList();
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "budget_admin":
                        return RedirectToAction("Dashboard", "Home");
                    case "budget_user":
                        return RedirectToAction("Dashboard", "Home");
                    case "accounting_admin":
                        return RedirectToAction("Dashboard", "Home");
                    case "accounting_user":
                        return RedirectToAction("Dashboard", "Home");
                    default:
                        return NotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, IFormCollection collection)
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
                    switch (user.Role)
                    {
                        case "budget_admin":
                            return RedirectToAction("Dashboard", "Home");
                        case "budget_user":
                            return RedirectToAction("Dashboard", "Home");
                        case "accounting_admin":
                            return RedirectToAction("Dashboard", "Home");
                        case "accounting_user":
                            return RedirectToAction("Dashboard", "Home");
                        default:
                            return NotFound();
                    }
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
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Login");
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
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("YearlyRef", user.Year),
                new Claim("YearlyRefId", user.YearId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, properties);
        }
        #endregion
    }
}
