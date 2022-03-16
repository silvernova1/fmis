using fmis.Models;
using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.ViewModel
{
    public class SaobViewModel
    {
        public BudgetAllotment budget_allotment { get; set; }
        public SubAllotment sub_allotment { get; set; }
        public Suballotment_amount suballotment_amount { get; set; }
        public Uacs uacs { get; set; }
        public Prexc prexc { get; set; }


    }
}
