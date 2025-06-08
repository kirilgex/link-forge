using LinkForge.Domain.Links;

namespace LinkForge.Application.Repositories;

public interface ILinksRepository
{
    Task<Link?> FindAsync(
        string code,
        CancellationToken ct = default);

    Task InsertAsync(
        Link link,
        CancellationToken ct = default);
}
