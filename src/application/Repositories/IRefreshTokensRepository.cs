using LinkForge.Application.Entities;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Application.Repositories;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> FindAsync(
        EntityId userId,
        UserAgent userAgent,
        CancellationToken ct = default);

    Task InsertAsync(
        RefreshToken token,
        CancellationToken ct = default);

    Task ReplaceOneAsync(
        RefreshToken token,
        CancellationToken ct = default);
}
