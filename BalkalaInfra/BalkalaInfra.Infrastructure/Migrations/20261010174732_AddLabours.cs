using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BalkalaInfra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLabours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labours",
                columns: table => new
                {
                    LabourId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    FatherName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Gender = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Dob = table.Column<DateTime>(type: "date", nullable: true),
                    Mobile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    AlternateMobile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: true),
                    Address = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    City = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    State = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    Pincode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Aadhaar = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    Pan = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    SkillTrade = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    ProjectId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ProjectName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Task = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    DailyWage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AdvanceSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    BankName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    AccountNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Ifsc = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    ProfilePhoto = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labours", x => x.LabourId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Labours_Status",
                table: "Labours",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Labours");
        }
    }
}
