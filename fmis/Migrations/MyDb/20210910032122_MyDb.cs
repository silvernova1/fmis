using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.MyDb
{
    public partial class MyDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget_allotment",
                columns: table => new
                {
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget_allotment", x => x.BudgetAllotmentId);
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
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prexc", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Budget_allotmentBudgetAllotmentId",
                table: "FundSource",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Id",
                table: "FundSource",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundSource");

            migrationBuilder.DropTable(
                name: "Budget_allotment");

            migrationBuilder.DropTable(
                name: "Prexc");
        }
    }
}
