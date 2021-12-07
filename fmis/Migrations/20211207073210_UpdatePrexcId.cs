using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class UpdatePrexcId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Prexc_Id",
                table: "FundSource");

            migrationBuilder.DropColumn(
                name: "PrexcCode",
                table: "FundSource");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FundSource",
                newName: "PrexcId");

            migrationBuilder.RenameIndex(
                name: "IX_FundSource_Id",
                table: "FundSource",
                newName: "IX_FundSource_PrexcId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource",
                column: "PrexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource");

            migrationBuilder.RenameColumn(
                name: "PrexcId",
                table: "FundSource",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_FundSource_PrexcId",
                table: "FundSource",
                newName: "IX_FundSource_Id");

            migrationBuilder.AddColumn<string>(
                name: "PrexcCode",
                table: "FundSource",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Prexc_Id",
                table: "FundSource",
                column: "Id",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
