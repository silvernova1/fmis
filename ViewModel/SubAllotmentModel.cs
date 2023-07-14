using fmis.Models;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class SubAllotmentModel
    {
        public List<SubAllotment> Customers { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
    }
}
