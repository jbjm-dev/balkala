using BalkalaInfra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BalkalaInfra.Infrastructure.Data.Configurations;

public sealed class ClientInquiryConfiguration : IEntityTypeConfiguration<ClientInquiry>
{
    public void Configure(EntityTypeBuilder<ClientInquiry> builder)
    {
        builder.ToTable("ClientInquiry");
        builder.HasKey(inquiry => inquiry.InquiryId).HasName("PK_clientinquiry");

        builder.Property(inquiry => inquiry.InquiryId)
            .ValueGeneratedOnAdd();

        builder.Property(inquiry => inquiry.Name).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.Email).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.Phone).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.Description).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.CreatedDate).HasColumnType("datetime(6)").IsRequired();
        builder.Property(inquiry => inquiry.Status).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.Location).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.Source).HasColumnType("longtext").IsRequired();
        builder.Property(inquiry => inquiry.ProjectLocation).HasMaxLength(200).IsRequired();
    }
}
