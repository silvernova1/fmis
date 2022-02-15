using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers.Budget
{
    public class BudgetAllotmentTrustFundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
