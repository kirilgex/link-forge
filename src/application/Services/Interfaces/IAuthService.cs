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

    public string CreateAuthToken(User user);
}
