using fmis.Models.ppmp;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class AppViewModel
    {
        public List<Expense> Expenses { get; set; }
        public List<AppModel> AppModels { get; set; }
        public List<Item> Items { get; set; }
    }
}
