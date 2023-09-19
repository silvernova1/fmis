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
     //   private readonly UserContext _userContext;
        public UsersController(fmisContext FmisContext)
        {
            _fmisContext = FmisContext;
          
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

      

    }
}
