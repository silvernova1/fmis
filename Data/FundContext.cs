using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using System.Threading;

namespace fmis.Data
{
    public class FundContext : DbContext
    {

        public FundContext(DbContextOptions<FundContext> options)
                  : base(options)
        {
        }

        public DbSet<fmis.Models.Fund> Fund { get; set; }
    }
}
