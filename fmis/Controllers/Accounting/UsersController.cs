using fmis.Data.Accounting;
using fmis.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Data;
using fmis.ViewModel;
using Microsoft.AspNetCore.Authorization;
using fmis.Filters;

namespace fmis.Controllers.Accounting
{
    [Authorize(AuthenticationSchemes = "Scheme2", Roles = "accounting_admin")]
    public class UsersController : Controller
    {
        
        private readonly fmisContext _fmisContext;
        private readonly MyDbContext _myDbContext;
        public UsersController(fmisContext FmisContext, MyDbContext myDbContext)
        {
            _fmisContext = FmisContext;
            _myDbContext = myDbContext;
          
        }

        
        [Route("Accounting/Users")]
        public async Task<IActionResult> Index(string searchEmployee)
        {
            ViewBag.filter = new FilterSidebar("Accounting", "index_of_payment", "index");
            
            var users = _fmisContext.users
                .Where(u => string.IsNullOrEmpty(searchEmployee) || u.Username.Contains(searchEmployee) || u.Email.Contains(searchEmployee))
                .ToList();
             

            return  View(users);
        }
        [HttpPost]
        public async Task<IActionResult> SaveUsers(int selectedEmployee)
        {
            var user = _fmisContext.users.FirstOrDefault(x => x.Id == selectedEmployee);

            return Json(user);

        }

      
      

    }
}
