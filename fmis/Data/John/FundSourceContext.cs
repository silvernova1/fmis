using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.John;

namespace fmis.Data.John
{
    public class FundSourceContext : DbContext
    {
        public DbSet<FundSource> FundSource { get; set; }
        public DbSet<Uacs> Uacs { get; set; }


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
