using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("tenantusers");
        builder.HasKey(x => x.TenantUserId).HasName("PK_TenantUsers");
        builder.Property(x => x.TenantUserId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Status).HasColumnType("enum('Active','Inactive','Invited','Suspended')").HasDefaultValue("Active").IsRequired(false);
        builder.Property(x => x.EmployeeId);
        builder.Property(x => x.DefaultOrganizationId);
        builder.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique().HasDatabaseName("UQ_TenantUsers_Tenant_User");
        builder.HasOne(x => x.Tenant).WithMany(x => x.TenantUsers).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.User).WithMany(x => x.TenantUsers).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
