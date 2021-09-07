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
        public DbSet<Obligation> Obligation { get; set; }
        public DbSet<Prexc> Prexc { get; set; }
        public DbSet<Utilization> Utilization { get; set; }
        public DbSet<Sub_allotment> Sub_allotment { get; set; }
        public DbSet<Suballotment_amount> Suballotment_amount { get; set; }
        public DbSet<Requesting_office> Requesting_office { get; set; }
        public DbSet<Yearly_reference> Yearly_reference { get; set; }
        public DbSet<Ors_head> Ors_head { get; set; }
        public DbSet<Uacs> Uacs { get; set; }
        public DbSet<Appropriation> Appropriation { get; set; }
        public DbSet<Obligated_amount> Obligated_amount { get; set; }
        public DbSet<FundSource> FundSource { get; set; }
        public DbSet<AllotmentClass> AllotmentClass { get; set; }




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
