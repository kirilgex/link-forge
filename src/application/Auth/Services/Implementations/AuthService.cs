using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using LinkForge.Application.Auth.Dto;
using LinkForge.Application.Auth.Errors;
using LinkForge.Application.Auth.PersistentStorageAccess;
using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Application.Auth.Settings;
using LinkForge.Domain.Shared;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;

namespace LinkForge.Application.Auth.Services.Implementations;

public class AuthService(
    IOptions<AuthSettings> authSettings,
    IPasswordValidationService passwordValidationService,
    IUsersRepository usersRepository,
    IRefreshTokensRepository refreshTokensRepository)
    : IAuthService
{
    private static readonly PasswordHasher<User> PasswordHasher =
        new(
            new OptionsWrapper<PasswordHasherOptions>(
                new PasswordHasherOptions
                {
                    CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                }));

    public async Task<Result> RegisterUserAsync(
        RegisterUserRequest request,
        CancellationToken ct = default)
    {
        if (!UserEmail.TryParseFromUserInput(request.Email, out var email))
        {
            return Result.Failure(new InvalidEmailError());
        }
        
        if (!passwordValidationService.ValidatePassword(request.Password, out var password))
        {
            return Result.Failure(new InvalidPasswordError(passwordValidationService.GetReadablePasswordRestrictions()));
        }
        
        var user = new User { Email = email, };
        user.PasswordHash = PasswordHasher.HashPassword(user, password);

        return await usersRepository.InsertAsync(user, ct);
    }

    public async Task<Result<AuthTokenPairResponse>> AuthenticateUserAsync(
        LoginRequest request,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        if (!UserEmail.TryParseFromUserInput(request.Email, out var email)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<AuthTokenPairResponse>.Failure(new InvalidCredentialsError());
        }
        
        var userLookupResult = await usersRepository.FindAsync(email, ct);
        if (!userLookupResult.IsSuccess)
        {
            return Result<AuthTokenPairResponse>.Failure(userLookupResult.Error!);
        }

        var user = userLookupResult.Value!;
        
        var verificationResult = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult is PasswordVerificationResult.Failed)
        {
            return Result<AuthTokenPairResponse>.Failure(new NotAuthenticatedError());
        }

        var authTokenPair = await CreateAuthTokensAsync(user, userAgent, ct);
        return Result<AuthTokenPairResponse>.Success(authTokenPair);
    }

    public async Task<Result<AuthTokenPairResponse>> RefreshTokensAsync(
        RefreshTokenRequest request,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<AuthTokenPairResponse>.Failure(new InvalidRefreshTokenError());
        }
        
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
        
        ClaimsPrincipal principal;

        try
        {
            principal = tokenHandler.ValidateToken(request.RefreshToken, parameters, out _);
        }
        catch
        {
            return Result<AuthTokenPairResponse>.Failure(new InvalidRefreshTokenError());
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
        {
            return Result<AuthTokenPairResponse>.Failure(new InvalidRefreshTokenError());
        }

        var refreshTokenData = await refreshTokensRepository.FindAsync(
            ObjectId.Parse(userIdClaim.Value), userAgent, ct);

        if (refreshTokenData is null
            || ComputeRefreshTokenHash(request.RefreshToken) != refreshTokenData.TokenHash)
        {
            return Result<AuthTokenPairResponse>.Failure(new InvalidRefreshTokenError());
        }

        var accessToken = CreateAccessToken(principal.Claims, tokenHandler);
        var newRefreshToken = CreateRefreshToken(principal.Claims, tokenHandler);

        await refreshTokensRepository.ReplaceOneAsync(
            new RefreshToken
            {
                Id = refreshTokenData.Id,
                User = refreshTokenData.User,
                UserAgent = userAgent,
                TokenHash = ComputeRefreshTokenHash(newRefreshToken),
            },
            ct);

        return Result<AuthTokenPairResponse>.Success(new AuthTokenPairResponse(accessToken, newRefreshToken));
    }
    
    private async Task<AuthTokenPairResponse> CreateAuthTokensAsync(
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
        var newRefreshTokenData = new RefreshToken
        {
            Id = oldRefreshTokenData?.Id ?? default,
            User = user,
            UserAgent = userAgent,
            TokenHash = ComputeRefreshTokenHash(refreshToken)
        };

        if (oldRefreshTokenData is null)
        {
            await refreshTokensRepository.InsertAsync(newRefreshTokenData, ct);
        }
        else
        {
            await refreshTokensRepository.ReplaceOneAsync(newRefreshTokenData, ct);
        }

        return new AuthTokenPairResponse(accessToken, refreshToken);
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