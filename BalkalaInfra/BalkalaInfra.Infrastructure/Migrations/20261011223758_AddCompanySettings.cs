using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkalaInfra.Infrastructure.Migrations;

public partial class AddCompanySettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CompanySettings",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                CompanyName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                LegalName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                LogoUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: true),
                PhoneNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                AlternatePhoneNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                Website = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                AddressLine1 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                AddressLine2 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                City = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                State = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                Country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                GSTNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                PANNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                CurrencyCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                FinancialYearStartMonth = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_CompanySettings", x => x.Id))
            .Annotation("MySQL:Charset", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "CompanySettings");
}
