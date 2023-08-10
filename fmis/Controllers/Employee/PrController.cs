using fmis.Data.MySql;
using fmis.Data;
using Microsoft.AspNetCore.Mvc;
using fmis.Filters;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models.pr;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using fmis.Models.ppmp;

namespace fmis.Controllers.Employee
{
    public class PrController : Controller
    {
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly MyDbContext _context;

        public PrController(PpmpContext ppmpContext, DtsContext dts, MyDbContext context)
        {
            _ppmpContext = ppmpContext;
            _dts = dts;
            _context = context;
        }
        [Route("Employee/Pr")]
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            ExpenseDropDownList();
            ItemsDropDownList();
            UsersDropDownList();

            var pr = await _context.Pr.Include(x => x.PrItems).ToListAsync();
            ViewBag.ItemDesc = _ppmpContext.item.FirstOrDefault();


            return View(pr);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pr pr)
        {
            _context.Add(pr);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetUnit(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_measurement);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult GetUnitCost(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_cost);
            }
            return NotFound();
        }
        public void ExpenseDropDownList(object selected = null)
        {
            var Query = from e in _ppmpContext.expense
                        orderby e.Id
                        select e;
            ViewBag.ExpenseId = new SelectList(Query, "Id", "Description", selected);
        }
        public void ItemsDropDownList(object selected = null)
        {
            var Query = from i in _ppmpContext.item
                        orderby i.Id
                        select i;
            ViewBag.ItemId = new SelectList(Query, "Id", "Description", selected);
        }
        public void UsersDropDownList(object selected = null)
        {
            var employee = from x in _dts.users
                        orderby x.Id
                        select new
                        {
                            Id = x.Id,
                            Fullname = x.Fname + " " + x.Lname + " " + x.Lname
                        };
            ViewBag.Approval = new SelectList(employee, "Id", "Fullname");
        }
        public List<Item> GetItemsId(int expenseId)
        {
            return _ppmpContext.item.Where(c => c.Expense_id == expenseId).ToList();
        }
    }
}
