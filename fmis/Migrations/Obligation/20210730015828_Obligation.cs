using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.Obligation
{
    public partial class Obligation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Obligation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_no = table.Column<int>(type: "int", nullable: false),
                    Fund_source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gross = table.Column<float>(type: "real", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: false),
                    Date_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_recieved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_released = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_released = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obligation");
        }
    }
}
