using fmis.Models;
using fmis.Models.Carlo;
using fmis.Models.John;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class BudgetAllotmentViewModel
    {

        public IEnumerable<FundSource> FundSources { get; set; }
        public IEnumerable<AllotmentClass> AllotmentClass { get; set; }
        public IEnumerable<Appropriation> Appropriation { get; set; }
        public IEnumerable<FundTransferedTo> FundTransferedTo { get; set; }
        public IEnumerable<FundSourceAmount> FundSourceAmount { get; set; }
        public IEnumerable<Uacs> Uacs { get; set; }

        public IEnumerable<SubAllotment> SubAllotments { get; set; }
        public IEnumerable<SubTransferedTo> SubTransferedTo { get; set; }

    }
}
