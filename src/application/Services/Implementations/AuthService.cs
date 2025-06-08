using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LinkForge.Application.Services.Implementations;

public class AuthService(
    IConfiguration configuration,
    IUsersRepository usersRepository)
    : IAuthService
{
    public async Task<bool> UserExistsAsync(
        UserEmail email,
        CancellationToken ct = default)
        => await usersRepository.ExistsAsync(email, ct);

    public async Task CreateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };

        await usersRepository.InsertAsync(user, ct);
    }

    public async Task<User?> AuthenticateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default)
    {
        var user = await usersRepository.FindAsync(email, ct);

        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public string CreateAuthToken(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Auth:Issuer"],
            audience: configuration["Auth:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Auth:TTLMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
