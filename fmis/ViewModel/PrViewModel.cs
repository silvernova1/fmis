
using fmis.Models.ppmp;
using fmis.Models.pr;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class PrViewModel
    {

        public Pr Pr { get; set; }
        public List<Item> PrItems { get; set; }
        public List<Expense> Expenses { get; set; }
        public List<Item> Items { get; set; }

        public int SelectedExpenseId { get; set; }
        public int SelectedItemId { get; set; }
    }
}
