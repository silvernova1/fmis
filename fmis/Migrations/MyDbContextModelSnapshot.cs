﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using fmis.Data;

namespace fmis.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("fmis.Models.Appropriation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Appropriation");
                });

            modelBuilder.Entity("fmis.Models.Budget_allotment", b =>
                {
                    b.Property<int>("BudgetAllotmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Allotment_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Allotment_series")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Allotment_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.Property<int>("YearlyReferenceId")
                        .HasColumnType("int");

                    b.HasKey("BudgetAllotmentId");

                    b.HasIndex("YearlyReferenceId")
                        .IsUnique();

                    b.ToTable("Budget_allotment");
                });

            modelBuilder.Entity("fmis.Models.Carlo.FundsRealignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Realignment_amount")
                        .HasColumnType("real");

                    b.Property<string>("Realignment_from")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Realignment_to")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UacsId")
                        .HasColumnType("int");

                    b.Property<int>("fundsource_id")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UacsId");

                    b.ToTable("FundsRealignment");
                });

            modelBuilder.Entity("fmis.Models.Designation", b =>
                {
                    b.Property<int>("Did")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("Ors_headId")
                        .HasColumnType("int");

                    b.Property<string>("Remember_Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Requesting_officeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Updated_At")
                        .HasColumnType("datetime2");

                    b.HasKey("Did");

                    b.HasIndex("Ors_headId");

                    b.HasIndex("Requesting_officeId");

                    b.ToTable("Designation");
                });

            modelBuilder.Entity("fmis.Models.Division", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Head")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remember_Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_At")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Division");
                });

            modelBuilder.Entity("fmis.Models.John.AllotmentClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Account_Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Allotment_Class")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Updated_At")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AllotmentClass");
                });

            modelBuilder.Entity("fmis.Models.John.FundSource", b =>
                {
                    b.Property<int>("FundSourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Budget_allotmentBudgetAllotmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FundSourceTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FundSourceTitleCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("PrexcCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Remainingbal")
                        .HasColumnType("real");

                    b.Property<string>("Respo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FundSourceId");

                    b.HasIndex("Budget_allotmentBudgetAllotmentId");

                    b.HasIndex("Id");

                    b.ToTable("FundSource");
                });

            modelBuilder.Entity("fmis.Models.John.FundSourceAmount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Account_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<int>("BudgetId")
                        .HasColumnType("int");

                    b.Property<int>("FundSourceId")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FundSourceAmount");
                });

            modelBuilder.Entity("fmis.Models.Obligated_amount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<int>("Expense_Title")
                        .HasColumnType("int");

                    b.Property<int>("Obligation_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Obligated_amount");
                });

            modelBuilder.Entity("fmis.Models.Obligation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Created_by")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date_recieved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date_released")
                        .HasColumnType("datetime2");

                    b.Property<string>("Dv")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fund_source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Gross")
                        .HasColumnType("real");

                    b.Property<int>("Ors_no")
                        .HasColumnType("int");

                    b.Property<string>("Particulars")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payee")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Po_no")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pr_no")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Time_recieved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Time_released")
                        .HasColumnType("datetime2");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Obligation");
                });

            modelBuilder.Entity("fmis.Models.Ors_head", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("Personalinfo_userid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Ors_head");
                });

            modelBuilder.Entity("fmis.Models.Personal_Information", b =>
                {
                    b.Property<int>("Pid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Budget_allotmentBudgetAllotmentId")
                        .HasColumnType("int");

                    b.Property<int?>("Ors_headId")
                        .HasColumnType("int");

                    b.Property<int?>("Requesting_officeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("designation")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("division")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("full_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("section")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("updated_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("userid")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Pid");

                    b.HasIndex("Budget_allotmentBudgetAllotmentId");

                    b.HasIndex("Ors_headId");

                    b.HasIndex("Requesting_officeId");

                    b.ToTable("Personal_Information");
                });

            modelBuilder.Entity("fmis.Models.Prexc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("pap_code1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pap_code2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pap_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Prexc");
                });

            modelBuilder.Entity("fmis.Models.Requesting_office", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("pi_userid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Requesting_office");
                });

            modelBuilder.Entity("fmis.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Division")
                        .HasColumnType("int");

                    b.Property<string>("Head")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remember_Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_At")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Section");
                });

            modelBuilder.Entity("fmis.Models.SubAllotment_Realignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Realignment_amount")
                        .HasColumnType("real");

                    b.Property<string>("Realignment_from")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Realignment_to")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UacsId")
                        .HasColumnType("int");

                    b.Property<int>("fundsource_id")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UacsId");

                    b.ToTable("SubAllotment_Realignment");
                });

            modelBuilder.Entity("fmis.Models.Sub_allotment", b =>
                {
                    b.Property<int>("SubId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Budget_allotmentBudgetAllotmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Prexc_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Remainbal")
                        .HasColumnType("real");

                    b.Property<string>("Responsibility_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Suballotment_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Suballotment_title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubId");

                    b.HasIndex("Budget_allotmentBudgetAllotmentId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Sub_allotment");
                });

            modelBuilder.Entity("fmis.Models.Suballotment_amount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<int>("BudgetId")
                        .HasColumnType("int");

                    b.Property<string>("Expenses")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FundSourceId")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Suballotment_amount");
                });

            modelBuilder.Entity("fmis.Models.Uacs", b =>
                {
                    b.Property<int>("UacsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Account_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Expense_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Obligated_amountId")
                        .HasColumnType("int");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UacsId");

                    b.HasIndex("Obligated_amountId");

                    b.ToTable("Uacs");
                });

            modelBuilder.Entity("fmis.Models.Uacsamount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Account_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<string>("Expense_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ObligationId")
                        .HasColumnType("int");

                    b.Property<float>("Total_disbursement")
                        .HasColumnType("real");

                    b.Property<float>("Total_net_amount")
                        .HasColumnType("real");

                    b.Property<float>("Total_others")
                        .HasColumnType("real");

                    b.Property<float>("Total_tax_amount")
                        .HasColumnType("real");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Uacsamount");
                });

            modelBuilder.Entity("fmis.Models.Utilization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Created_by")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date_recieved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date_released")
                        .HasColumnType("datetime2");

                    b.Property<string>("Dv")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fund_source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Gross")
                        .HasColumnType("real");

                    b.Property<int>("Ors_no")
                        .HasColumnType("int");

                    b.Property<string>("Particulars")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Po_no")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pr_no")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Time_recieved")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Time_released")
                        .HasColumnType("datetime2");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Utilization");
                });

            modelBuilder.Entity("fmis.Models.Yearly_reference", b =>
                {
                    b.Property<int>("YearlyReferenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("YearlyReference")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("YearlyReferenceId");

                    b.ToTable("Yearly_reference");
                });

            modelBuilder.Entity("fmis.Models.silver.ManageUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserRole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ManageUsers");
                });

            modelBuilder.Entity("fmis.Models.silver.SummaryReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SummaryReports")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UacsId")
                        .HasColumnType("int");

                    b.Property<DateTime>("datefrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("dateto")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("UacsId");

                    b.ToTable("SummaryReport");
                });

            modelBuilder.Entity("fmis.Models.Budget_allotment", b =>
                {
                    b.HasOne("fmis.Models.Yearly_reference", "Yearly_reference")
                        .WithOne("Budget_allotment")
                        .HasForeignKey("fmis.Models.Budget_allotment", "YearlyReferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Yearly_reference");
                });

            modelBuilder.Entity("fmis.Models.Carlo.FundsRealignment", b =>
                {
                    b.HasOne("fmis.Models.Uacs", "Uacs")
                        .WithMany("FundsRealignments")
                        .HasForeignKey("UacsId");

                    b.Navigation("Uacs");
                });

            modelBuilder.Entity("fmis.Models.Designation", b =>
                {
                    b.HasOne("fmis.Models.Ors_head", "Ors_head")
                        .WithMany()
                        .HasForeignKey("Ors_headId");

                    b.HasOne("fmis.Models.Requesting_office", "Requesting_office")
                        .WithMany()
                        .HasForeignKey("Requesting_officeId");

                    b.Navigation("Ors_head");

                    b.Navigation("Requesting_office");
                });

            modelBuilder.Entity("fmis.Models.John.FundSource", b =>
                {
                    b.HasOne("fmis.Models.Budget_allotment", "Budget_allotment")
                        .WithMany("FundSources")
                        .HasForeignKey("Budget_allotmentBudgetAllotmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fmis.Models.Prexc", "Prexc")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Budget_allotment");

                    b.Navigation("Prexc");
                });

            modelBuilder.Entity("fmis.Models.Personal_Information", b =>
                {
                    b.HasOne("fmis.Models.Budget_allotment", "Budget_allotment")
                        .WithMany("Personal_Information")
                        .HasForeignKey("Budget_allotmentBudgetAllotmentId");

                    b.HasOne("fmis.Models.Ors_head", "Ors_head")
                        .WithMany()
                        .HasForeignKey("Ors_headId");

                    b.HasOne("fmis.Models.Requesting_office", "Requesting_office")
                        .WithMany()
                        .HasForeignKey("Requesting_officeId");

                    b.Navigation("Budget_allotment");

                    b.Navigation("Ors_head");

                    b.Navigation("Requesting_office");
                });

            modelBuilder.Entity("fmis.Models.SubAllotment_Realignment", b =>
                {
                    b.HasOne("fmis.Models.Uacs", "uacs")
                        .WithMany("SubAllotment_Realignment")
                        .HasForeignKey("UacsId");

                    b.Navigation("uacs");
                });

            modelBuilder.Entity("fmis.Models.Sub_allotment", b =>
                {
                    b.HasOne("fmis.Models.Budget_allotment", "Budget_allotment")
                        .WithMany("Sub_allotments")
                        .HasForeignKey("Budget_allotmentBudgetAllotmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fmis.Models.Prexc", "Prexc")
                        .WithOne("Sub_Allotment")
                        .HasForeignKey("fmis.Models.Sub_allotment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Budget_allotment");

                    b.Navigation("Prexc");
                });

            modelBuilder.Entity("fmis.Models.Uacs", b =>
                {
                    b.HasOne("fmis.Models.Obligated_amount", null)
                        .WithMany("Uacs")
                        .HasForeignKey("Obligated_amountId");
                });

            modelBuilder.Entity("fmis.Models.silver.SummaryReport", b =>
                {
                    b.HasOne("fmis.Models.Uacs", "Uacs")
                        .WithMany()
                        .HasForeignKey("UacsId");

                    b.Navigation("Uacs");
                });

            modelBuilder.Entity("fmis.Models.Budget_allotment", b =>
                {
                    b.Navigation("FundSources");

                    b.Navigation("Personal_Information");

                    b.Navigation("Sub_allotments");
                });

            modelBuilder.Entity("fmis.Models.Obligated_amount", b =>
                {
                    b.Navigation("Uacs");
                });

            modelBuilder.Entity("fmis.Models.Prexc", b =>
                {
                    b.Navigation("Sub_Allotment");
                });

            modelBuilder.Entity("fmis.Models.Uacs", b =>
                {
                    b.Navigation("FundsRealignments");

                    b.Navigation("SubAllotment_Realignment");
                });

            modelBuilder.Entity("fmis.Models.Yearly_reference", b =>
                {
                    b.Navigation("Budget_allotment");
                });
#pragma warning restore 612, 618
        }
    }
}
