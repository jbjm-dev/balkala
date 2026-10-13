using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.RoleId).HasName("PK_Roles");
        builder.Property(x => x.RoleId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.RoleCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RoleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RoleType).HasColumnType("enum('System','Administrator','Manager','Employee','Custom')").HasDefaultValue("Custom").IsRequired(false);
        // IsSystemRole is a non-nullable bool in the entity, so EF must map it as required.
        builder.Property(x => x.IsSystemRole).HasDefaultValue(false).IsRequired();
        builder.HasOne(x => x.Tenant).WithMany(x => x.Roles).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}
