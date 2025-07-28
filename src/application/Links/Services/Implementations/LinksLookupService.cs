using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links;

namespace LinkForge.Application.Links.Services.Implementations;

public class LinksLookupService(
    ILinksRepository linksRepository)
    : ILinksLookupService
{
    public async Task<Link?> FindLinkAsync(
        string code,
        CancellationToken ct = default)
    {
        return await linksRepository.FindAsync(code, ct);
    }
}
