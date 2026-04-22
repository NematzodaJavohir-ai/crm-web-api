using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;



public class GroupStudentConfiguration : IEntityTypeConfiguration<GroupStudent>
{
    public void Configure(EntityTypeBuilder<GroupStudent> builder)
    {
        builder.HasKey(gs => gs.Id);

    
        builder.HasIndex(gs => new { gs.GroupId, gs.StudentId })
            .IsUnique();

        builder.Property(gs => gs.RemoveReason)
            .HasMaxLength(500);
    }
}