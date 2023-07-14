using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Deduction : BaseEntityTimeStramp
    { 
        public int DeductionId { get; set; }
        public string DeductionDescription { get; set; }
    }
}
