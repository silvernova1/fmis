using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.ViewModel
{
    public class DashboardVM
    {
        public IEnumerable<ObligationAmount> ObligationAmounts { get; set; }
        public IEnumerable<FundSource> FundSources { get; set; }
        public IEnumerable<BudgetAllotment> BudgetAllotments { get; set; }
        public IEnumerable<Yearly_reference> Yearly_references { get; set; }
    }
}
