using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BalkalaInfra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    EmployeeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Dob = table.Column<DateOnly>(type: "date", nullable: false),
                    Mobile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    AlternateMobile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Department = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Designation = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    EmployeeType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    JoiningDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Shift = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ReportingManager = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    Branch = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    WorkLocation = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    EmploymentStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PanNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    AadhaarNumber = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    PresentAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    PermanentAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    City = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    State = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Pincode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    AccountHolder = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    BankName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Ifsc = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    BankBranch = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    UpiId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ProfilePhoto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    AadharCard = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    PanCard = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    PassportPhoto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Resume = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    OfferLetter = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    JoiningLetter = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmploymentStatus",
                table: "Employees",
                column: "EmploymentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
