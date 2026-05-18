using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).HasMaxLength(50);
        builder.Property(p => p.LastName).HasMaxLength(50);
        builder.Property(p => p.AvatarUrl).HasMaxLength(500);
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.Address).HasMaxLength(300);
        builder.Property(p => p.TelegramUsername).HasMaxLength(50);
        builder.Property(p => p.LinkedInUrl).HasMaxLength(300);
        builder.Property(p => p.GithubUrl).HasMaxLength(300);
        builder.Property(p => p.AboutMe).HasMaxLength(1000);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Phone);
        builder.HasIndex(p => p.TelegramUsername);
    }
}