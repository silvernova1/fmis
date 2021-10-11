using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.Carlo;

namespace fmis.Data
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public DbSet<Section> Section { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Personal_Information> Personal_Information { get; set; }
        public DbSet<Budget_allotment> Budget_allotments { get; set; }
        public DbSet<fmis.Models.John.FundSource> FundSources { get; set; }
        public DbSet<Obligation> Obligation { get; set; }
        public DbSet<Prexc> Prexc { get; set; }
        public DbSet<Personal_Information> Personal_information { get; set; }
        public DbSet<Requesting_office> Requesting_office { get; set; }
        public DbSet<Utilization> Utilization { get; set; }
        public DbSet<Yearly_reference> Yearly_reference { get; set; }
        public DbSet<Uacs> Uacs { get; set; }
        public DbSet<AllotmentClass> AllotmentClass { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Uacsamount> Uacsamount { get; set; }
        public DbSet<FundSourceAmount> FundSourceAmount { get; set; }
        public DbSet<FundsRealignment> FundsRealignment { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //1:M relationship to budget allotments and fundsources
            modelBuilder.Entity<FundSource>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSources);
            /*.HasForeignKey(p => p.BudgetAllotmentId);*/


            //1:M relationship to budget allotments and sub allotments
            modelBuilder.Entity<Sub_allotment>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.Sub_allotments);
            /*.HasForeignKey(p => p.BudgetAllotmentId);*/


            //1:M relationship
            modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.FundSource)
            .WithMany(b => b.FundSourceAmounts);


            modelBuilder.Entity<FundSource>()
           .HasKey(s => s.FundSourceId);


            //arnell

            modelBuilder.Entity<Personal_Information>()
           .HasKey(s => s.Pid);
            modelBuilder.Entity<Designation>()
           .HasKey(s => s.Did);

            modelBuilder.Entity<Requesting_office>()
            .HasOne<Personal_Information>(p => p.Personal_Information)
            .WithOne(s => s.Requesting_office);

            modelBuilder.Entity<Requesting_office>()
            .HasOne<Designation>(d => d.Designation)
            .WithOne(s => s.Requesting_office);

            //Yearylyref

            modelBuilder.Entity<Yearly_reference>()
           .HasKey(s => s.YearlyReferenceId);

            modelBuilder.Entity<Budget_allotment>()
            .HasOne<Yearly_reference>(d => d.Yearly_reference)
            .WithOne(s => s.Budget_allotment);




            modelBuilder.Entity<Budget_allotment>().ToTable("Budget_allotment");
            modelBuilder.Entity<FundSource>().ToTable("FundSource");
            /*modelBuilder.Entity<Yearly_reference>().ToTable("Yearly_reference");
            modelBuilder.Entity<Requesting_office>().ToTable("Requesting_office");*/

        }
    }
}
