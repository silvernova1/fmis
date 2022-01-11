using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class MyDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllotmentClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allotment_Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appropriation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appropriation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManageUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_no = table.Column<int>(type: "int", nullable: false),
                    Gross = table.Column<float>(type: "real", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: false),
                    Date_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_released = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_released = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ors_head",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Personalinfo_userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ors_head", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requesting_office",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pi_userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requesting_office", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Division = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_no = table.Column<int>(type: "int", nullable: false),
                    Fund_source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gross = table.Column<float>(type: "real", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: false),
                    Date_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_released = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_released = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yearly_reference",
                columns: table => new
                {
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearlyReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yearly_reference", x => x.YearlyReferenceId);
                });

            migrationBuilder.CreateTable(
                name: "ObligationAmount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObligationId = table.Column<int>(type: "int", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    Expense_code = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total_disbursement = table.Column<float>(type: "real", nullable: false),
                    Total_net_amount = table.Column<float>(type: "real", nullable: false),
                    Total_tax_amount = table.Column<float>(type: "real", nullable: false),
                    Total_others = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationAmount_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Designation",
                columns: table => new
                {
                    Did = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true),
                    Ors_headId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designation", x => x.Did);
                    table.ForeignKey(
                        name: "FK_Designation_Ors_head_Ors_headId",
                        column: x => x.Ors_headId,
                        principalTable: "Ors_head",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Designation_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAllotment",
                columns: table => new
                {
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allotment_series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    obiligate_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAllotment", x => x.BudgetAllotmentId);
                    table.ForeignKey(
                        name: "FK_BudgetAllotment_Yearly_reference_YearlyReferenceId",
                        column: x => x.YearlyReferenceId,
                        principalTable: "Yearly_reference",
                        principalColumn: "YearlyReferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personal_Information",
                columns: table => new
                {
                    Pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    division = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    section = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: true),
                    Ors_headId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Information", x => x.Pid);
                    table.ForeignKey(
                        name: "FK_Personal_Information_BudgetAllotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Ors_head_Ors_headId",
                        column: x => x.Ors_headId,
                        principalTable: "Ors_head",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundSource",
                columns: table => new
                {
                    FundSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundSourceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Respo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrexcId = table.Column<int>(type: "int", nullable: false),
                    Beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    obligated_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObligationId = table.Column<int>(type: "int", nullable: true),
                    SummaryReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSource", x => x.FundSourceId);
                    table.ForeignKey(
                        name: "FK_FundSource_BudgetAllotment_BudgetAllotmentId",
                        column: x => x.BudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSource_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundsRealignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Realignment_from = table.Column<int>(type: "int", nullable: false),
                    Realignment_to = table.Column<int>(type: "int", nullable: false),
                    Realignment_amount = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundsRealignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundsRealignment_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub_allotment",
                columns: table => new
                {
                    SubAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Suballotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suballotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsibility_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    prexcId = table.Column<int>(type: "int", nullable: false),
                    Remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    obligated_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObligationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_allotment", x => x.SubAllotmentId);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_BudgetAllotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suballotment_amount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    suballotment_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    suballotment_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suballotment_amount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suballotment_amount_Sub_allotment_SubAllotmentId",
                        column: x => x.SubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundSourceAmount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fundsource_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fundsource_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrexcId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSourceAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundSourceAmount_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Uacs",
                columns: table => new
                {
                    UacsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundSourceAmountId = table.Column<int>(type: "int", nullable: true),
                    ObligationId = table.Column<int>(type: "int", nullable: true),
                    Sub_allotmentSubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    Suballotment_amountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacs", x => x.UacsId);
                    table.ForeignKey(
                        name: "FK_Uacs_FundSourceAmount_FundSourceAmountId",
                        column: x => x.FundSourceAmountId,
                        principalTable: "FundSourceAmount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Sub_allotment_Sub_allotmentSubAllotmentId",
                        column: x => x.Sub_allotmentSubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Suballotment_amount_Suballotment_amountId",
                        column: x => x.Suballotment_amountId,
                        principalTable: "Suballotment_amount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundSourceUacs",
                columns: table => new
                {
                    FundSourceId = table.Column<int>(type: "int", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSourceUacs", x => new { x.FundSourceId, x.UacsId });
                    table.ForeignKey(
                        name: "FK_FundSourceUacs_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSourceUacs_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubAllotment_Realignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Realignment_from = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Realignment_to = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Realignment_amount = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    fundsource_id = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sub_allotmentSubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    UacsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAllotment_Realignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubAllotment_Realignment_Sub_allotment_Sub_allotmentSubAllotmentId",
                        column: x => x.Sub_allotmentSubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubAllotment_Realignment_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SummaryReports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    datefrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReport_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prexc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pap_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UacsId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SummaryReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prexc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prexc_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prexc_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotment_YearlyReferenceId",
                table: "BudgetAllotment",
                column: "YearlyReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designation_Ors_headId",
                table: "Designation",
                column: "Ors_headId");

            migrationBuilder.CreateIndex(
                name: "IX_Designation_Requesting_officeId",
                table: "Designation",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_BudgetAllotmentId",
                table: "FundSource",
                column: "BudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_ObligationId",
                table: "FundSource",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_PrexcId",
                table: "FundSource",
                column: "PrexcId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_SummaryReportId",
                table: "FundSource",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_PrexcId",
                table: "FundSourceAmount",
                column: "PrexcId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceUacs_UacsId",
                table: "FundSourceUacs",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_FundsRealignment_FundSourceId",
                table: "FundsRealignment",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationAmount_ObligationId",
                table: "ObligationAmount",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Ors_headId",
                table: "Personal_Information",
                column: "Ors_headId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Requesting_officeId",
                table: "Personal_Information",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_Prexc_SummaryReportId",
                table: "Prexc",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Prexc_UacsId",
                table: "Prexc",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_ObligationId",
                table: "Sub_allotment",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_prexcId",
                table: "Sub_allotment",
                column: "prexcId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suballotment_amount_SubAllotmentId",
                table: "Suballotment_amount",
                column: "SubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAllotment_Realignment_Sub_allotmentSubAllotmentId",
                table: "SubAllotment_Realignment",
                column: "Sub_allotmentSubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAllotment_Realignment_UacsId",
                table: "SubAllotment_Realignment",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReport_UacsId",
                table: "SummaryReport",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_FundSourceAmountId",
                table: "Uacs",
                column: "FundSourceAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_ObligationId",
                table: "Uacs",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_Sub_allotmentSubAllotmentId",
                table: "Uacs",
                column: "Sub_allotmentSubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_Suballotment_amountId",
                table: "Uacs",
                column: "Suballotment_amountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource",
                column: "PrexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_SummaryReport_SummaryReportId",
                table: "FundSource",
                column: "SummaryReportId",
                principalTable: "SummaryReport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_allotment_Prexc_prexcId",
                table: "Sub_allotment",
                column: "prexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_Prexc_PrexcId",
                table: "FundSourceAmount",
                column: "PrexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotment_Yearly_reference_YearlyReferenceId",
                table: "BudgetAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_BudgetAllotment_BudgetAllotmentId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_BudgetAllotment_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Obligation_ObligationId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Obligation_ObligationId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_Uacs_Obligation_ObligationId",
                table: "Uacs");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_Prexc_PrexcId",
                table: "FundSourceAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Prexc_prexcId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_SummaryReport_SummaryReportId",
                table: "FundSource");

            migrationBuilder.DropTable(
                name: "AllotmentClass");

            migrationBuilder.DropTable(
                name: "Appropriation");

            migrationBuilder.DropTable(
                name: "Designation");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "FundSourceUacs");

            migrationBuilder.DropTable(
                name: "FundsRealignment");

            migrationBuilder.DropTable(
                name: "ManageUsers");

            migrationBuilder.DropTable(
                name: "ObligationAmount");

            migrationBuilder.DropTable(
                name: "Personal_Information");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "SubAllotment_Realignment");

            migrationBuilder.DropTable(
                name: "Utilization");

            migrationBuilder.DropTable(
                name: "Ors_head");

            migrationBuilder.DropTable(
                name: "Requesting_office");

            migrationBuilder.DropTable(
                name: "Yearly_reference");

            migrationBuilder.DropTable(
                name: "BudgetAllotment");

            migrationBuilder.DropTable(
                name: "Obligation");

            migrationBuilder.DropTable(
                name: "Prexc");

            migrationBuilder.DropTable(
                name: "SummaryReport");

            migrationBuilder.DropTable(
                name: "Uacs");

            migrationBuilder.DropTable(
                name: "FundSourceAmount");

            migrationBuilder.DropTable(
                name: "Suballotment_amount");

            migrationBuilder.DropTable(
                name: "FundSource");

            migrationBuilder.DropTable(
                name: "Sub_allotment");
        }
    }
}
