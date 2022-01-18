using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class arnel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FundSourceId",
                table: "ObligationAmount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObligationAmount_FundSourceId",
                table: "ObligationAmount",
                column: "FundSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationAmount_FundSource_FundSourceId",
                table: "ObligationAmount",
                column: "FundSourceId",
                principalTable: "FundSource",
                principalColumn: "FundSourceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObligationAmount_FundSource_FundSourceId",
                table: "ObligationAmount");

            migrationBuilder.DropIndex(
                name: "IX_ObligationAmount_FundSourceId",
                table: "ObligationAmount");

            migrationBuilder.DropColumn(
                name: "FundSourceId",
                table: "ObligationAmount");
        }
    }
}
