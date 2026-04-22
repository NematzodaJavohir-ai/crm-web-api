using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateToken(User user);
    
}
