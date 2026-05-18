using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GroupStudentConfiguration : IEntityTypeConfiguration<GroupStudent>
{
    public void Configure(EntityTypeBuilder<GroupStudent> builder)
    {
        builder.HasKey(gs => gs.Id);

        builder.Property(gs => gs.RemoveReason).HasMaxLength(500);

        builder.HasOne(gs => gs.Group)
            .WithMany(g => g.GroupStudents)
            .HasForeignKey(gs => gs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gs => gs.Student)
            .WithMany(s => s.GroupStudents)
            .HasForeignKey(gs => gs.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gs => gs.TransferredFrom)
            .WithMany()
            .HasForeignKey(gs => gs.TransferredFromGroupStudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(gs => gs.TransferredTo)
            .WithMany()
            .HasForeignKey(gs => gs.TransferredToGroupStudentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(gs => new { gs.GroupId, gs.StudentId }).IsUnique();
        builder.HasIndex(gs => gs.IsActive);
    }
}