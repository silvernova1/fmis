using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "FundSourceAmount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "FundSourceAmount",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "FundSourceAmount");

            migrationBuilder.DropColumn(
                name: "token",
                table: "FundSourceAmount");
        }
    }
}
