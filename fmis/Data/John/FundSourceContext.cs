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
        public DbSet<fmis.Models.John.FundSource> FundSource { get; set; }
        public DbSet<fmis.Models.Uacs> Uacs { get; set; }


      /*  protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Uacs>()
                .HasOne(p => p.FundSource)
                .WithMany(b => b.Uacs);
        }
*/

        public FundSourceContext(DbContextOptions<FundSourceContext> options)
            : base(options)
        {
        }

    }
}
