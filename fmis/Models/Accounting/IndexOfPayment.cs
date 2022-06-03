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
        public int CategoryId { get; set; }
        public int DeductionId { get; set; }
        public int DvId { get; set; }
    }
}
