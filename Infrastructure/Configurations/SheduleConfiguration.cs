using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SheduleConfiguration : IEntityTypeConfiguration<Shedule>
{
    public void Configure(EntityTypeBuilder<Shedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Day).HasConversion<string>().HasMaxLength(15);
     

        builder.HasOne(s => s.Group)
            .WithMany()
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.GroupId, s.Day, s.StartTime }).IsUnique();
    }
}