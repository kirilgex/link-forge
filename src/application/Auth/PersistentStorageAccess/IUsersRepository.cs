using LinkForge.Domain.Shared;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.Application.Auth.PersistentStorageAccess;

public interface IUsersRepository
{
    Task<Result<User>> FindAsync(
        UserEmail email,
        CancellationToken ct = default);

    Task<Result> InsertAsync(
        User user,
        CancellationToken ct = default);
}
