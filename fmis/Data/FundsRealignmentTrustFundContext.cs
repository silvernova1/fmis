using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class FundsRealignmentTrustFundContext : DbContext
    {
        public FundsRealignmentTrustFundContext(DbContextOptions<FundsRealignmentTrustFundContext> options)
     : base(options)
        {
        }

        public DbSet<FundsRealignmentTrustFund> FundsRealignmentTrustFund { get; set; }
    }
}
