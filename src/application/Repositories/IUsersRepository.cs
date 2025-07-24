using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.Application.Repositories;

public interface IUsersRepository
{
    Task<bool> ExistsAsync(
        UserEmail email,
        CancellationToken ct = default);

    Task<User?> FindAsync(
        UserEmail email,
        CancellationToken ct = default);

    Task InsertAsync(
        User user,
        CancellationToken ct = default);
}
