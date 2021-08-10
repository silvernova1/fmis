using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Data
{
    public class UacsContext : DbContext
    {
        public UacsContext(DbContextOptions<UacsContext> options)
         : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Uacs>().
                Property(p => p.Created_at)
                .HasColumnType("date");

            builder.Entity<Uacs>().
                Property(p => p.Updated_at)
                .HasColumnType("date");
        }

        public DbSet<fmis.Models.Uacs> Uacs { get; set; }
    }
}