using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Application.Configurations;

public class MentorConfiguration : IEntityTypeConfiguration<Mentor>
{
    public void Configure(EntityTypeBuilder<Mentor> builder)
    {
        
        
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Phone)
            .HasMaxLength(20);

        builder.Property(m => m.Specialization)
            .HasMaxLength(100);

        builder.Property(m => m.Bio)
            .HasMaxLength(1000);


        
        builder.HasMany(m => m.Groups)
            .WithOne(g => g.Mentor)
            .HasForeignKey(g => g.MentorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
}


