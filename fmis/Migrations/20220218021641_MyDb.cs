﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Migrations
{
    public partial class MyDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllotmentClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allotment_Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fund_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllotmentClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appropriation",
                columns: table => new
                {
                    AppropriationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppropriationSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appropriation", x => x.AppropriationId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dts",
                columns: table => new
                {
                    DtsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dts", x => x.DtsId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_id = table.Column<int>(type: "int", nullable: false),
                    created_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: false),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logs_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogsId);
                });

            migrationBuilder.CreateTable(
                name: "ManageUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RememberMe = table.Column<bool>(type: "bit", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Obligation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_no = table.Column<int>(type: "int", nullable: false),
                    Gross = table.Column<float>(type: "real", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obligation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ors_head",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Personalinfo_userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ors_head", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PapType",
                columns: table => new
                {
                    PapTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PapTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PapType", x => x.PapTypeID);
                });

            migrationBuilder.CreateTable(
                name: "PrexcTrustFund",
                columns: table => new
                {
                    PrexcTrustFundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pap_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrexcTrustFund", x => x.PrexcTrustFundId);
                });

            migrationBuilder.CreateTable(
                name: "Requesting_office",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pi_userid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requesting_office", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestingOfficeTrustFund",
                columns: table => new
                {
                    HeadnameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Headname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Headinformation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestingOfficeTrustFund", x => x.HeadnameId);
                });

            migrationBuilder.CreateTable(
                name: "RespoCenter",
                columns: table => new
                {
                    RespoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Respo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespoCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespoHead = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespoHeadPosition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespoCenter", x => x.RespoId);
                });

            migrationBuilder.CreateTable(
                name: "RespoCenterTrustFund",
                columns: table => new
                {
                    RespocentertrustfundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Respocentertrustfund = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespocentertrustfundCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespoCenterTrustFund", x => x.RespocentertrustfundId);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Division = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Head = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UacsTrustFund",
                columns: table => new
                {
                    UacsTrustFundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uacs_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UacsTrustFund", x => x.UacsTrustFundId);
                });

            migrationBuilder.CreateTable(
                name: "Utilization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pr_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Po_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ors_no = table.Column<int>(type: "int", nullable: false),
                    Gross = table.Column<float>(type: "real", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created_by = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    utilization_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yearly_reference",
                columns: table => new
                {
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearlyReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yearly_reference", x => x.YearlyReferenceId);
                });

            migrationBuilder.CreateTable(
                name: "Fund",
                columns: table => new
                {
                    FundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fund_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fund_code_current = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fund_code_conap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppropriationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fund", x => x.FundId);
                    table.ForeignKey(
                        name: "FK_Fund_Appropriation_AppropriationID",
                        column: x => x.AppropriationID,
                        principalTable: "Appropriation",
                        principalColumn: "AppropriationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Designation",
                columns: table => new
                {
                    Did = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remember_Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true),
                    Ors_headId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designation", x => x.Did);
                    table.ForeignKey(
                        name: "FK_Designation_Ors_head_Ors_headId",
                        column: x => x.Ors_headId,
                        principalTable: "Ors_head",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Designation_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAllotmentTrustFund",
                columns: table => new
                {
                    BudgetAllotmentTrustFundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAllotmentTrustFund", x => x.BudgetAllotmentTrustFundId);
                    table.ForeignKey(
                        name: "FK_BudgetAllotmentTrustFund_Yearly_reference_YearlyReferenceId",
                        column: x => x.YearlyReferenceId,
                        principalTable: "Yearly_reference",
                        principalColumn: "YearlyReferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAllotment",
                columns: table => new
                {
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Allotment_series = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearlyReferenceId = table.Column<int>(type: "int", nullable: false),
                    AllotmentClassId = table.Column<int>(type: "int", nullable: true),
                    AppropriationId = table.Column<int>(type: "int", nullable: true),
                    FundId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAllotment", x => x.BudgetAllotmentId);
                    table.ForeignKey(
                        name: "FK_BudgetAllotment_AllotmentClass_AllotmentClassId",
                        column: x => x.AllotmentClassId,
                        principalTable: "AllotmentClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAllotment_Appropriation_AppropriationId",
                        column: x => x.AppropriationId,
                        principalTable: "Appropriation",
                        principalColumn: "AppropriationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAllotment_Fund_FundId",
                        column: x => x.FundId,
                        principalTable: "Fund",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAllotment_Yearly_reference_YearlyReferenceId",
                        column: x => x.YearlyReferenceId,
                        principalTable: "Yearly_reference",
                        principalColumn: "YearlyReferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personal_Information",
                columns: table => new
                {
                    Pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    division = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    section = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requesting_officeId = table.Column<int>(type: "int", nullable: true),
                    Budget_allotmentBudgetAllotmentId = table.Column<int>(type: "int", nullable: true),
                    Ors_headId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Information", x => x.Pid);
                    table.ForeignKey(
                        name: "FK_Personal_Information_BudgetAllotment_Budget_allotmentBudgetAllotmentId",
                        column: x => x.Budget_allotmentBudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Ors_head_Ors_headId",
                        column: x => x.Ors_headId,
                        principalTable: "Ors_head",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personal_Information_Requesting_office_Requesting_officeId",
                        column: x => x.Requesting_officeId,
                        principalTable: "Requesting_office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundSource",
                columns: table => new
                {
                    FundSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundSourceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceTitleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespoId = table.Column<int>(type: "int", nullable: false),
                    PrexcId = table.Column<int>(type: "int", nullable: false),
                    PapTypeID = table.Column<int>(type: "int", nullable: true),
                    AllotmentClassId = table.Column<int>(type: "int", nullable: false),
                    AppropriationId = table.Column<int>(type: "int", nullable: false),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    Beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    obligated_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    utilized_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: true),
                    BudgetAllotmentTrustFundId = table.Column<int>(type: "int", nullable: true),
                    ObligationId = table.Column<int>(type: "int", nullable: true),
                    SummaryReportId = table.Column<int>(type: "int", nullable: true),
                    UtilizationId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSource", x => x.FundSourceId);
                    table.ForeignKey(
                        name: "FK_FundSource_AllotmentClass_AllotmentClassId",
                        column: x => x.AllotmentClassId,
                        principalTable: "AllotmentClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_Appropriation_AppropriationId",
                        column: x => x.AppropriationId,
                        principalTable: "Appropriation",
                        principalColumn: "AppropriationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_BudgetAllotment_BudgetAllotmentId",
                        column: x => x.BudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSource_BudgetAllotmentTrustFund_BudgetAllotmentTrustFundId",
                        column: x => x.BudgetAllotmentTrustFundId,
                        principalTable: "BudgetAllotmentTrustFund",
                        principalColumn: "BudgetAllotmentTrustFundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSource_Fund_FundId",
                        column: x => x.FundId,
                        principalTable: "Fund",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSource_PapType_PapTypeID",
                        column: x => x.PapTypeID,
                        principalTable: "PapType",
                        principalColumn: "PapTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSource_RespoCenter_RespoId",
                        column: x => x.RespoId,
                        principalTable: "RespoCenter",
                        principalColumn: "RespoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundSource_Utilization_UtilizationId",
                        column: x => x.UtilizationId,
                        principalTable: "Utilization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub_allotment",
                columns: table => new
                {
                    SubAllotmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Suballotment_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suballotment_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespoId = table.Column<int>(type: "int", nullable: false),
                    prexcId = table.Column<int>(type: "int", nullable: false),
                    AppropriationId = table.Column<int>(type: "int", nullable: false),
                    AllotmentClassId = table.Column<int>(type: "int", nullable: false),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    Remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    obligated_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    utilized_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: true),
                    ObligationId = table.Column<int>(type: "int", nullable: true),
                    UtilizationId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_allotment", x => x.SubAllotmentId);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_AllotmentClass_AllotmentClassId",
                        column: x => x.AllotmentClassId,
                        principalTable: "AllotmentClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Appropriation_AppropriationId",
                        column: x => x.AppropriationId,
                        principalTable: "Appropriation",
                        principalColumn: "AppropriationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_BudgetAllotment_BudgetAllotmentId",
                        column: x => x.BudgetAllotmentId,
                        principalTable: "BudgetAllotment",
                        principalColumn: "BudgetAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Fund_FundId",
                        column: x => x.FundId,
                        principalTable: "Fund",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_RespoCenter_RespoId",
                        column: x => x.RespoId,
                        principalTable: "RespoCenter",
                        principalColumn: "RespoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_allotment_Utilization_UtilizationId",
                        column: x => x.UtilizationId,
                        principalTable: "Utilization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObligationAmount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObligationId = table.Column<int>(type: "int", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    Expense_code = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total_disbursement = table.Column<float>(type: "real", nullable: false),
                    Total_net_amount = table.Column<float>(type: "real", nullable: false),
                    Total_tax_amount = table.Column<float>(type: "real", nullable: false),
                    Total_others = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    obligation_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationAmount_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObligationAmount_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObligationAmount_Sub_allotment_SubAllotmentId",
                        column: x => x.SubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Uacs",
                columns: table => new
                {
                    UacsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expense_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uacs_type = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    ObligationId = table.Column<int>(type: "int", nullable: true),
                    Sub_allotmentSubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    UtilizationId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uacs", x => x.UacsId);
                    table.ForeignKey(
                        name: "FK_Uacs_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Obligation_ObligationId",
                        column: x => x.ObligationId,
                        principalTable: "Obligation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Sub_allotment_Sub_allotmentSubAllotmentId",
                        column: x => x.Sub_allotmentSubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uacs_Utilization_UtilizationId",
                        column: x => x.UtilizationId,
                        principalTable: "Utilization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UtilizationAmount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizationId = table.Column<int>(type: "int", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    Expense_code = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total_disbursement = table.Column<float>(type: "real", nullable: false),
                    Total_net_amount = table.Column<float>(type: "real", nullable: false),
                    Total_tax_amount = table.Column<float>(type: "real", nullable: false),
                    Total_others = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    utilization_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    utilization_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizationAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizationAmount_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UtilizationAmount_Sub_allotment_SubAllotmentId",
                        column: x => x.SubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UtilizationAmount_Utilization_UtilizationId",
                        column: x => x.UtilizationId,
                        principalTable: "Utilization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suballotment_amount",
                columns: table => new
                {
                    SubAllotmentAmountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    suballotment_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    suballotment_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suballotment_amount", x => x.SubAllotmentAmountId);
                    table.ForeignKey(
                        name: "FK_Suballotment_amount_Sub_allotment_SubAllotmentId",
                        column: x => x.SubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suballotment_amount_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SummaryReports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    datefrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UacsId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryReport_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubAllotment_Realignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubAllotmentAmountId = table.Column<int>(type: "int", nullable: true),
                    Realignment_to = table.Column<int>(type: "int", nullable: false),
                    Realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAllotmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAllotment_Realignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubAllotment_Realignment_Sub_allotment_SubAllotmentId",
                        column: x => x.SubAllotmentId,
                        principalTable: "Sub_allotment",
                        principalColumn: "SubAllotmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubAllotment_Realignment_Suballotment_amount_SubAllotmentAmountId",
                        column: x => x.SubAllotmentAmountId,
                        principalTable: "Suballotment_amount",
                        principalColumn: "SubAllotmentAmountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prexc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pap_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_code1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pap_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UacsId = table.Column<int>(type: "int", nullable: true),
                    PapTypeID = table.Column<int>(type: "int", nullable: false),
                    SummaryReportId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prexc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prexc_PapType_PapTypeID",
                        column: x => x.PapTypeID,
                        principalTable: "PapType",
                        principalColumn: "PapTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prexc_SummaryReport_SummaryReportId",
                        column: x => x.SummaryReportId,
                        principalTable: "SummaryReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prexc_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundSourceAmount",
                columns: table => new
                {
                    FundSourceAmountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UacsId = table.Column<int>(type: "int", nullable: false),
                    beginning_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    remaining_balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fundsource_amount_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fundsource_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetAllotmentId = table.Column<int>(type: "int", nullable: false),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    PrexcId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundSourceAmount", x => x.FundSourceAmountId);
                    table.ForeignKey(
                        name: "FK_FundSourceAmount_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSourceAmount_Prexc_PrexcId",
                        column: x => x.PrexcId,
                        principalTable: "Prexc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundSourceAmount_Uacs_UacsId",
                        column: x => x.UacsId,
                        principalTable: "Uacs",
                        principalColumn: "UacsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundsRealignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundSourceAmountId = table.Column<int>(type: "int", nullable: true),
                    Realignment_to = table.Column<int>(type: "int", nullable: false),
                    Realignment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FundSourceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundsRealignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundsRealignment_FundSource_FundSourceId",
                        column: x => x.FundSourceId,
                        principalTable: "FundSource",
                        principalColumn: "FundSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundsRealignment_FundSourceAmount_FundSourceAmountId",
                        column: x => x.FundSourceAmountId,
                        principalTable: "FundSourceAmount",
                        principalColumn: "FundSourceAmountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AllotmentClass",
                columns: new[] { "Id", "Account_Code", "Allotment_Class", "CreatedAt", "Desc", "Fund_Code", "UpdatedAt" },
                values: new object[,]
                {
                    { 3, "300", "CO", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9996), "Capital Outlay", "06", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9997) },
                    { 2, "200", "MOOE", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9993), "Maintenance and Other Operating Expenses", "02", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9994) },
                    { 1, "100", "PS", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9985), "Personnel Services", "01", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(9990) }
                });

            migrationBuilder.InsertData(
                table: "Appropriation",
                columns: new[] { "AppropriationId", "AppropriationSource", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CURRENT", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(60), new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(8095) },
                    { 2, "CONAP", new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(8421), new DateTime(2022, 2, 18, 10, 16, 40, 93, DateTimeKind.Local).AddTicks(8425) }
                });

            migrationBuilder.InsertData(
                table: "Fund",
                columns: new[] { "FundId", "AppropriationID", "Fund_code_conap", "Fund_code_current", "Fund_description" },
                values: new object[,]
                {
                    { 6, null, "", "104102", "Retirement and Life Insurance Premium (RLIP)" },
                    { 5, null, "102407", "101407", "Pension and Gratuity Fund" },
                    { 3, null, "102402", "101402", "Contingent Fund" },
                    { 4, null, "101406", "101406", "Miscellaneous Personnel Benefits Fund (MPBF)" },
                    { 1, null, "102101", "101101", "Specific Budget of National Government Agencies" },
                    { 2, null, "102401", "101401", "National Disaster Risk Reduction and Management Fund (Calamity Fund)" }
                });

            migrationBuilder.InsertData(
                table: "PapType",
                columns: new[] { "PapTypeID", "PapTypeName" },
                values: new object[,]
                {
                    { 4, "" },
                    { 3, "OPERATION" },
                    { 2, "STO" },
                    { 1, "GAS" }
                });

            migrationBuilder.InsertData(
                table: "RespoCenter",
                columns: new[] { "RespoId", "Respo", "RespoCode", "RespoHead", "RespoHeadPosition" },
                values: new object[,]
                {
                    { 3, "RD/ARD", "13-001-03-0007-2022-003", "GUY R. PEREZ, MD, RPT, FPSMS, MBAHA, CESE", "DIRECTOR III" },
                    { 1, "MSD", "13-001-03-0007-2022-001", "ELIZABETH P. TABASA, CPA, MBA, CEO VI", "CHIEF - MANAGEMENT SUPPORT DIVISION" },
                    { 2, "LHSD", "13-001-03-0007-2022-002", "JONATHAN NEIL V. ERASMO, MD, MPH, FPSMS", "CHIEF - LOCAL HEALTH SUPPORT DIVISION" },
                    { 4, "RLED", "13-001-03-0007-2022-004", "SOPHIA M. MANCAO, MD, DPSP", "CHIEF - Regulation of Regional Health Facilities and Services" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotment_AllotmentClassId",
                table: "BudgetAllotment",
                column: "AllotmentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotment_AppropriationId",
                table: "BudgetAllotment",
                column: "AppropriationId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotment_FundId",
                table: "BudgetAllotment",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotment_YearlyReferenceId",
                table: "BudgetAllotment",
                column: "YearlyReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllotmentTrustFund_YearlyReferenceId",
                table: "BudgetAllotmentTrustFund",
                column: "YearlyReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Designation_Ors_headId",
                table: "Designation",
                column: "Ors_headId");

            migrationBuilder.CreateIndex(
                name: "IX_Designation_Requesting_officeId",
                table: "Designation",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fund_AppropriationID",
                table: "Fund",
                column: "AppropriationID");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_AllotmentClassId",
                table: "FundSource",
                column: "AllotmentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_AppropriationId",
                table: "FundSource",
                column: "AppropriationId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_BudgetAllotmentId",
                table: "FundSource",
                column: "BudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_BudgetAllotmentTrustFundId",
                table: "FundSource",
                column: "BudgetAllotmentTrustFundId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_FundId",
                table: "FundSource",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_ObligationId",
                table: "FundSource",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_PapTypeID",
                table: "FundSource",
                column: "PapTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_PrexcId",
                table: "FundSource",
                column: "PrexcId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_RespoId",
                table: "FundSource",
                column: "RespoId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_SummaryReportId",
                table: "FundSource",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSource_UtilizationId",
                table: "FundSource",
                column: "UtilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_FundSourceId",
                table: "FundSourceAmount",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_PrexcId",
                table: "FundSourceAmount",
                column: "PrexcId");

            migrationBuilder.CreateIndex(
                name: "IX_FundSourceAmount_UacsId",
                table: "FundSourceAmount",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_FundsRealignment_FundSourceAmountId",
                table: "FundsRealignment",
                column: "FundSourceAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_FundsRealignment_FundSourceId",
                table: "FundsRealignment",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationAmount_FundSourceId",
                table: "ObligationAmount",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationAmount_ObligationId",
                table: "ObligationAmount",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligationAmount_SubAllotmentId",
                table: "ObligationAmount",
                column: "SubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Budget_allotmentBudgetAllotmentId",
                table: "Personal_Information",
                column: "Budget_allotmentBudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Ors_headId",
                table: "Personal_Information",
                column: "Ors_headId");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Information_Requesting_officeId",
                table: "Personal_Information",
                column: "Requesting_officeId");

            migrationBuilder.CreateIndex(
                name: "IX_Prexc_PapTypeID",
                table: "Prexc",
                column: "PapTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Prexc_SummaryReportId",
                table: "Prexc",
                column: "SummaryReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Prexc_UacsId",
                table: "Prexc",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_AllotmentClassId",
                table: "Sub_allotment",
                column: "AllotmentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_AppropriationId",
                table: "Sub_allotment",
                column: "AppropriationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_BudgetAllotmentId",
                table: "Sub_allotment",
                column: "BudgetAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_FundId",
                table: "Sub_allotment",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_ObligationId",
                table: "Sub_allotment",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_prexcId",
                table: "Sub_allotment",
                column: "prexcId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_RespoId",
                table: "Sub_allotment",
                column: "RespoId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_allotment_UtilizationId",
                table: "Sub_allotment",
                column: "UtilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Suballotment_amount_SubAllotmentId",
                table: "Suballotment_amount",
                column: "SubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Suballotment_amount_UacsId",
                table: "Suballotment_amount",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAllotment_Realignment_SubAllotmentAmountId",
                table: "SubAllotment_Realignment",
                column: "SubAllotmentAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAllotment_Realignment_SubAllotmentId",
                table: "SubAllotment_Realignment",
                column: "SubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReport_UacsId",
                table: "SummaryReport",
                column: "UacsId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_FundSourceId",
                table: "Uacs",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_ObligationId",
                table: "Uacs",
                column: "ObligationId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_Sub_allotmentSubAllotmentId",
                table: "Uacs",
                column: "Sub_allotmentSubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Uacs_UtilizationId",
                table: "Uacs",
                column: "UtilizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationAmount_FundSourceId",
                table: "UtilizationAmount",
                column: "FundSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationAmount_SubAllotmentId",
                table: "UtilizationAmount",
                column: "SubAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationAmount_UtilizationId",
                table: "UtilizationAmount",
                column: "UtilizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource",
                column: "PrexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundSource_SummaryReport_SummaryReportId",
                table: "FundSource",
                column: "SummaryReportId",
                principalTable: "SummaryReport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_allotment_Prexc_prexcId",
                table: "Sub_allotment",
                column: "prexcId",
                principalTable: "Prexc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotment_AllotmentClass_AllotmentClassId",
                table: "BudgetAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_AllotmentClass_AllotmentClassId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_AllotmentClass_AllotmentClassId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotment_Appropriation_AppropriationId",
                table: "BudgetAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_Fund_Appropriation_AppropriationID",
                table: "Fund");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Appropriation_AppropriationId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Appropriation_AppropriationId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotment_Fund_FundId",
                table: "BudgetAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Fund_FundId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Fund_FundId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotment_Yearly_reference_YearlyReferenceId",
                table: "BudgetAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllotmentTrustFund_Yearly_reference_YearlyReferenceId",
                table: "BudgetAllotmentTrustFund");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_BudgetAllotment_BudgetAllotmentId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_BudgetAllotment_BudgetAllotmentId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_BudgetAllotmentTrustFund_BudgetAllotmentTrustFundId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Obligation_ObligationId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Obligation_ObligationId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_Uacs_Obligation_ObligationId",
                table: "Uacs");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_PapType_PapTypeID",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Prexc_PapType_PapTypeID",
                table: "Prexc");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_Prexc_PrexcId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_Prexc_prexcId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_RespoCenter_RespoId",
                table: "FundSource");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_allotment_RespoCenter_RespoId",
                table: "Sub_allotment");

            migrationBuilder.DropForeignKey(
                name: "FK_FundSource_SummaryReport_SummaryReportId",
                table: "FundSource");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Designation");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "Dts");

            migrationBuilder.DropTable(
                name: "FundsRealignment");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "ManageUsers");

            migrationBuilder.DropTable(
                name: "ObligationAmount");

            migrationBuilder.DropTable(
                name: "Personal_Information");

            migrationBuilder.DropTable(
                name: "PrexcTrustFund");

            migrationBuilder.DropTable(
                name: "RequestingOfficeTrustFund");

            migrationBuilder.DropTable(
                name: "RespoCenterTrustFund");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "SubAllotment_Realignment");

            migrationBuilder.DropTable(
                name: "UacsTrustFund");

            migrationBuilder.DropTable(
                name: "UtilizationAmount");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FundSourceAmount");

            migrationBuilder.DropTable(
                name: "Ors_head");

            migrationBuilder.DropTable(
                name: "Requesting_office");

            migrationBuilder.DropTable(
                name: "Suballotment_amount");

            migrationBuilder.DropTable(
                name: "AllotmentClass");

            migrationBuilder.DropTable(
                name: "Appropriation");

            migrationBuilder.DropTable(
                name: "Fund");

            migrationBuilder.DropTable(
                name: "Yearly_reference");

            migrationBuilder.DropTable(
                name: "BudgetAllotment");

            migrationBuilder.DropTable(
                name: "BudgetAllotmentTrustFund");

            migrationBuilder.DropTable(
                name: "Obligation");

            migrationBuilder.DropTable(
                name: "PapType");

            migrationBuilder.DropTable(
                name: "Prexc");

            migrationBuilder.DropTable(
                name: "RespoCenter");

            migrationBuilder.DropTable(
                name: "SummaryReport");

            migrationBuilder.DropTable(
                name: "Uacs");

            migrationBuilder.DropTable(
                name: "FundSource");

            migrationBuilder.DropTable(
                name: "Sub_allotment");

            migrationBuilder.DropTable(
                name: "Utilization");
        }
    }
}
