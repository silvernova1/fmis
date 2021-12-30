using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.Carlo;
using fmis.Models.silver;
using System;

namespace fmis.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

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
        public DbSet<Requesting_office> Requesting_office { get; set; }
        public DbSet<Utilization> Utilization { get; set; }
        public DbSet<Yearly_reference> Yearly_reference { get; set; }
        public DbSet<Uacs> Uacs { get; set; }
        public DbSet<AllotmentClass> AllotmentClass { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<ObligationAmount> ObligationAmount { get; set; }
        public DbSet<FundSourceAmount> FundSourceAmount { get; set; }
        public DbSet<FundsRealignment> FundsRealignment { get; set; }
        public DbSet<Sub_allotment> Sub_allotment { get; set; }
        public DbSet<Suballotment_amount> Suballotment_amount { get; set; }
        public DbSet<SubAllotment_Realignment> SubAllotment_Realignment { get; set; }
        public DbSet<Ors_head> Ors_head { get; set; }
        public DbSet<Appropriation> Appropriation { get; set; }
        public DbSet<ManageUsers> ManageUsers { get; set; }
        public DbSet<SummaryReport> SummaryReport { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
                        //1:M relationship to uacs and funds realignment
                        modelBuilder.Entity<SubAllotment_Realignment>()
                        .HasOne(p => p.uacs)
                        .WithMany(b => b.SubAllotment_Realignment);*/

            //1:M relationship to budget allotments and fundsources
            modelBuilder.Entity<FundSource>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSources);


            

            //1:M relationship to budget allotments and fundsourceamounts
           /* modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.FundSource)
            .WithMany(b => b.FundSourceAmounts);*/

            //1:M relationship to budget allotments and fundsourceamounts
            /*modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSourceAmounts);*/

            //1:M relationship to budget allotments and sub allotments
            modelBuilder.Entity<Sub_allotment>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.Sub_allotments);


            //1:M relationship
            /*modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.FundSource)
            .WithMany(b => b.FundSourceAmounts);*/

            modelBuilder.Entity<FundSource>()
           .HasKey(s => s.FundSourceId);

          /*  //1:M relationship
            modelBuilder.Entity<Suballotment_amount>()
            .HasOne(p => p.Sub_allotment)
            .WithMany(b => b.Suballotment_amount);*/

            modelBuilder.Entity<Sub_allotment>()
           .HasKey(s => s.SubId);


            //arnell
            modelBuilder.Entity<Personal_Information>()
           .HasKey(s => s.Pid);
            modelBuilder.Entity<ManageUsers>()
           .HasKey(s => s.Id);

            modelBuilder.Entity<Personal_Information>()
           .HasKey(s => s.Pid);
            modelBuilder.Entity<Requesting_office>()
           .HasKey(s => s.Id);

            //Yearylyref
            modelBuilder.Entity<Yearly_reference>()
           .HasKey(s => s.YearlyReferenceId);

            modelBuilder.Entity<Budget_allotment>()
           .HasOne<Yearly_reference>(d => d.Yearly_reference)
           .WithOne(s => s.Budget_allotment);

            modelBuilder.Entity<Budget_allotment>().ToTable("Budget_allotment");
            modelBuilder.Entity<FundSource>().ToTable("FundSource");
            modelBuilder.Entity<FundSourceAmount>().ToTable("FundSourceAmount");
            modelBuilder.Entity<Sub_allotment>().ToTable("Sub_allotment");
            modelBuilder.Entity<FundsRealignment>().ToTable("FundsRealignment");
            modelBuilder.Entity<Yearly_reference>().ToTable("Yearly_reference");
            modelBuilder.Entity<SubAllotment_Realignment>().ToTable("SubAllotment_Realignment");

        }

        internal static object Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
