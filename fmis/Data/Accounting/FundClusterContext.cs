using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class FundClusterContext : DbContext
    {
        public FundClusterContext(DbContextOptions<FundClusterContext> options)
        : base(options)
        {
        }
    }
}
