using LinkForge.Domain.Users;

namespace LinkForge.Application.Repositories;

public interface IUsersRepository
{
    Task<User?> FindAsync(
        string email,
        CancellationToken ct = default);

    Task InsertAsync(
        User user,
        CancellationToken ct = default);
}
