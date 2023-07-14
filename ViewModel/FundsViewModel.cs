using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.ViewModel
{
    public class FundsViewModel
    {
        public FundSource fundsource { get; set; }
        public FundSourceAmount fundsourceamounts { get; set; }
        public Appropriation appropriations { get; set; }
        public BudgetAllotment budgetallotments { get; set; }

    }
}
