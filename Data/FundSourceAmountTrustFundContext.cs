using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class FundSourceAmountTrustFundContext : DbContext
    {
        public FundSourceAmountTrustFundContext(DbContextOptions<FundSourceAmountTrustFundContext> options)
                : base(options)
        {
        }

        public DbSet<fmis.Models.FundSourceAmountTrustFund> FundSourceAmountTrustFund { get; set; }
    }
}
