using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data.John
{
    public class FundSourceContext : DbContext
    {
        public FundSourceContext(DbContextOptions<FundSourceContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.John.FundSource> FundSource { get; set; }
    }
}
