using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.John;

namespace fmis.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<Budget_allotment> Budget_allotment { get; set; }
        public DbSet<fmis.Models.John.FundSource> FundSource { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<FundSource>()
                .HasOne<Budget_allotment>(p => p.Budget_allotment)
                .WithMany(b => b.FundSource);
                /*.HasForeignKey(e => e.BudgetAllotmentId);*/
        }


        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}
