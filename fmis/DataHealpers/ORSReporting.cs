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
            var Budget_allotments = db.Budget_allotments.Where(p => p.BudgetAllotmentId.ToString() == ors_allotment).FirstOrDefault();
            return Budget_allotments.Allotment_code ?? "Allotment Code 234";
        }
    }
}
