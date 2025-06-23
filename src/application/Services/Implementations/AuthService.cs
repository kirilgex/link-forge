using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using LinkForge.Application.DTO;
using LinkForge.Application.Entities;
using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Application.Settings;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LinkForge.Application.Services.Implementations;

public class AuthService(
    IOptions<AuthSettings> authSettings,
    IUsersRepository usersRepository,
    IRefreshTokensRepository refreshTokensRepository)
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

        user.PasswordHash = PasswordHasher.HashPassword(user, password.ToString());

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

        var verificationResult = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password.ToString());

        if (verificationResult is PasswordVerificationResult.Failed)
            return null;

        return user;
    }

    public async Task<AuthTokenPair> CreateAuthTokensAsync(
        User user,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var accessToken = CreateAccessToken(claims, tokenHandler);
        var refreshToken = CreateRefreshToken(claims, tokenHandler);

        var oldRefreshTokenData = await refreshTokensRepository.FindAsync(user.Id, userAgent, ct);

        await refreshTokensRepository.ReplaceOneAsync(
            new RefreshToken(oldRefreshTokenData?.Id, user, userAgent, ComputeRefreshTokenHash(refreshToken)),
            ct);

        return new AuthTokenPair(accessToken, refreshToken);
    }

    public async Task<AuthTokenPair?> RefreshTokensAsync(
        string refreshToken,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(authSettings.Value.RefreshToken.SecretKey);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = authSettings.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
        };

        ClaimsPrincipal? principal = null;

        try
        {
            principal = tokenHandler.ValidateToken(refreshToken, parameters, out var _);
        }
        catch
        {
            // Consider token invalid.
        }

        if (principal is null)
            return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
            return null;

        var refreshTokenData = await refreshTokensRepository.FindAsync(
            new EntityId(userIdClaim.Value),
            userAgent,
            ct);

        if (refreshTokenData is null)
            return null;

        if (ComputeRefreshTokenHash(refreshToken) != refreshTokenData.TokenHash)
            return null;

        var accessToken = CreateAccessToken(principal.Claims, tokenHandler);
        var newRefreshToken = CreateRefreshToken(principal.Claims, tokenHandler);

        await refreshTokensRepository.ReplaceOneAsync(
            new RefreshToken(
                refreshTokenData.Id,
                refreshTokenData.User,
                userAgent,
                ComputeRefreshTokenHash(newRefreshToken)),
            ct);

        return new AuthTokenPair(accessToken, newRefreshToken);
    }

    private string CreateAccessToken(
        IEnumerable<Claim> claims,
        JwtSecurityTokenHandler tokenHandler)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Value.AccessToken.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: authSettings.Value.Issuer,
            audience: authSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(authSettings.Value.AccessToken.ExpirationTimeInMinutes),
            signingCredentials: credentials);

        return tokenHandler.WriteToken(token);
    }

    private string CreateRefreshToken(
        IEnumerable<Claim> claims,
        JwtSecurityTokenHandler tokenHandler)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Value.RefreshToken.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: authSettings.Value.Issuer,
            audience: authSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(authSettings.Value.RefreshToken.ExpirationTimeInMinutes),
            signingCredentials: credentials);

        return tokenHandler.WriteToken(token);
    }

    private string ComputeRefreshTokenHash(string token)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(authSettings.Value.RefreshToken.SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
