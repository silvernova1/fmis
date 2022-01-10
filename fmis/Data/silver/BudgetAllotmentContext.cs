using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.silver;

namespace fmis.Data
{
    public class BudgetAllotmentContext : DbContext
    {

        public BudgetAllotmentContext(DbContextOptions<BudgetAllotmentContext> options)
            : base(options)
        {
        }

        public DbSet<BudgetAllotment> BudgetAllotment { get; set; }
    }
}