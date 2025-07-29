using LinkForge.Application.Auth.Dto;
using LinkForge.Domain.Shared;
using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.Application.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterUserAsync(
        RegisterUserRequest request,
        CancellationToken ct = default);

    Task<Result<AuthTokenPairResponse>> AuthenticateUserAsync(
        LoginRequest request,
        UserAgent userAgent,
        CancellationToken ct = default);

    Task<Result<AuthTokenPairResponse>> RefreshTokensAsync(
        RefreshTokenRequest request,
        UserAgent userAgent,
        CancellationToken ct = default);
}
