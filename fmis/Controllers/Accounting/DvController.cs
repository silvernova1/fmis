using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers.Accounting
{
    public class DvController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
