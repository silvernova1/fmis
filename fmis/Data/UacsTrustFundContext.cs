using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class UacsTrustFundContext : DbContext
    {
        public UacsTrustFundContext(DbContextOptions<UacsTrustFundContext> options)
       : base(options)
        {
        }

        public DbSet<UacsTrustFund> UacsTrustFund { get; set; }

    }
}
