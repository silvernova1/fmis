using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class Addingapro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Desc",
                table: "AllotmentClass",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fund_Code",
                table: "AllotmentClass",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desc",
                table: "AllotmentClass");

            migrationBuilder.DropColumn(
                name: "Fund_Code",
                table: "AllotmentClass");
        }
    }
}
