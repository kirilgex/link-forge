using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LinkForge.Application.Services.Implementations;

public class JwtTokenService(
    IConfiguration configuration)
    : IJwtTokenService
{
    public string GenerateToken(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
