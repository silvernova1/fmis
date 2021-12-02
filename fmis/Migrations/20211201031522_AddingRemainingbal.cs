using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class AddingRemainingbal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FundsTotal",
                table: "FundSource",
                newName: "Remainingbal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remainingbal",
                table: "FundSource",
                newName: "FundsTotal");
        }
    }
}
