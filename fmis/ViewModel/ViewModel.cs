using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models;

namespace fmis.ViewModel
{
    public class ViewModel
    {
        public IEnumerable<Budget_allotment> Budget_allotments { get; set; }
        public IEnumerable<FundSource> FundSources { get; set; }
    }
}
