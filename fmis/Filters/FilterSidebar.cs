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

        public FilterSidebar(string main, string sub) {
            this.main = main;
            this.sub = sub;
        }
    }
}
