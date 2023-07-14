using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class PrexcTrustFundContext : DbContext
    {
        public PrexcTrustFundContext(DbContextOptions<PrexcTrustFundContext> options)
        : base(options)
        {
        }
        public DbSet<fmis.Models.PrexcTrustFund> PrexcTrustFund { get; set; }
    }
}
