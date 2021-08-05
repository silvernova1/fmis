using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.Uacs
{
    public partial class Uacs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<int>(type: "int", nullable: false),
                    Created_at = table.Column<int>(type: "int", nullable: false),
                    Updated_at = table.Column<int>(type: "int", nullable: false),
                    Date_recieved = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Uacs");
        }
    }
}
