using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BalkalaInfra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertPrimaryKeysToUuidAndAddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Preserve existing rows and old identifiers for reference while assigning new UUID primary keys.
            migrationBuilder.Sql("ALTER TABLE `Employees` ADD COLUMN `Id_New` varchar(36) NULL;");
            migrationBuilder.Sql("UPDATE `Employees` SET `Id_New` = UUID() WHERE `Id_New` IS NULL;");
            migrationBuilder.Sql("ALTER TABLE `Employees` MODIFY COLUMN `Id` int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Employees` DROP PRIMARY KEY, CHANGE COLUMN `Id` `LegacyId` int NOT NULL, CHANGE COLUMN `Id_New` `Id` varchar(36) NOT NULL, ADD PRIMARY KEY (`Id`);");

            migrationBuilder.Sql("ALTER TABLE `Dashboards` ADD COLUMN `Id_New` varchar(36) NULL;");
            migrationBuilder.Sql("UPDATE `Dashboards` SET `Id_New` = UUID() WHERE `Id_New` IS NULL;");
            migrationBuilder.Sql("ALTER TABLE `Dashboards` MODIFY COLUMN `Id` int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Dashboards` DROP PRIMARY KEY, CHANGE COLUMN `Id` `LegacyId` int NOT NULL, CHANGE COLUMN `Id_New` `Id` varchar(36) NOT NULL, ADD PRIMARY KEY (`Id`);");

            migrationBuilder.Sql("ALTER TABLE `clientinquiry` ADD COLUMN `InquiryId_New` varchar(36) NULL;");
            migrationBuilder.Sql("UPDATE `clientinquiry` SET `InquiryId_New` = UUID() WHERE `InquiryId_New` IS NULL;");
            migrationBuilder.Sql("ALTER TABLE `clientinquiry` MODIFY COLUMN `InquiryId` int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `clientinquiry` DROP PRIMARY KEY, CHANGE COLUMN `InquiryId` `LegacyId` int NOT NULL, CHANGE COLUMN `InquiryId_New` `InquiryId` varchar(36) NOT NULL, ADD PRIMARY KEY (`InquiryId`);");

            migrationBuilder.Sql("ALTER TABLE `Labours` ADD COLUMN `LabourId_New` varchar(36) NULL;");
            migrationBuilder.Sql("UPDATE `Labours` SET `LabourId_New` = UUID() WHERE `LabourId_New` IS NULL;");
            migrationBuilder.Sql("ALTER TABLE `Labours` DROP PRIMARY KEY, CHANGE COLUMN `LabourId` `LabourCode` varchar(32) NOT NULL, CHANGE COLUMN `LabourId_New` `LabourId` varchar(36) NOT NULL, ADD PRIMARY KEY (`LabourId`);");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    PermissionCode = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    ModuleCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ActionCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: false),
                    Mobile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    GlobalStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    RoleCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    RoleType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Custom"),
                    IsSystemRole = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.UniqueConstraint("AK_Roles_Tenant_Role", x => new { x.TenantId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Roles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    TenantUserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.TenantUserId);
                    table.UniqueConstraint("AK_TenantUsers_Tenant_TenantUser", x => new { x.TenantId, x.TenantUserId });
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    UserSessionId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    SessionTokenHash = table.Column<string>(type: "char(128)", maxLength: 128, nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "char(128)", maxLength: 128, nullable: false),
                    DeviceId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RefreshExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.UserSessionId);
                    table.ForeignKey(
                        name: "FK_UserSessions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RolePermissionId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    PermissionId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    AccessScope = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "Tenant")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_TenantId_RoleId",
                        columns: x => new { x.TenantId, x.RoleId },
                        principalTable: "Roles",
                        principalColumns: new[] { "TenantId", "RoleId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantUserRoles",
                columns: table => new
                {
                    TenantUserRoleId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    TenantUserId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    OrganizationId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    ProjectId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRoles", x => x.TenantUserRoleId);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_Roles_TenantId_RoleId",
                        columns: x => new { x.TenantId, x.RoleId },
                        principalTable: "Roles",
                        principalColumns: new[] { "TenantId", "RoleId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantUsers_TenantId_TenantUserId",
                        columns: x => new { x.TenantId, x.TenantUserId },
                        principalTable: "TenantUsers",
                        principalColumns: new[] { "TenantId", "TenantUserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "ActionCode", "Description", "IsActive", "ModuleCode", "PermissionCode" },
                values: new object[,]
                {
                    { "00b8be78-3ad3-4b59-988a-29492c99a85c", "read", "Read projects.", true, "project", "project.read" },
                    { "0fe67627-69e8-2d94-128d-39ef65d9a4fe", "create", "Create projects.", true, "project", "project.create" },
                    { "17db6b43-df7d-2eab-97b6-3d62f206ec0f", "read", "Read roles and assignments.", true, "role", "role.read" },
                    { "1b65ee39-7593-1dc3-9fca-002484577a46", "create", "Create user records.", true, "user", "user.create" },
                    { "2320d9f2-8ce3-d30b-5501-78263ff1a112", "update", "Update user records.", true, "user", "user.update" },
                    { "2e95e599-7107-2071-174c-644e9a2e4eb7", "read", "Read attendance records.", true, "attendance", "attendance.read" },
                    { "6afd069f-9ab4-17bc-15e5-59541e469c96", "read", "Read user records.", true, "user", "user.read" },
                    { "6ee5f755-0a7c-ca17-1448-123790194cc0", "assign", "Assign or revoke roles.", true, "role", "role.assign" },
                    { "b8e5c9f9-de9b-34f9-7952-e115c4056a24", "read", "Read payroll records.", true, "payroll", "payroll.read" },
                    { "cfeb608d-b1b1-6fed-0791-f95643e5f717", "read", "Read employee records.", true, "employee", "employee.read" },
                    { "d927f628-a242-ae19-6e91-fed7f53fa88f", "deactivate", "Deactivate user records.", true, "user", "user.deactivate" },
                    { "da349b92-f185-34cd-0a06-40646a70d5e5", "update", "Update projects.", true, "project", "project.update" },
                    { "e1a0b98a-510c-131d-be72-b70e686b3ffc", "create", "Create employee records.", true, "employee", "employee.create" }
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "CreatedAt", "Name", "Status", "TenantCode" },
                values: new object[] { "cc38040b-d727-cca6-d02c-43bb2301a87f", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Balkala Infra LLP", "Active", "BALKALA-INFRA-LLP" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "IsSystemRole", "RoleCode", "RoleName", "RoleType", "TenantId" },
                values: new object[,]
                {
                    { "0942e2e8-d2d4-07c7-4004-c4b9c4a12609", "Project management role.", true, "ProjectManager", "Project Manager", "System", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "a6b76b94-5811-3095-dd61-72c2c57f6222", "Employee self-service role.", true, "Employee", "Employee", "System", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "Human resources management role.", true, "HRManager", "HR Manager", "System", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "c845f311-0c60-16a7-3cc9-bb9256373bef", "Company-level system administration.", true, "Admin", "Admin", "System", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "fe9e595b-0e95-169c-e59e-a450b0df0cf8", "Labour self-service role.", true, "Labour", "Labour", "System", "cc38040b-d727-cca6-d02c-43bb2301a87f" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "RolePermissionId", "AccessScope", "PermissionId", "RoleId", "TenantId" },
                values: new object[,]
                {
                    { "0708cdb6-80a5-5af7-2e5b-82d218a7990d", "Own", "2e95e599-7107-2071-174c-644e9a2e4eb7", "a6b76b94-5811-3095-dd61-72c2c57f6222", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "08b7ba51-c172-5e94-25a5-ec338b1cac3e", "Tenant", "b8e5c9f9-de9b-34f9-7952-e115c4056a24", "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "116ac67d-9ec2-22dc-0630-013051e26828", "Tenant", "da349b92-f185-34cd-0a06-40646a70d5e5", "0942e2e8-d2d4-07c7-4004-c4b9c4a12609", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "26cd066a-58a0-9a6c-cf2c-c5e427a7fb21", "Tenant", "e1a0b98a-510c-131d-be72-b70e686b3ffc", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "324d7b7c-7344-92dd-4fdb-2357ae87666c", "Own", "2e95e599-7107-2071-174c-644e9a2e4eb7", "fe9e595b-0e95-169c-e59e-a450b0df0cf8", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "364d9a54-0f79-3bba-b75a-1a5236b539da", "Tenant", "0fe67627-69e8-2d94-128d-39ef65d9a4fe", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "3770568f-0cb2-8f0a-3894-b722be55920d", "Tenant", "2e95e599-7107-2071-174c-644e9a2e4eb7", "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "4c0cd01c-2821-eb70-cdc0-0b2a6d6acca6", "Tenant", "1b65ee39-7593-1dc3-9fca-002484577a46", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "524badd9-1af9-1a4c-f1e0-05b46c76f4d6", "Tenant", "6afd069f-9ab4-17bc-15e5-59541e469c96", "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "5e0ad5ce-30c6-2837-a08b-71c26a7b85ee", "Tenant", "2320d9f2-8ce3-d30b-5501-78263ff1a112", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "6298ff6b-fde5-348c-f100-130bd8db9127", "Tenant", "b8e5c9f9-de9b-34f9-7952-e115c4056a24", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "6917d119-0c6f-7012-a679-c86f94b802d1", "Tenant", "6ee5f755-0a7c-ca17-1448-123790194cc0", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "69baaa7b-ebc8-0459-840a-f897cc071eb6", "Tenant", "17db6b43-df7d-2eab-97b6-3d62f206ec0f", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "9a12c395-decf-8e64-d8b3-099700f9ac56", "Tenant", "6afd069f-9ab4-17bc-15e5-59541e469c96", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "9ab0e730-2e66-a65d-dfda-b1a8b567a749", "Tenant", "da349b92-f185-34cd-0a06-40646a70d5e5", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "a8b60763-c950-ac8d-b162-e5828a679b2c", "Tenant", "2e95e599-7107-2071-174c-644e9a2e4eb7", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "b8c00460-dfac-f10a-be2a-c52de7b6991f", "Tenant", "cfeb608d-b1b1-6fed-0791-f95643e5f717", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "b9ebe353-76f3-9287-6402-2aa4335e5070", "Tenant", "cfeb608d-b1b1-6fed-0791-f95643e5f717", "0942e2e8-d2d4-07c7-4004-c4b9c4a12609", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "ba213f61-3423-6863-ca9f-f8f5fd005b13", "Own", "cfeb608d-b1b1-6fed-0791-f95643e5f717", "a6b76b94-5811-3095-dd61-72c2c57f6222", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "cd830a22-df62-3c37-69c9-9d461a50b07b", "Tenant", "d927f628-a242-ae19-6e91-fed7f53fa88f", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "d3b77e80-0345-2124-531b-e3faa1b6e24c", "Tenant", "e1a0b98a-510c-131d-be72-b70e686b3ffc", "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "e13e8fe0-472b-e5a7-27f4-ba63df3c99e1", "Tenant", "00b8be78-3ad3-4b59-988a-29492c99a85c", "0942e2e8-d2d4-07c7-4004-c4b9c4a12609", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "e90f889c-7800-e204-7031-51027fddac0f", "Tenant", "cfeb608d-b1b1-6fed-0791-f95643e5f717", "a78fcf7e-1bbd-960e-c889-c5d3e6c585cd", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "eb1eeb10-325d-9a6b-6262-2fc00a5ac370", "Tenant", "00b8be78-3ad3-4b59-988a-29492c99a85c", "c845f311-0c60-16a7-3cc9-bb9256373bef", "cc38040b-d727-cca6-d02c-43bb2301a87f" },
                    { "f0a4b875-d434-9192-82ff-edfc86be09e0", "Tenant", "0fe67627-69e8-2d94-128d-39ef65d9a4fe", "0942e2e8-d2d4-07c7-4004-c4b9c4a12609", "cc38040b-d727-cca6-d02c-43bb2301a87f" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Labours_LabourCode",
                table: "Labours",
                column: "LabourCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UQ_RolePermissions_Tenant_Role_Permission",
                table: "RolePermissions",
                columns: new[] { "TenantId", "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Roles_Tenant_RoleCode",
                table: "Roles",
                columns: new[] { "TenantId", "RoleCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Tenants_TenantCode",
                table: "Tenants",
                column: "TenantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantId_RoleId",
                table: "TenantUserRoles",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantId_TenantUserId",
                table: "TenantUserRoles",
                columns: new[] { "TenantId", "TenantUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_UserId",
                table: "TenantUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_TenantUsers_Tenant_User",
                table: "TenantUsers",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_TenantId",
                table: "UserSessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_RevokedAt",
                table: "UserSessions",
                columns: new[] { "UserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "UQ_UserSessions_RefreshTokenHash",
                table: "UserSessions",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_UserSessions_SessionTokenHash",
                table: "UserSessions",
                column: "SessionTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TenantUserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.Sql("ALTER TABLE `Employees` DROP PRIMARY KEY, CHANGE COLUMN `Id` `Id_Uuid` varchar(36) NOT NULL, CHANGE COLUMN `LegacyId` `Id` int NOT NULL, ADD PRIMARY KEY (`Id`);");
            migrationBuilder.Sql("ALTER TABLE `Employees` MODIFY COLUMN `Id` int NOT NULL AUTO_INCREMENT;");
            migrationBuilder.Sql("ALTER TABLE `Employees` DROP COLUMN `Id_Uuid`;");

            migrationBuilder.Sql("ALTER TABLE `Dashboards` DROP PRIMARY KEY, CHANGE COLUMN `Id` `Id_Uuid` varchar(36) NOT NULL, CHANGE COLUMN `LegacyId` `Id` int NOT NULL, ADD PRIMARY KEY (`Id`);");
            migrationBuilder.Sql("ALTER TABLE `Dashboards` MODIFY COLUMN `Id` int NOT NULL AUTO_INCREMENT;");
            migrationBuilder.Sql("ALTER TABLE `Dashboards` DROP COLUMN `Id_Uuid`;");

            migrationBuilder.Sql("ALTER TABLE `clientinquiry` DROP PRIMARY KEY, CHANGE COLUMN `InquiryId` `InquiryId_Uuid` varchar(36) NOT NULL, CHANGE COLUMN `LegacyId` `InquiryId` int NOT NULL, ADD PRIMARY KEY (`InquiryId`);");
            migrationBuilder.Sql("ALTER TABLE `clientinquiry` MODIFY COLUMN `InquiryId` int NOT NULL AUTO_INCREMENT;");
            migrationBuilder.Sql("ALTER TABLE `clientinquiry` DROP COLUMN `InquiryId_Uuid`;");

            migrationBuilder.Sql("ALTER TABLE `Labours` DROP INDEX `UQ_Labours_LabourCode`;");
            migrationBuilder.Sql("ALTER TABLE `Labours` DROP PRIMARY KEY, CHANGE COLUMN `LabourId` `LabourId_Uuid` varchar(36) NOT NULL, CHANGE COLUMN `LabourCode` `LabourId` varchar(32) NOT NULL, ADD PRIMARY KEY (`LabourId`);");
            migrationBuilder.Sql("ALTER TABLE `Labours` DROP COLUMN `LabourId_Uuid`;");
        }
    }
}
