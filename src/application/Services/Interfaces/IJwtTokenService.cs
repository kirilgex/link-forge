using LinkForge.Domain.Users;

namespace LinkForge.Application.Services.Interfaces;

public interface IJwtTokenService
{
    public string GenerateToken(User user);
}
