using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class pap_title : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pap_title",
                table: "Prexc",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pap_title",
                table: "Prexc");
        }
    }
}
