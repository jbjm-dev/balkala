using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.UserId).HasName("PK_Users");
        builder.Property(x => x.UserId).ValueGeneratedOnAdd();
        builder.Property(x => x.Email).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Mobile).HasMaxLength(20);
        builder.Property(x => x.IdentityProvider).HasColumnType("enum('Local','Google','Microsoft','AzureAD','Apple','Other')").HasDefaultValue("Local").IsRequired(false);
        builder.Property(x => x.IdentityProviderSubject).HasMaxLength(255);
        builder.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(x => x.GlobalStatus).HasColumnType("enum('Active','Inactive','Locked','Suspended')").HasDefaultValue("Active").IsRequired(false);
        builder.Property(x => x.LastLoginAt).HasColumnType("datetime");
        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQ_Users_Email");
    }
}
