using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class AddBbalanceRbalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remainingbal",
                table: "FundSource",
                newName: "Remaining_balance");

            migrationBuilder.AddColumn<float>(
                name: "Beginning_balance",
                table: "FundSource",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beginning_balance",
                table: "FundSource");

            migrationBuilder.RenameColumn(
                name: "Remaining_balance",
                table: "FundSource",
                newName: "Remainingbal");
        }
    }
}
