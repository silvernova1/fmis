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
        

       /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

          *//*  modelBuilder.Entity<FundSource>()
                .HasOne(p => p.Budget_allotment)
                .WithMany(b => b.FundSources)
                .HasForeignKey(e => e.BudgetAllotmentId);*/

            /*modelBuilder.Entity<FundSource>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSources);*/
            /* .HasForeignKey(p => p.BudgetAllotmentForeignKey);*/

            /*modelBuilder.Entity<Budget_allotment>()
                .HasMany(b => b.FundSources)
                .WithOne(p => p.Budget_allotment)
                .OnDelete(DeleteBehavior.NoAction);*//*


            modelBuilder.Entity<FundSource>().ToTable("FundSource");
            modelBuilder.Entity<Budget_allotment>().ToTable("Budget_allotment");

        }*/


        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public DbSet<Budget_allotment> Budget_allotments { get; set; }
        public DbSet<fmis.Models.John.FundSource> FundSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<FundSource>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSources);
            /*.HasForeignKey(p => p.BudgetAllotmentId);*/



            modelBuilder.Entity<Budget_allotment>().ToTable("Budget_allotment");
            modelBuilder.Entity<FundSource>().ToTable("FundSource");
        }
    }
}
