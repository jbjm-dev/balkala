using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(x => x.TenantId).HasName("PK_Tenants");
        builder.Property(x => x.TenantId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Status).HasColumnType("enum('Active','Inactive','Suspended')").HasDefaultValue("Active").IsRequired(false);
        builder.Property(x => x.DeploymentModel).HasColumnType("enum('Shared','Dedicated')").HasDefaultValue("Shared").IsRequired(false);
        builder.Property(x => x.DefaultTimeZone).HasMaxLength(100).HasDefaultValue("Asia/Kolkata");
        builder.Property(x => x.DefaultCurrencyId);
        builder.Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(x => x.TenantCode).IsUnique().HasDatabaseName("UQ_Tenants_TenantCode");
    }
}
