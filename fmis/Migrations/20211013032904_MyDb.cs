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
                name: "FundsRealignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Realignment_from = table.Column<int>(type: "int", nullable: false),
                    Realignment_to = table.Column<int>(type: "int", nullable: false),
                    Realignment_amount = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundsRealignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligated_amount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Obligation_id = table.Column<int>(type: "int", nullable: false),
                    Expense_Title = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligated_amount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligation", x => x.Id);
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
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prexc", x => x.Id);
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
                name: "Uacsamount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObligationId = table.Column<int>(type: "int", nullable: false),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Total_disbursement = table.Column<float>(type: "real", nullable: false),
                    Total_net_amount = table.Column<float>(type: "real", nullable: false),
                    Total_tax_amount = table.Column<float>(type: "real", nullable: false),
                    Total_others = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacsamount", x => x.Id);
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
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Uacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Obligated_amountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uacs_Obligated_amount_Obligated_amountId",
                        column: x => x.Obligated_amountId,
                        principalTable: "Obligated_amount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designation", x => x.Did);
                    table.ForeignKey(
                        name: "FK_Designation_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budget_allotment",
                columns: table => new
                {
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allotment_series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget_allotment", x => x.BudgetAllotmentId);
                    table.ForeignKey(
                        name: "FK_Budget_allotment_Yearly_reference_YearlyReferenceId",
                        column: x => x.YearlyReferenceId,
                        principalTable: "Yearly_reference",
                        principalColumn: "YearlyReferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundSource",
                columns: table => new
                {
                    FundSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrexcCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Respo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSource", x => x.FundSourceId);
                    table.ForeignKey(
                        name: "FK_FundSource_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "Budget_allotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_Prexc_Id",
                        column: x => x.Id,
                        principalTable: "Prexc",
                        principalColumn: "Id",
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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Information", x => x.Pid);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "Budget_allotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub_allotment",
                columns: table => new
                {
                    SubId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prexe_code = table.Column<int>(type: "int", nullable: false),
                    Suballotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suballotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_head = table.Column<int>(type: "int", nullable: false),
                    Responsibility_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PId = table.Column<int>(type: "int", nullable: false),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_allotment", x => x.SubId);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "Budget_allotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Prexc_PId",
                        column: x => x.PId,
                        principalTable: "Prexc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundSourceAmount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    FundSourceId = table.Column<int>(type: "int", nullable: true)
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
                name: "Ors_head",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Head_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Pid = table.Column<int>(type: "int", nullable: false),
                    Did = table.Column<int>(type: "int", nullable: false),
                    Sub_AllotmentSubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ors_head", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ors_head_Designation_Did",
                        column: x => x.Did,
                        principalTable: "Designation",
                        principalColumn: "Did",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ors_head_Personal_Information_Pid",
                        column: x => x.Pid,
                        principalTable: "Personal_Information",
                        principalColumn: "Pid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ors_head_Sub_allotment_Sub_AllotmentSubId",
                        column: x => x.Sub_AllotmentSubId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suballotment_amount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Expenses = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Fund_source = table.Column<int>(type: "int", nullable: false),
                    Sub_allotmentSubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suballotment_amount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suballotment_amount_Sub_allotment_Sub_allotmentSubId",
                        column: x => x.Sub_allotmentSubId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budget_allotment_YearlyReferenceId",
                table: "Budget_allotment",
                column: "YearlyReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designation_Requesting_officeId",
                table: "Designation",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Budget_allotmentBudgetAllotmentId",
                table: "FundSource",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Id",
                table: "FundSource",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ors_head_Did",
                table: "Ors_head",
                column: "Did",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ors_head_Pid",
                table: "Ors_head",
                column: "Pid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ors_head_Sub_AllotmentSubId",
                table: "Ors_head",
                column: "Sub_AllotmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Requesting_officeId",
                table: "Personal_Information",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_PId",
                table: "Sub_allotment",
                column: "PId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suballotment_amount_Sub_allotmentSubId",
                table: "Suballotment_amount",
                column: "Sub_allotmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_Obligated_amountId",
                table: "Uacs",
                column: "Obligated_amountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllotmentClass");

            migrationBuilder.DropTable(
                name: "Appropriation");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "FundSourceAmount");

            migrationBuilder.DropTable(
                name: "FundsRealignment");

            migrationBuilder.DropTable(
                name: "Obligation");

            migrationBuilder.DropTable(
                name: "Ors_head");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "Suballotment_amount");

            migrationBuilder.DropTable(
                name: "Uacs");

            migrationBuilder.DropTable(
                name: "Uacsamount");

            migrationBuilder.DropTable(
                name: "Utilization");

            migrationBuilder.DropTable(
                name: "FundSource");

            migrationBuilder.DropTable(
                name: "Designation");

            migrationBuilder.DropTable(
                name: "Personal_Information");

            migrationBuilder.DropTable(
                name: "Sub_allotment");

            migrationBuilder.DropTable(
                name: "Obligated_amount");

            migrationBuilder.DropTable(
                name: "Requesting_office");

            migrationBuilder.DropTable(
                name: "Budget_allotment");

            migrationBuilder.DropTable(
                name: "Prexc");

            migrationBuilder.DropTable(
                name: "Yearly_reference");
        }
    }
}
