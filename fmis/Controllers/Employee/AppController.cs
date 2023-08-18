using AspNetCore;
using fmis.Data;
using fmis.Data.MySql;
using fmis.Filters;
using fmis.Models.ppmp;
using fmis.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");

            var expenses = new List<Expense>();
            if (_context.Expense.Count() > 0)
            {
                expenses = _context.Expense.Include(x => x.Items).Take(2).ToList();
            }
            else
            {
                expenses = _ppmpContext.expense.Include(x => x.Items).Take(2).ToList();
            }

            return View(expenses);
        }
        [HttpPost]
        public async Task<IActionResult> Create(List<Expense> expenses)
        {

            foreach (var expense in expenses)
            {
                var existingExpense = _context.Expense.FirstOrDefault(e => e.Id == expense.Id);

                if (existingExpense != null)
                {
                    existingExpense.Description = expense.Description;

                    existingExpense.Items.Clear();
                    foreach (var newItem in expense.Items)
                    {
                        var item = new Item
                        {
                            Description = newItem.Description,
                        };
                        existingExpense.Items.Add(item);
                    }
                }
                else
                {
                    var newExpense = new Expense
                    {
                        Description = expense.Description,
                        Items = new List<Item>()
                    };
                    foreach (var newItem in expense.Items)
                    {
                        var item = new Item
                        {
                            Description = newItem.Description
                        };
                        newExpense.Items.Add(item);
                    }
                    _context.Expense.Add(newExpense);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
