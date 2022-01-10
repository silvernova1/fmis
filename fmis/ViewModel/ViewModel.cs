using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models;
using System.Web.Mvc;
using fmis.Models.silver;

namespace fmis.ViewModel
{
    public class ViewModel
    {
        public IEnumerable<BudgetAllotment> Budget_allotments { get; set; }
        public IEnumerable<FundSource> FundSources { get; set; }

    }
}
