using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations.MyDb
{
    public partial class MyDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget_allotment",
                columns: table => new
                {
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget_allotment", x => x.BudgetAllotmentId);
                });

            migrationBuilder.CreateTable(
                name: "Personal_information",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    signature = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    fname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    lname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    mname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    name_ext = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    place_of_birth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    civil_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    citizenship = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    indicate_country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    height = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    weight = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    blood_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gsis_idno = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gsis_polnno = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    pagibig_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phic_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    sss_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    tin_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    residential_address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    residential_municipality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    residential_province = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RHouse_no = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RStreet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RSubdivision = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RBarangay = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RMunicipality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RProvince = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phouse_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PStreet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PSubdivision = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PBarangay = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PMunicipality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PProvince = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RZip_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PZip_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    region_zip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    telno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    emall_address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cellno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    employee_status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    job_status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    inactive_area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    case_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    case_address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    case_contact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    designation_id = table.Column<int>(type: "int", nullable: true),
                    division_id = table.Column<int>(type: "int", nullable: true),
                    section_id = table.Column<int>(type: "int", nullable: true),
                    disbursement_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    salary_charge = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bbalance_cto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vacation_balance = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sick_balance = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sched = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    account_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    field_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Rsitio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    resigned_effectivity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Psitio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_information", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "Requesting_office",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Head_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Pid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requesting_office", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requesting_office_Personal_information_Pid",
                        column: x => x.Pid,
                        principalTable: "Personal_information",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    Respo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSource", x => x.FundSourceId);
                    table.ForeignKey(
                        name: "FK_FundSource_Budget_allotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "Budget_allotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_Prexc_Id",
                        column: x => x.Id,
                        principalTable: "Prexc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Budget_allotmentBudgetAllotmentId",
                table: "FundSource",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_Id",
                table: "FundSource",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requesting_office_Pid",
                table: "Requesting_office",
                column: "Pid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundSource");

            migrationBuilder.DropTable(
                name: "Requesting_office");

            migrationBuilder.DropTable(
                name: "Budget_allotment");

            migrationBuilder.DropTable(
                name: "Prexc");

            migrationBuilder.DropTable(
                name: "Personal_information");
        }
    }
}
