using fmis.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using fmis.Data;

namespace fmis.Controllers.Budget.Carlo
{
    public class TempController : Controller
    {
        private readonly UacsContext _UacsContext;

        public TempController(UacsContext UacsContext)
        {
           
            _UacsContext = UacsContext;
            
        }

        public IActionResult Index()
        {
            ViewBag.filter = new FilterSidebar("ors", "temp");
            ViewBag.layout = "_Layout";

            var uacs_data = JsonSerializer.Serialize(_UacsContext.Uacs.ToList());
            ViewBag.uacs = uacs_data;

            return View("~/Views/Carlo/Temp/Index.cshtml");
        }
    }
}
