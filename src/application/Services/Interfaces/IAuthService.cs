using LinkForge.Application.DTO;
using LinkForge.Application.Entities;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;

namespace LinkForge.Application.Services.Interfaces;

public interface IAuthService
{
    Task<bool> UserExistsAsync(
        UserEmail email,
        CancellationToken ct = default);

    Task CreateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default);

    Task<User?> AuthenticateUserAsync(
        UserEmail email,
        UserPassword password,
        CancellationToken ct = default);

    public Task<AuthTokenPair> CreateAuthTokensAsync(
        User user,
        UserAgent userAgent,
        CancellationToken ct = default);

    Task<AuthTokenPair?> RefreshTokensAsync(
        string refreshToken,
        UserAgent userAgent,
        CancellationToken ct = default);
}
