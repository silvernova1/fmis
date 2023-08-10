using fmis.Data;
using fmis.Data.MySql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fmis.Controllers.Employee
{
    public class AppController : Controller
    {
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly MyDbContext _context;

        public AppController(PpmpContext ppmpContext, DtsContext dts, MyDbContext context)
        {
            _ppmpContext = ppmpContext;
            _dts = dts;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_ppmpContext.item.ToList());
        }
    }
}
