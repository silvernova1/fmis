using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.Prexc
{
    public partial class Prexc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prexc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pap_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prexc", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prexc");
        }
    }
}
