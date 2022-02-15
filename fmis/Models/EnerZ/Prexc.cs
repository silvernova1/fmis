using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.Budget;
using fmis.Models.John;
using fmis.Models.silver;

namespace fmis.Models
{
    public class Prexc : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public string pap_title { get; set; }
        public string pap_code1 { get; set; }
        public string pap_type { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public Sub_allotment Sub_Allotment { get; set; }
        public Uacs Uacs { get; set; }
        public virtual ICollection<FundSourceAmount> FundSourceAmounts { get; set; }

        public int PapTypeID { get; set; }
        public PapType PapType { get; set; }

    }
}
