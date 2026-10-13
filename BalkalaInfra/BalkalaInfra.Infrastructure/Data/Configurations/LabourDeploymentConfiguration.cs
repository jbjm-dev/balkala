using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class LabourDeploymentConfiguration : IEntityTypeConfiguration<LabourDeployment>
{
    public void Configure(EntityTypeBuilder<LabourDeployment> builder)
    {
        builder.ToTable("labourdeployments");
        builder.HasKey(x => x.LabourDeploymentId).HasName("PRIMARY");
        builder.Property(x => x.LabourDeploymentId).ValueGeneratedOnAdd();
        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.ProjectId).IsRequired();
        builder.Property(x => x.TaskId);
        builder.Property(x => x.WorkerId).IsRequired();
        builder.Property(x => x.WorkDate).HasColumnType("date").IsRequired();
        builder.Property(x => x.ShiftId).IsRequired();
        builder.Property(x => x.HoursWorked).HasPrecision(5, 2);
        builder.Property(x => x.QuantityCompleted).HasPrecision(12, 2);
        builder.Property(x => x.Rate).HasPrecision(12, 2);

        builder.HasIndex(x => new { x.ProjectId, x.WorkerId, x.WorkDate, x.ShiftId })
            .IsUnique().HasDatabaseName("UQ_LabourDeployment");
        builder.HasIndex(x => x.TenantId).HasDatabaseName("FK_LabourDeployments_Tenants");
        builder.HasIndex(x => x.WorkerId).HasDatabaseName("FK_LabourDeployments_Workers");
        builder.HasIndex(x => x.ShiftId).HasDatabaseName("FK_LabourDeployments_Shifts");
    }
}
