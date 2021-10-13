﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using fmis.Data;

namespace fmis.Migrations.MyDb
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20211008030043_MyDb")]
    partial class MyDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Remember_Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_At")
                        .HasColumnType("datetime2");

                    b.HasKey("Did");

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

                    b.Property<int?>("FundSourceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FundSourceId");

                    b.ToTable("FundSourceAmount");
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

                    b.Property<int>("Did")
                        .HasColumnType("int");

                    b.Property<string>("Head_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pid")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Sub_AllotmentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Did")
                        .IsUnique();

                    b.HasIndex("Pid")
                        .IsUnique();

                    b.HasIndex("Sub_AllotmentId");

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

                    b.Property<string>("PBarangay")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PMunicipality")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PProvince")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PStreet")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PSubdivision")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PZip_code")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phouse_no")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Psitio")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RBarangay")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RHouse_no")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RMunicipality")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RProvince")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RStreet")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RSubdivision")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("RZip_code")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Rsitio")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("account_number")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("bbalance_cto")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("blood_type")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("case_address")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("case_contact")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("case_name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("cellno")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("citizenship")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("civil_status")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("date_of_birth")
                        .HasColumnType("datetime2");

                    b.Property<int?>("designation_id")
                        .HasColumnType("int");

                    b.Property<string>("disbursement_type")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("division_id")
                        .HasColumnType("int");

                    b.Property<string>("emall_address")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("employee_status")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("field_status")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("fname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("gsis_idno")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("gsis_polnno")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("height")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("inactive_area")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("indicate_country")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("job_status")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("lname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("mname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("name_ext")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("pagibig_no")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("phic_no")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("picture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("place_of_birth")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("region")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("region_zip")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("residential_address")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("residential_municipality")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("residential_province")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("resigned_effectivity")
                        .HasColumnType("datetime2");

                    b.Property<string>("salary_charge")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("sched")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("section_id")
                        .HasColumnType("int");

                    b.Property<string>("sex")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("sick_balance")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("signature")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("sss_no")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("telno")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("tin_no")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("updated_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("userid")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("vacation_balance")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("weight")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Pid");

                    b.HasIndex("Budget_allotmentBudgetAllotmentId");

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

                    b.Property<int>("Did")
                        .HasColumnType("int");

                    b.Property<string>("Head_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pid")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Did")
                        .IsUnique();

                    b.HasIndex("Pid")
                        .IsUnique();

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

            modelBuilder.Entity("fmis.Models.Sub_allotment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Budget_allotmentBudgetAllotmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ors_head")
                        .HasColumnType("int");

                    b.Property<int>("PId")
                        .HasColumnType("int");

                    b.Property<int>("Prexe_code")
                        .HasColumnType("int");

                    b.Property<string>("Responsibility_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Suballotment_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Suballotment_title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Budget_allotmentBudgetAllotmentId");

                    b.HasIndex("PId")
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

                    b.Property<int>("Expenses")
                        .HasColumnType("int");

                    b.Property<int>("Fund_source")
                        .HasColumnType("int");

                    b.Property<int?>("Sub_allotmentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Sub_allotmentId");

                    b.ToTable("Suballotment_amount");
                });

            modelBuilder.Entity("fmis.Models.Uacs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Account_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Expense_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

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

            modelBuilder.Entity("fmis.Models.Budget_allotment", b =>
                {
                    b.HasOne("fmis.Models.Yearly_reference", "Yearly_reference")
                        .WithOne("Budget_allotment")
                        .HasForeignKey("fmis.Models.Budget_allotment", "YearlyReferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Yearly_reference");
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

            modelBuilder.Entity("fmis.Models.John.FundSourceAmount", b =>
                {
                    b.HasOne("fmis.Models.John.FundSource", "FundSource")
                        .WithMany("FundSourceAmounts")
                        .HasForeignKey("FundSourceId");

                    b.Navigation("FundSource");
                });

            modelBuilder.Entity("fmis.Models.Ors_head", b =>
                {
                    b.HasOne("fmis.Models.Designation", "Designation")
                        .WithOne("Ors_head")
                        .HasForeignKey("fmis.Models.Ors_head", "Did")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fmis.Models.Personal_Information", "Personal_Information")
                        .WithOne("Ors_head")
                        .HasForeignKey("fmis.Models.Ors_head", "Pid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fmis.Models.Sub_allotment", "Sub_Allotment")
                        .WithMany()
                        .HasForeignKey("Sub_AllotmentId");

                    b.Navigation("Designation");

                    b.Navigation("Personal_Information");

                    b.Navigation("Sub_Allotment");
                });

            modelBuilder.Entity("fmis.Models.Personal_Information", b =>
                {
                    b.HasOne("fmis.Models.Budget_allotment", "Budget_allotment")
                        .WithMany("Personal_Information")
                        .HasForeignKey("Budget_allotmentBudgetAllotmentId");

                    b.Navigation("Budget_allotment");
                });

            modelBuilder.Entity("fmis.Models.Requesting_office", b =>
                {
                    b.HasOne("fmis.Models.Designation", "Designation")
                        .WithOne("Requesting_office")
                        .HasForeignKey("fmis.Models.Requesting_office", "Did")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fmis.Models.Personal_Information", "Personal_Information")
                        .WithOne("Requesting_office")
                        .HasForeignKey("fmis.Models.Requesting_office", "Pid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Designation");

                    b.Navigation("Personal_Information");
                });

            modelBuilder.Entity("fmis.Models.Sub_allotment", b =>
                {
                    b.HasOne("fmis.Models.Budget_allotment", "Budget_allotment")
                        .WithMany("Sub_allotments")
                        .HasForeignKey("Budget_allotmentBudgetAllotmentId");

                    b.HasOne("fmis.Models.Prexc", "Prexc")
                        .WithOne("Sub_Allotment")
                        .HasForeignKey("fmis.Models.Sub_allotment", "PId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Budget_allotment");

                    b.Navigation("Prexc");
                });

            modelBuilder.Entity("fmis.Models.Suballotment_amount", b =>
                {
                    b.HasOne("fmis.Models.Sub_allotment", "Sub_allotment")
                        .WithMany("Suballotment_amount")
                        .HasForeignKey("Sub_allotmentId");

                    b.Navigation("Sub_allotment");
                });

            modelBuilder.Entity("fmis.Models.Budget_allotment", b =>
                {
                    b.Navigation("FundSources");

                    b.Navigation("Personal_Information");

                    b.Navigation("Sub_allotments");
                });

            modelBuilder.Entity("fmis.Models.Designation", b =>
                {
                    b.Navigation("Ors_head");

                    b.Navigation("Requesting_office");
                });

            modelBuilder.Entity("fmis.Models.John.FundSource", b =>
                {
                    b.Navigation("FundSourceAmounts");
                });

            modelBuilder.Entity("fmis.Models.Personal_Information", b =>
                {
                    b.Navigation("Ors_head");

                    b.Navigation("Requesting_office");
                });

            modelBuilder.Entity("fmis.Models.Prexc", b =>
                {
                    b.Navigation("Sub_Allotment");
                });

            modelBuilder.Entity("fmis.Models.Sub_allotment", b =>
                {
                    b.Navigation("Suballotment_amount");
                });

            modelBuilder.Entity("fmis.Models.Yearly_reference", b =>
                {
                    b.Navigation("Budget_allotment");
                });
#pragma warning restore 612, 618
        }
    }
}