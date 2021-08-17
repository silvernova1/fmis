using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.FundSource
{
    public partial class FundSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundSource",
                columns: table => new
                {
                    FundSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrexcCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Respo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSource", x => x.FundSourceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundSource");
        }
    }
}
