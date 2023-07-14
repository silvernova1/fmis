using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class ObligationTrustFundContext : DbContext
    {
        public ObligationTrustFundContext(DbContextOptions<ObligationTrustFundContext> options)
         : base(options)
        {
        }
        public DbSet<fmis.Models.ObligationTrustFund> ObligationTrustFund { get; set; }
    }

}