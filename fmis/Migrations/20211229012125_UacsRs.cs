using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class UacsRs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount");

            migrationBuilder.AddColumn<int>(
                name: "FundSourceAmountId",
                table: "Uacs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FundSourceId",
                table: "FundSourceAmount",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_FundSourceAmountId",
                table: "Uacs",
                column: "FundSourceAmountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId",
                principalTable: "FundSource",
                principalColumn: "FundSourceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Uacs_FundSourceAmount_FundSourceAmountId",
                table: "Uacs",
                column: "FundSourceAmountId",
                principalTable: "FundSourceAmount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_Uacs_FundSourceAmount_FundSourceAmountId",
                table: "Uacs");

            migrationBuilder.DropIndex(
                name: "IX_Uacs_FundSourceAmountId",
                table: "Uacs");

            migrationBuilder.DropColumn(
                name: "FundSourceAmountId",
                table: "Uacs");

            migrationBuilder.AlterColumn<int>(
                name: "FundSourceId",
                table: "FundSourceAmount",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSourceAmount_FundSource_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId",
                principalTable: "FundSource",
                principalColumn: "FundSourceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
