using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class BudgetAllotmentContext : DbContext
    {

        public BudgetAllotmentContext(DbContextOptions<BudgetAllotmentContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.BudgetAllotment> Budget_allotment{ get; set; }
    }
}
