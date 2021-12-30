using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class AddingRS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrexcId",
                table: "FundSourceAmount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_PrexcId",
                table: "FundSourceAmount",
                column: "PrexcId");

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
                name: "FK_FundSourceAmount_Prexc_PrexcId",
                table: "FundSourceAmount");

            migrationBuilder.DropIndex(
                name: "IX_FundSourceAmount_PrexcId",
                table: "FundSourceAmount");

            migrationBuilder.DropColumn(
                name: "PrexcId",
                table: "FundSourceAmount");
        }
    }
}
