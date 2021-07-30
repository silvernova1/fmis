using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.BudgetAllotment
{
    public partial class BudgetAllotment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget_allotment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Allotment_Series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_Tittle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget_allotment", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget_allotment");
        }
    }
}
