using fmis.Areas.Identity.Data;
using fmis.Data;
using fmis.Models;
using fmis.Models.Budget;
using fmis.Models.silver;
using fmis.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Controllers.Budget
{

    public class AccountController : Controller
    {
        private readonly SignInManager<fmisUser> signInManager;
        private readonly MyDbContext context;

        public AccountController(SignInManager<fmisUser> signInManager, MyDbContext context)
        {
            this.signInManager = signInManager;
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            /*var model = new LoginViewModel();
            ViewBag.AlbumList = context.Yearly_reference.ToList();*/
            ViewData["Year"] = context.Yearly_reference.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, IFormCollection collection)
        {
            var value = collection["year"];


            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }
    }
}
