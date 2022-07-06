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
        public IndexOfPayment IndexOfPayment { get; set; }

    }
}
