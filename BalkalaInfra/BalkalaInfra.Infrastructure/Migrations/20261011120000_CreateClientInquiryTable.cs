using BalkalaInfra.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BalkalaInfra.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261011120000_CreateClientInquiryTable")]
public sealed class CreateClientInquiryTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS `clientinquiry`
            (
                `InquiryId` INT NOT NULL AUTO_INCREMENT,
                `Name` LONGTEXT NOT NULL,
                `Email` LONGTEXT NOT NULL,
                `Phone` LONGTEXT NOT NULL,
                `Description` LONGTEXT NOT NULL,
                `CreatedDate` DATETIME(6) NOT NULL,
                `Status` LONGTEXT NOT NULL,
                `Location` LONGTEXT NOT NULL,
                `Source` LONGTEXT NOT NULL,
                CONSTRAINT `PK_clientinquiry` PRIMARY KEY (`InquiryId`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "clientinquiry");
}
