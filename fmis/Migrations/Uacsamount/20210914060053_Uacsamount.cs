using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.Uacsamount
{
    public partial class Uacsamount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uacsamount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObligationId = table.Column<int>(type: "int", nullable: false),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Total_disbursement = table.Column<float>(type: "real", nullable: false),
                    Total_net_amount = table.Column<float>(type: "real", nullable: false),
                    Total_tax_amount = table.Column<float>(type: "real", nullable: false),
                    Total_others = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacsamount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Uacsamount");
        }
    }
}
