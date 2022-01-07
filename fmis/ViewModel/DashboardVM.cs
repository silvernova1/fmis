using fmis.Models;
using fmis.Models.John;
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
    }
}
