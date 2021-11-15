using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class relationshiptobudgetallotmentsandfundsource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotment",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount");

            migrationBuilder.DropIndex(
                name: "IX_FundSourceAmount_Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount");

            migrationBuilder.DropColumn(
                name: "Budget_allotmentBudgetAllotmentId",
                table: "FundSourceAmount");
        }
    }
}
