using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(x => x.EmployeeId).HasName("PK_Employees");
        builder.Property(x => x.EmployeeId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.EmployeeCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.TenantUserId);
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.DesignationId).IsRequired();
        builder.Property(x => x.JoinDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.Status).HasColumnType("enum('Active','Inactive','OnLeave','Resigned','Terminated')").HasDefaultValue("Active");
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.TenantUserId);
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.DesignationId);
    }
}
