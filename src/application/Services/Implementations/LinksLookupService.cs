using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Links;

namespace LinkForge.Application.Services.Implementations;

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
