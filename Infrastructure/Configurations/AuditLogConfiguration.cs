using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Action).HasMaxLength(50).IsRequired();
        builder.Property(a => a.OldValues).HasColumnType("nvarchar(max)");
        builder.Property(a => a.NewValues).HasColumnType("nvarchar(max)");
        builder.Property(a => a.IpAddress).HasMaxLength(45);

        builder.HasIndex(a => a.EntityName);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.Action);
    }
}