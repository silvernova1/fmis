using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class Personal_Information : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_allotments_Yearly_reference_YearlyReferenceId",
                table: "Budget_allotments");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_FundSources_FundSourceId",
                table: "FundSourceAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSources_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "FundSources");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSources_Prexc_Id",
                table: "FundSources");

            migrationBuilder.DropForeignKey(
                name: "FK_Personal_Information_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FundSources",
                table: "FundSources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budget_allotments",
                table: "Budget_allotments");

            migrationBuilder.RenameTable(
                name: "FundSources",
                newName: "FundSource");

            migrationBuilder.RenameTable(
                name: "Budget_allotments",
                newName: "Budget_allotment");

            migrationBuilder.RenameIndex(
                name: "IX_FundSources_Id",
                table: "FundSource",
                newName: "IX_FundSource_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FundSources_Budget_allotmentBudgetAllotmentId",
                table: "FundSource",
                newName: "IX_FundSource_Budget_allotmentBudgetAllotmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_allotments_YearlyReferenceId",
                table: "Budget_allotment",
                newName: "IX_Budget_allotment_YearlyReferenceId");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Personal_Information",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FundSource",
                table: "FundSource",
                column: "FundSourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budget_allotment",
                table: "Budget_allotment",
                column: "BudgetAllotmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_allotment_Yearly_reference_YearlyReferenceId",
                table: "Budget_allotment",
                column: "YearlyReferenceId",
                principalTable: "Yearly_reference",
                principalColumn: "YearlyReferenceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "FundSource",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotment",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Prexc_Id",
                table: "FundSource",
                column: "Id",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId",
                principalTable: "FundSource",
                principalColumn: "FundSourceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personal_Information_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotment",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_allotment_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotment",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_allotment_Yearly_reference_YearlyReferenceId",
                table: "Budget_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Prexc_Id",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_Personal_Information_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FundSource",
                table: "FundSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budget_allotment",
                table: "Budget_allotment");

            migrationBuilder.DropColumn(
                name: "username",
                table: "Personal_Information");

            migrationBuilder.RenameTable(
                name: "FundSource",
                newName: "FundSources");

            migrationBuilder.RenameTable(
                name: "Budget_allotment",
                newName: "Budget_allotments");

            migrationBuilder.RenameIndex(
                name: "IX_FundSource_Id",
                table: "FundSources",
                newName: "IX_FundSources_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FundSource_Budget_allotmentBudgetAllotmentId",
                table: "FundSources",
                newName: "IX_FundSources_Budget_allotmentBudgetAllotmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_allotment_YearlyReferenceId",
                table: "Budget_allotments",
                newName: "IX_Budget_allotments_YearlyReferenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FundSources",
                table: "FundSources",
                column: "FundSourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budget_allotments",
                table: "Budget_allotments",
                column: "BudgetAllotmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_allotments_Yearly_reference_YearlyReferenceId",
                table: "Budget_allotments",
                column: "YearlyReferenceId",
                principalTable: "Yearly_reference",
                principalColumn: "YearlyReferenceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_FundSources_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId",
                principalTable: "FundSources",
                principalColumn: "FundSourceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSources_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "FundSources",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotments",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSources_Prexc_Id",
                table: "FundSources",
                column: "Id",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Personal_Information_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotments",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_allotment_Budget_allotments_Budget_allotmentBudgetAllotmentId",
                table: "Sub_allotment",
                column: "Budget_allotmentBudgetAllotmentId",
                principalTable: "Budget_allotments",
                principalColumn: "BudgetAllotmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
