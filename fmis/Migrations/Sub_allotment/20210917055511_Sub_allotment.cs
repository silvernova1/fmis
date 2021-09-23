using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.Sub_allotment
{
    public partial class Sub_allotment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sub_allotment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prexe_code = table.Column<int>(type: "int", nullable: false),
                    Suballotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suballotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_head = table.Column<int>(type: "int", nullable: false),
                    Responsibility_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_allotment", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sub_allotment");
        }
    }
}
