using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Controllers
{
    public class ORS_headControllers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
