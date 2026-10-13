using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("rolepermissions");
        builder.HasKey(x => x.RolePermissionId).HasName("PK_RolePermissions");
        builder.Property(x => x.RolePermissionId).ValueGeneratedOnAdd();
        builder.Property(x => x.AccessScope).HasColumnType("enum('Global','Organization','Branch','Department','Project','Own')").HasDefaultValue("Own").IsRequired(false);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.RoleId);
        builder.HasIndex(x => x.PermissionId);
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);
    }
}
