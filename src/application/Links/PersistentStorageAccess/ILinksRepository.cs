using LinkForge.Domain.Links;
using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.PersistentStorageAccess;

public interface ILinksRepository
{
    Task<Result<Link>> FindAsync(
        string code,
        CancellationToken ct = default);

    Task InsertAsync(
        Link link,
        CancellationToken ct = default);
}
