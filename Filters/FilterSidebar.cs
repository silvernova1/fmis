using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Filters
{
    public class FilterSidebar
    {
        public string main { get; set; }
        public string sub { get; set; }
        public string sub2 { get; set; }

        public FilterSidebar(string main, string sub, string sub2) {
            this.main = main;
            this.sub = sub;
            this.sub2 = sub2;
        }

        public FilterSidebar(string v1, string v2)
        {
        }
    }
}
