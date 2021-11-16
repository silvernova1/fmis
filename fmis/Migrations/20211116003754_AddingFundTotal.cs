using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class AddingFundTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "FundsTotal",
                table: "FundSource",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FundsTotal",
                table: "FundSource");
        }
    }
}
