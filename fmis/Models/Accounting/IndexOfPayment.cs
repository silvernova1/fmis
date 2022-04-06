using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class IndexOfPayment : BaseEntityTimeStramp
    {
        public int IndexOfPaymentId { get; set; }
        public string CategoryDescription { get; set; }
    }
}
