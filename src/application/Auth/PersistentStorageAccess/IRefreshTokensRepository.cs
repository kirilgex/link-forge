using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.Application.Auth.PersistentStorageAccess;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> FindAsync(
        ObjectId userId,
        UserAgent userAgent,
        CancellationToken ct = default);

    Task InsertAsync(
        RefreshToken token,
        CancellationToken ct = default);

    Task ReplaceOneAsync(
        RefreshToken token,
        CancellationToken ct = default);
}
