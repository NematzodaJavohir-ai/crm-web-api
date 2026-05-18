using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Note).HasMaxLength(500);
        builder.Property(p => p.ReceiptUrl).HasMaxLength(500);

        builder.HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Group)
            .WithMany()
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.StudentId);
        builder.HasIndex(p => p.GroupId);
        builder.HasIndex(p => p.Date);
        builder.HasIndex(p => p.IsConfirmed);
    }
}