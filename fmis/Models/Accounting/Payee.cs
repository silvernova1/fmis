using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Payee : BaseEntityTimeStramp
    {
        public int PayeeId { get; set; }
        public string PayeeDescription { get; set; }
    }
}
