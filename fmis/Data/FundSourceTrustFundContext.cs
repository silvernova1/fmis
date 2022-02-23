using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class FundSourceTrustFundContext : DbContext
    {
        public FundSourceTrustFundContext(DbContextOptions<FundSourceTrustFundContext> options)
                : base(options)
        {
        }

        public DbSet<fmis.Models.FundSourceTrustFund> FundSourceTrustFund { get; set; }
    }
}
