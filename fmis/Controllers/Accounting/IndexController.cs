using fmis.Data;
using fmis.Models.UserModels;
using fmis.Services;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace fmis.Controllers.Accounting
{
    public class IndexController : Controller
    {
        private readonly fmisContext _context;
        private readonly IUserService _userService;
        public IndexController(fmisContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        #region LOGIN
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
        public async Task<IActionResult> Login(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateUserCredentialsAsync(model.Username, model.Password);
                if (user is not null)
                {
                    await LoginAsync(user, model.RememberMe);


                    if (user.Username == "201700272" || user.Username == "0623" || user.Username == "0437" || user.Username == "hr_admin")
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
            await HttpContext.SignOutAsync("Scheme2");
            return RedirectToAction("Login", "Index");
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
                new Claim(ClaimTypes.Role, user.Username.Equals("201700272")||user.Username.Equals("0623")||user.Username.Equals("0780")||user.Username.Equals("0437")||user.Username.Equals("hr_admin")?"accounting_admin" : "accounting_user"),
                new Claim(ClaimTypes.GivenName, user.Fname),
                new Claim(ClaimTypes.Surname, user.Lname)
            };

            var identity1 = new ClaimsIdentity(claims, "Scheme2");
            var principal1 = new ClaimsPrincipal(identity1);

            await HttpContext.SignInAsync("Scheme2", principal1);
        }
        #endregion

        #region COOKIES
        public string UserRole { get { return User.FindFirstValue(ClaimTypes.Role); } }
        #endregion
    }
}
