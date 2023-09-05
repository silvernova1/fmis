using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace fmis.Models.ppmp
{
    public class Expense
    {
        public int Id { get; set; }

        public string Division { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public List<Item> Items { get; set; }

        public List<AppModel> AppModels { get; set; }

    }
}
