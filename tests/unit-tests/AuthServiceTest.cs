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
using LinkForge.UnitTests.Builders;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;

using Moq;

namespace LinkForge.UnitTests;

public class AuthServiceTest
{
    private readonly AuthSettings _authSettings;
    private readonly IPasswordValidationService _passwordValidationService;

    public AuthServiceTest()
    {
        _authSettings = new AuthSettingsBuilder()
            .WithIssuer("test-issuer")
            .WithAudience("test-audience")
            .WithAccessToken(builder => builder
                .WithSecretKey("@&c8sZ%XO4UW2LZ#cIbUC3FYtGvHk%vf")
                .WithExpirationTimeInMinutes(5))
            .WithRefreshToken(builder => builder
                .WithSecretKey("@&c8sZ%XO4UW2LZ#cIbUC3FYtGvHk%vf")
                .WithExpirationTimeInMinutes(1440))
            .WithPasswordRestrictions(builder => builder
                .WithMinimalLength(8).WithUppercaseLetters().WithLowercaseLetters().WithDigits())
            .Build();
        
        _passwordValidationService = new PasswordValidationServiceBuilder()
            .WithAuthSettings(_authSettings)
            .Build();
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidInput_ShouldSucceed()
    {
        var request = new RegisterUserRequest(Email: "test@example.com", Password: "ValidPassword123!");
        
        var usersRepositoryMock = new Mock<IUsersRepository>();
        usersRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var sut = new AuthServiceBuilder()
            .WithAuthSettings(_authSettings)
            .WithPasswordValidationService(_passwordValidationService)
            .WithUsersRepository(usersRepositoryMock.Object)
            .Build();

        var result = await sut.RegisterUserAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RegisterUserAsync_WithInvalidEmail_ShouldReturnError()
    {
        var request = new RegisterUserRequest(Email: "invalid-email", Password: "ValidPassword123!");
        
        var usersRepositoryMock = new Mock<IUsersRepository>();
        usersRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        var sut = new AuthServiceBuilder()
            .WithAuthSettings(_authSettings)
            .WithPasswordValidationService(_passwordValidationService)
            .WithUsersRepository(usersRepositoryMock.Object)
            .Build();

        var result = await sut.RegisterUserAsync(request);

        Assert.False(result.IsSuccess);
        Assert.IsType<InvalidEmailError>(result.Error);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithValidCredentials_ShouldSucceed()
    {
        var request = new LoginRequest(Email: "test@example.com", Password: "ValidPassword123!");
        
        var userAgent = new UserAgent("Test");
        
        var user = new User { Email = UserEmail.FromTrusted("test@example.com"), };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

        var usersRepositoryMock = new Mock<IUsersRepository>();
        usersRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<UserEmail>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<User>.Success(user));
        
        var sut = new AuthServiceBuilder()
            .WithAuthSettings(_authSettings)
            .WithPasswordValidationService(_passwordValidationService)
            .WithUsersRepository(usersRepositoryMock.Object)
            .Build();

        var result = await sut.AuthenticateUserAsync(request, userAgent);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.AccessToken);
        Assert.NotEmpty(result.Value.RefreshToken);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithInvalidCredentials_ShouldReturnError()
    {
        var request = new LoginRequest(Email: "test@example.com", Password: "WrongPassword123!");
        
        var userAgent = new UserAgent("Test");
        
        var user = new User { Email = UserEmail.FromTrusted("test@example.com"), };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "ValidPassword123!");

        var usersRepositoryMock = new Mock<IUsersRepository>();
        usersRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<UserEmail>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<User>.Success(user));
        
        var sut = new AuthServiceBuilder()
            .WithAuthSettings(_authSettings)
            .WithPasswordValidationService(_passwordValidationService)
            .WithUsersRepository(usersRepositoryMock.Object)
            .Build();

        var result = await sut.AuthenticateUserAsync(request, userAgent);

        Assert.False(result.IsSuccess);
        Assert.IsType<NotAuthenticatedError>(result.Error);
    }

    [Fact]
    public async Task RefreshTokensAsync_WithValidToken_ShouldSucceed()
    {
        var userId = ObjectId.GenerateNewId();
        
        var userAgent = new UserAgent("Test");
        
        var user = new User { Id = userId, Email = UserEmail.FromTrusted("test@example.com"), };
        
        var refreshToken = CreateValidRefreshToken(userId.ToString());
        var request = new RefreshTokenRequest(refreshToken);
        
        var refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();
        refreshTokensRepositoryMock
            .Setup(x => x.FindAsync(userId, userAgent, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshToken
            {
                User = user,
                UserAgent = userAgent,
                TokenHash = ComputeRefreshTokenHash(refreshToken, _authSettings.RefreshToken.SecretKey)
            });
        
        var sut = new AuthServiceBuilder()
            .WithAuthSettings(_authSettings)
            .WithPasswordValidationService(_passwordValidationService)
            .WithRefreshTokensRepository(refreshTokensRepositoryMock.Object)
            .Build();

        var result = await sut.RefreshTokensAsync(request, userAgent);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.AccessToken);
        Assert.NotEmpty(result.Value.RefreshToken);
    }

    private string CreateValidRefreshToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.RefreshToken.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            claims: [ new Claim(ClaimTypes.NameIdentifier, userId), ],
            expires: DateTime.UtcNow.AddMinutes(_authSettings.RefreshToken.ExpirationTimeInMinutes),
            signingCredentials: credentials);

        return tokenHandler.WriteToken(token);
    }

    private static string ComputeRefreshTokenHash(string token, string secretKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}