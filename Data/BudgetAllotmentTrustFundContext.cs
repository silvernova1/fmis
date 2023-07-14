using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class BudgetAllotmentTrustFundContext : DbContext
    {
        public BudgetAllotmentTrustFundContext(DbContextOptions<BudgetAllotmentTrustFundContext> options)
         : base(options)
        {
        }

        public DbSet<BudgetAllotmentTrustFund> BudgetAllotmentTrustFund { get; set; }
    }
}
