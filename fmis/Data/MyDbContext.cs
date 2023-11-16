using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Models.John;
using fmis.Models.silver;
using fmis.Models.Carlo;
using fmis.Models.Accounting;
using System;
using fmis.Models.Budget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using fmis.Models.UserModels;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Xml;
using fmis.Models.pr;
using fmis.Models.ppmp;
using fmis.Models.Procurement;
using fmis.Models.Maiff;

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

        public DbSet<FmisUser> FmisUsers { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Sections> Sections { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Personal_Information> Personal_Information { get; set; }
        public DbSet<BudgetAllotment> Budget_allotments { get; set; }
        public DbSet<FundSource> FundSources { get; set; }
        public DbSet<Obligation> Obligation { get; set; }
        public DbSet<Prexc> Prexc { get; set; }
        public DbSet<Requesting_office> Requesting_office { get; set; }
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
        public DbSet<Category> Category { get; set; }
        public DbSet<IndexOfPayment> Indexofpayment { get; set; }
        public DbSet<Payee> Payee { get; set; }
        public DbSet<Deduction> Deduction { get; set; }
        public DbSet<Dv> Dv { get; set; }
        public DbSet<FundCluster> FundCluster { get; set; }
        public DbSet<FundTransferedTo> FundTransferedTo { get; set; }
        public DbSet<SubTransferedTo> SubTransferedTo { get; set; }
        //public DbSet<SectionsContext> Sections { get; set; }
        public DbSet<Assignee> Assignee { get; set; }
        public DbSet<IndexDeduction> IndexDeduction { get; set; }
        public DbSet<BillNumber> BillNumber { get; set; }
        public DbSet<SubNegative> SubNegative { get; set; }
        public DbSet<IndexFundSource> IndexFundSource { get; set; }
        public DbSet<InfraAdvancePayment> InfraAdvancePayment { get; set; }
        public DbSet<InfraRetention> InfraRetentions { get; set; }
        public DbSet<InfraProgress> InfraProgress { get; set; }
        public DbSet<Pr> Pr { get; set; }
        public DbSet<PrItems> PrItems { get; set; }

        public DbSet<Programs> Programs { get; set; }
        public DbSet<Expense> Expense { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<IndexUser> IndexUser { get; set; }
        public DbSet<AppModel> AppModel { get; set; }
        public DbSet<MaiffDv> MaiffDv { get; set; }



        public class MyEntityConfiguration : IEntityTypeConfiguration<InfraProgress>
        {
            public void Configure(EntityTypeBuilder<InfraProgress> builder)
            {
                builder.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 4)");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InfraRetention>()
                .HasOne(x => x.Dv)
                .WithMany(x => x.InfraRetentions)
                .HasForeignKey(x=>x.DvId);

            modelBuilder.ApplyConfiguration(new MyEntityConfiguration());


            /*modelBuilder.Entity<IndexOfPayment>()
            .HasMany(o => o.indexDeductions)
            .WithOne(i => i.IndexOfPayment)
            .HasForeignKey(i => i.IndexOfPaymentId);*/

            modelBuilder.Entity<FmisUser>().ToTable("FmisUser");
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

            modelBuilder.Entity<FundSource>().HasData(new FundSource { FundSourceId = 51, FundSourceTitle = "CANCELLED", RespoId = 1, PrexcId = 1, AllotmentClassId = 1,
                AppropriationId = 1, FundId = 1, Beginning_balance = (decimal)0.00, Remaining_balance = (decimal)0.00, obligated_amount = (decimal)0.00, utilized_amount = (decimal)0.00, realignment_amount = (decimal)0.00, IsAddToNextAllotment = false, Original = false, Breakdown = false });
            modelBuilder.Entity<SubAllotment>().HasData(new SubAllotment
            {
                SubAllotmentId = 138,
                Suballotment_title = "CANCELLED",
                RespoId = 1,
                prexcId = 1,
                AllotmentClassId = 1,
                AppropriationId = 1,
                FundId = 1,
                Beginning_balance = (decimal)0.00,
                Remaining_balance = (decimal)0.00,
                obligated_amount = (decimal)0.00,
                utilized_amount = (decimal)0.00,
                realignment_amount = (decimal)0.00,
                IsAddToNextAllotment = false,
                Original = false,
                Breakdown = false
            });

            modelBuilder.Entity<IndexFundSource>().HasData(
                    new IndexFundSource { Id = 1, Title = "Regular Agency Fund"},
                    new IndexFundSource { Id = 2, Title = "Foreign Assisted Project Fund" },
                    new IndexFundSource { Id = 3, Title = "Special Accounts - Locally Funded" },
                    new IndexFundSource { Id = 4, Title = "Special Accounts - Foreign Assisted/Grants" },
                    new IndexFundSource { Id = 5, Title = "Internally Generated Income" },
                    new IndexFundSource { Id = 6, Title = "Business Type Income" },
                    new IndexFundSource { Id = 7, Title = "Trust Fund" }           
            );
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
