using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeds;

public static class DefaultUser
{
    public static async Task SeedAsync(DataContext context)
    {
        const string adminEmail = "javohir@gmail.com";
        const string adminPhone = "941580303";

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        
        if (adminRole == null) return;

        bool anyAdminExists = await context.Users.AnyAsync(u => u.Role.Name == "Admin");
        bool emailTaken = await context.Users.AnyAsync(u => u.Email == adminEmail);
        bool phoneTaken = await context.Users.AnyAsync(u => u.PhoneNumber == adminPhone);

        if (!anyAdminExists && !emailTaken && !phoneTaken)
        {
            var admin = new User
            {
                FirstName = "Javohir",
                LastName = "Nematzoda",
                PhoneNumber = adminPhone,
                Email = adminEmail,
                RoleId = adminRole.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("javohir2006")
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
            Console.WriteLine("--> [SEED] Admin created.");
        }
        else
        {
            if (anyAdminExists) Console.WriteLine("--> [SEED] Admin already exists.");
            else if (emailTaken || phoneTaken) Console.WriteLine("--> [SEED] Cannot seed admin: Email or Phone is already taken!");
        }
    }
}