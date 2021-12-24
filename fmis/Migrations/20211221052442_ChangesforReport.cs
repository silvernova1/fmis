using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class ChangesforReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "FundSource",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code2",
                table: "Budget_allotment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "active",
                table: "Budget_allotment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "year",
                table: "Budget_allotment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "FundSource");

            migrationBuilder.DropColumn(
                name: "Code2",
                table: "Budget_allotment");

            migrationBuilder.DropColumn(
                name: "active",
                table: "Budget_allotment");

            migrationBuilder.DropColumn(
                name: "year",
                table: "Budget_allotment");
        }
    }
}
