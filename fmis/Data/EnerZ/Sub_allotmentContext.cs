using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Sub_allotmentContext : DbContext
    {
        public DbSet<fmis.Models.Sub_allotment> Sub_allotment { get; set; }
        public DbSet<fmis.Models.Uacs> Uacs { get; set; }


        /*  protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
              modelBuilder.Entity<Uacs>()
                  .HasOne(p => p.FundSource)
                  .WithMany(b => b.Uacs);
          }
  */

        public Sub_allotmentContext(DbContextOptions<Sub_allotmentContext> options)
            : base(options)
        {
        }

    }
}