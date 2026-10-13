using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(x => x.PermissionId).HasName("PK_Permissions");
        builder.Property(x => x.PermissionId).ValueGeneratedOnAdd();
        builder.Property(x => x.PermissionCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ModuleCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ActionCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.HasIndex(x => x.PermissionCode).IsUnique().HasDatabaseName("UQ_Permissions_PermissionCode");
    }
}
