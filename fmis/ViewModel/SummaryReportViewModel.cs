using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.ViewModel
{
    public class SummaryReportViewModel
    {

        public FundSource fund_source { get; set; }
        public Prexc prexc { get; set; }
        public FundSourceAmount fundsource_amount { get; set; }
        public Uacs uacs { get; set; }
        public BudgetAllotment budget_allotment { get; set; }

    }
}
