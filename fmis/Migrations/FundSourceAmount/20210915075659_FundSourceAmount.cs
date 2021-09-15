using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.FundSourceAmount
{
    public partial class FundSourceAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundSourceAmount",
                columns: table => new
                {
                    FundsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSourceAmount", x => x.FundsId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundSourceAmount");
        }
    }
}
