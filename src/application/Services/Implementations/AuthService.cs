using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LinkForge.Application.Services.Implementations;

public class AuthService(
    IConfiguration configuration,
    IUsersRepository usersRepository)
    : IAuthService
{
    private static readonly IPasswordHasher<User> PasswordHasher =
        new PasswordHasher<User>(
            new OptionsWrapper<PasswordHasherOptions>(
                new PasswordHasherOptions
                {
                    CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                }));

    public async Task<bool> UserExistsAsync(
        UserEmail email,
        CancellationToken ct = default)
        => await usersRepository.ExistsAsync(email, ct);

    public async Task CreateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default)
    {
        var user = new User(email);

        user.PasswordHash = PasswordHasher.HashPassword(user, password);

        await usersRepository.InsertAsync(user, ct);
    }

    public async Task<User?> AuthenticateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default)
    {
        var user = await usersRepository.FindAsync(email, ct);

        if (user is null || user.PasswordHash is null)
            return null;

        var verificationResult = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verificationResult is PasswordVerificationResult.Failed)
            return null;

        return user;
    }

    public string CreateAuthToken(User user)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
