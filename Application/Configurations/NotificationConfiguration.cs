using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(nt => nt.Id);

        builder.Property(nt => nt.Title)
               .IsRequired()
               .HasMaxLength(100);


            builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .HasConversion<string>();

        builder.HasIndex(n => new { n.UserId, n.IsRead }); 
    }

}
