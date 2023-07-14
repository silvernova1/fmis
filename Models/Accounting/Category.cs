 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Category : BaseEntityTimeStramp
    {
        public int CategoryId { get; set; }
        public string CategoryDescription { get; set; }

    }

    public class PaginationViewModel<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
