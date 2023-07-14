using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class ObligationAmountTrustFundContext : DbContext
    {
        public ObligationAmountTrustFundContext(DbContextOptions<ObligationAmountTrustFundContext> options)
        : base(options)
        {
        }
        public DbSet<fmis.Models.ObligationAmountTrustFund> ObligationAmountTrustFund { get; set; }
    }
}

