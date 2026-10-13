using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class DashboardDefinitionConfiguration : IEntityTypeConfiguration<DashboardDefinition>
{
    public void Configure(EntityTypeBuilder<DashboardDefinition> builder)
    {
        builder.ToTable("dashboarddefinitions");
        builder.HasKey(x => x.DashboardDefinitionId).HasName("PRIMARY");
        builder.Property(x => x.DashboardDefinitionId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.DashboardCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DashboardName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.AudienceType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LayoutJson).HasColumnType("json").IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime").IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.DashboardCode })
            .IsUnique().HasDatabaseName("UQ_DashboardDefinitions");
        builder.HasIndex(x => x.TenantId).HasDatabaseName("IX_DashboardDefinitions_TenantId");
        builder.HasIndex(x => x.DashboardCode).HasDatabaseName("IX_DashboardDefinitions_DashboardCode");
        builder.HasIndex(x => x.AudienceType).HasDatabaseName("IX_DashboardDefinitions_AudienceType");
        builder.HasIndex(x => x.IsDefault).HasDatabaseName("IX_DashboardDefinitions_IsDefault");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("IX_DashboardDefinitions_IsActive");

        builder.HasOne(x => x.Tenant).WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_DashboardDefinitions_Tenant");
    }
}
