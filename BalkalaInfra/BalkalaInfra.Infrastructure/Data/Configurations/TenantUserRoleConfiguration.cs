using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class TenantUserRoleConfiguration : IEntityTypeConfiguration<TenantUserRole>
{
    public void Configure(EntityTypeBuilder<TenantUserRole> builder)
    {
        builder.ToTable("tenantuserroles");
        builder.HasKey(x => x.TenantUserRoleId).HasName("PK_TenantUserRoles");
        builder.Property(x => x.TenantUserRoleId).ValueGeneratedOnAdd();
        builder.Property(x => x.OrganizationId);
        builder.Property(x => x.ProjectId);
        builder.Property(x => x.EffectiveFrom).HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnType("date");
        builder.HasIndex(x => new { x.TenantId, x.TenantUserId });
        builder.HasIndex(x => new { x.TenantId, x.RoleId });
        builder.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantUser).WithMany(x => x.TenantUserRoles).HasForeignKey(x => x.TenantUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Role).WithMany(x => x.TenantUserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}
