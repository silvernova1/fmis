using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Dv : BaseEntityTimeStramp
    {
        public int DvId { get; set; }
        public string DvDescription { get; set; }
        public string Payee { get; set; }
    }
}
