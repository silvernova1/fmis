using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using System;
using fmis.Models.Budget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using fmis.Areas.Identity.Data;

namespace fmis.Data
{
    public class MyDbContext : IdentityDbContext<fmisUser>
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }


        public DbSet<Account> Account { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Personal_Information> Personal_Information { get; set; }
        public DbSet<BudgetAllotment> Budget_allotments { get; set; }
        public DbSet<FundSource> FundSources { get; set; }
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
        public DbSet<SubAllotment> SubAllotment { get; set; }
        public DbSet<Suballotment_amount> Suballotment_amount { get; set; }
        public DbSet<SubAllotment_Realignment> SubAllotment_Realignment { get; set; }
        public DbSet<Ors_head> Ors_head { get; set; }
        public DbSet<Appropriation> Appropriation { get; set; }
        public DbSet<ManageUsers> ManageUsers { get; set; }
        public DbSet<SummaryReport> SummaryReport { get; set; }
        public DbSet<UtilizationAmount> UtilizationAmount { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<RespoCenter> RespoCenter { get; set; }
        public DbSet<Dts> Dts { get; set; }
        public DbSet<Fund> Fund { get; set; }
        public DbSet<PapType> PapType { get; set; }
        public DbSet<UacsTrustFund> UacsTrustFund { get; set; }
        public DbSet<PrexcTrustFund> PrexcTrustFund { get; set; }
        public DbSet<RespoCenterTrustFund> RespoCenterTrustFund { get; set; }
        public DbSet<RequestingOfficeTrustFund> RequestingOfficeTrustFund { get; set; }
        public DbSet<BudgetAllotmentTrustFund> BudgetAllotmentTrustFund { get; set; }
        public DbSet<FundSourceTrustFund> FundSourceTrustFund { get; set; }
        public DbSet<FundSourceAmountTrustFund> FundSourceAmountTrustFund { get; set; }
        public DbSet<FundsRealignmentTrustFund> FundsRealignmentTrustFund { get; set; }
        public DbSet<ObligationTrustFund> ObligationTrustFund { get; set; }
        public DbSet<ObligationAmountTrustFund> ObligationAmountTrustFund { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);



            /*modelBuilder.Entity<BudgetAllotment>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("getdate()");*/

            /* modelBuilder.Entity<BudgetAllotment>()
                 .Property(b => b.CreatedAt)
                 .HasDefaultValueSql("getdate()");*/

            //1:M relationship to budget allotments and fundsources
            /*modelBuilder.Entity<FundSource>()
            .HasOne(p => p.BudgetAllotment)
            .WithMany(b => b.FundSources);*/


            //1:M relationship to budget allotments and fundsourceamounts
            /* modelBuilder.Entity<FundSourceAmount>()
             .HasOne(p => p.FundSource)
             .WithMany(b => b.FundSourceAmounts);*/

            //1:M relationship to budget allotments and fundsourceamounts
            /*modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.Budget_allotment)
            .WithMany(b => b.FundSourceAmounts);*/

            //1:M relationship to budget allotments and sub allotments


            //1:M relationship
            /*modelBuilder.Entity<FundSourceAmount>()
            .HasOne(p => p.FundSource)
            .WithMany(b => b.FundSourceAmounts);*/

            /*modelBuilder.Entity<FundSource>()
           .HasKey(s => s.FundSourceId);*/

            /*  //1:M relationship
              modelBuilder.Entity<Suballotment_amount>()
              .HasOne(p => p.Sub_allotment)
              .WithMany(b => b.Suballotment_amount);*/

            /*modelBuilder.Entity<Sub_allotment>()
           .HasKey(s => s.SubAllotmentId);*/


            //arnell
            /*modelBuilder.Entity<Personal_Information>()
           .HasKey(s => s.Pid);
            modelBuilder.Entity<ManageUsers>()
           .HasKey(s => s.Id);

            modelBuilder.Entity<Personal_Information>()
           .HasKey(s => s.Pid);
            modelBuilder.Entity<Requesting_office>()
           .HasKey(s => s.Id);*/

            //Yearylyref
            /* modelBuilder.Entity<Yearly_reference>()
            .HasKey(s => s.YearlyReferenceId);

             modelBuilder.Entity<BudgetAllotment>()
            .HasOne<Yearly_reference>(d => d.Yearly_reference)
            .WithOne(s => s.Budget_allotment);*/


            modelBuilder.Entity<BudgetAllotment>().ToTable("BudgetAllotment");
            modelBuilder.Entity<FundSource>().ToTable("FundSource");
            modelBuilder.Entity<FundSourceAmount>().ToTable("FundSourceAmount");
            modelBuilder.Entity<SubAllotment>().ToTable("SubAllotment");
            modelBuilder.Entity<FundsRealignment>().ToTable("FundsRealignment");
            modelBuilder.Entity<Yearly_reference>().ToTable("Yearly_reference");
            modelBuilder.Entity<SubAllotment_Realignment>().ToTable("SubAllotment_Realignment");
            modelBuilder.Entity<AllotmentClass>().ToTable("AllotmentClass");
            //PAP SEEDER
            modelBuilder.Entity<PapType>().HasData(new PapType { PapTypeID = 1, PapTypeName = "GAS" },
                                                            new PapType { PapTypeID = 2, PapTypeName = "STO" },
                                                            new PapType { PapTypeID = 3, PapTypeName = "OPERATION" },
                                                            new PapType { PapTypeID = 4, PapTypeName = "" });
            //APPROPRIATION SEEDER
            modelBuilder.Entity<Appropriation>().HasData(new Appropriation { AppropriationId = 1, AppropriationSource = "CURRENT", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                                                         new Appropriation { AppropriationId = 2, AppropriationSource = "CONAP", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
            //ALLOTMENTCLASS SEEDER
            modelBuilder.Entity<AllotmentClass>().HasData(new AllotmentClass { Id = 1, Allotment_Class = "PS", Account_Code = "100", Fund_Code = "01", Desc = "Personnel Services", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                                                         new AllotmentClass { Id = 2, Allotment_Class = "MOOE", Account_Code = "200", Fund_Code = "02", Desc = "Maintenance and Other Operating Expenses", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                                                         new AllotmentClass { Id = 3, Allotment_Class = "CO", Account_Code = "300", Fund_Code = "06", Desc = "Capital Outlay", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
            //FUND SEEDER
            modelBuilder.Entity<Fund>().HasData(new Fund { FundId = 1, Fund_description = "Specific Budget of National Government Agencies", Fund_code_current = "101101", Fund_code_conap = "102101" },
                new Fund { FundId = 2, Fund_description = "National Disaster Risk Reduction and Management Fund (Calamity Fund)", Fund_code_current = "101401", Fund_code_conap = "102401" },
                new Fund { FundId = 3, Fund_description = "Contingent Fund", Fund_code_current = "101402", Fund_code_conap = "102402" },
                new Fund { FundId = 4, Fund_description = "Miscellaneous Personnel Benefits Fund (MPBF)", Fund_code_current = "101406", Fund_code_conap = "101406" },
                new Fund { FundId = 5, Fund_description = "Pension and Gratuity Fund", Fund_code_current = "101407", Fund_code_conap = "102407" },
                new Fund { FundId = 6, Fund_description = "Retirement and Life Insurance Premium (RLIP)", Fund_code_current = "104102", Fund_code_conap = "" });
            //RESPO SEEDER
            modelBuilder.Entity<RespoCenter>().HasData(new RespoCenter { RespoId = 1, Respo = "MSD", RespoCode = "13-001-03-0007-2022-001", RespoHead = "ELIZABETH P. TABASA, CPA, MBA, CEO VI", RespoHeadPosition = "CHIEF - MANAGEMENT SUPPORT DIVISION" },
                                                       new RespoCenter { RespoId = 2, Respo = "LHSD", RespoCode = "13-001-03-0007-2022-002", RespoHead = "JONATHAN NEIL V. ERASMO, MD, MPH, FPSMS", RespoHeadPosition = "CHIEF - LOCAL HEALTH SUPPORT DIVISION" },
                                                       new RespoCenter { RespoId = 3, Respo = "RD/ARD", RespoCode = "13-001-03-0007-2022-003", RespoHead = "GUY R. PEREZ, MD, RPT, FPSMS, MBAHA, CESE", RespoHeadPosition = "DIRECTOR III" },
                                                       new RespoCenter { RespoId = 4, Respo = "RLED", RespoCode = "13-001-03-0007-2022-004", RespoHead = "SOPHIA M. MANCAO, MD, DPSP", RespoHeadPosition = "CHIEF - Regulation of Regional Health Facilities and Services" });
        }


        public static MyDbContext Create()
        {
            return new MyDbContext();
        }

        internal static object Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntityTimeStramp && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                //var now = DateTime.UtcNow; // current datetime
                var now = DateTime.Now;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntityTimeStramp)entity.Entity).CreatedAt = now;
                }
                ((BaseEntityTimeStramp)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
