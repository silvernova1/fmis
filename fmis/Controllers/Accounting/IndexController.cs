using fmis.Data;
using fmis.Models.UserModels;
using fmis.Services;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fmis.Controllers.Accounting
{
    public class IndexController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IUserService _userService;
        public IndexController(MyDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                switch (User.FindFirstValue(ClaimTypes.Role))
                {
                    case "accounting_admin":
                        return RedirectToAction("Index", "IndexOfPayment");
                    default:
                        return RedirectToAction("Index", "Dv");
                }
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
                    await LoginAsync(user, model.RememberMe);


                    if (user.Username == "201700272" || user.Username == "0623" || user.Username == "0437" || user.Username == "hr_admin" || user.Username == "1731")
                    {
                        return RedirectToAction("Index", "IndexOfPayment");
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
            else if (User.IsInRole("user"))
            {
                return RedirectToAction("Login", "Index");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


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
                new Claim(ClaimTypes.Role, user.Username.Equals("201700272")||user.Username.Equals("0623")||user.Username.Equals("0437")||user.Username.Equals("hr_admin")?"accounting_admin" : "user"),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname)
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
