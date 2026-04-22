using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateToken(User user)
    {
        // 1. Получаем секретный ключ из конфигурации
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 2. Формируем список Claims (данные внутри токена)
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            // ЗАЩИТА: Если user.Role == null, берем "User", чтобы не было ошибки NullReferenceException
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
        };

        // 3. Настраиваем время жизни токена
        if (!int.TryParse(configuration["Jwt:ExpireMinutes"], out var minutes))
        {
            minutes = 60;
        }
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}