using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class AddingRemBal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "RemainingBalAmount",
                table: "FundSourceAmount",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingBalAmount",
                table: "FundSourceAmount");
        }
    }
}
