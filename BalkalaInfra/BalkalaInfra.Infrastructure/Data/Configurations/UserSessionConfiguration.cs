using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("usersessions");
        builder.HasKey(x => x.UserSessionId).HasName("PK_UserSessions");
        builder.Property(x => x.UserSessionId).ValueGeneratedOnAdd();
        builder.Property(x => x.SessionTokenHash).HasColumnType("char(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.RefreshTokenHash).HasColumnType("char(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.DeviceId).HasMaxLength(255);
        builder.Property(x => x.ExpiresAt).HasColumnType("datetime").IsRequired();
        builder.Property(x => x.RefreshExpiresAt).HasColumnType("datetime").IsRequired();
        builder.Property(x => x.RevokedAt).HasColumnType("datetime");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TenantId);
        builder.HasOne(x => x.User).WithMany(x => x.UserSessions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Tenant).WithMany(x => x.UserSessions).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}
