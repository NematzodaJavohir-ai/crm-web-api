using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;


public class MentorConfiguration : IEntityTypeConfiguration<Mentor>
{
    public void Configure(EntityTypeBuilder<Mentor> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Phone).HasMaxLength(20);
        builder.Property(m => m.Specialization).HasMaxLength(100);
        
        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Groups)
            .WithOne(g => g.Mentor)
            .HasForeignKey(g => g.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.Specialization);
        builder.HasIndex(m => m.IsActive);
    }
}