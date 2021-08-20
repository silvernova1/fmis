using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<fmis.Models.John.FundSource> Uacs { get; set; }
        public DbSet<fmis.Models.Uacs> UacsAmount { get; set; }

/*        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Uacs>()
                .HasOne(p => p.FundSource)
                .WithMany(b => b.Uacses);
        }*/


        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}
