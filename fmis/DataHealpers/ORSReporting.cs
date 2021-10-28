using fmis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.DataHealpers
{
    public class ORSReporting
    {
        private MyDbContext db = new MyDbContext();

        public String GetOrsCode(String ors_allotment)
        {
            var allotment = db.Obligation.Where(p => p.Id.ToString() == ors_allotment).FirstOrDefault();
            return allotment.Dv ?? "";
        }
    }
}
