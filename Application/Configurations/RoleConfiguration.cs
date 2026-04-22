using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;



public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(200);

        builder.HasIndex(r => r.Name)
            .IsUnique();

            builder.HasData(
            new Role 
            { 
                Id = 1, 
                Name = "Admin", 
                Description = "System administrator with full access" 
            },
            new Role 
            { 
                Id = 2, 
                Name = "Mentor", 
                Description = "Teacher/Mentor with course management access" 
            },
            new Role 
            { 
                Id = 3, 
                Name = "Student", 
                Description = "Regular user/Student" 
            }
        );
    }
}