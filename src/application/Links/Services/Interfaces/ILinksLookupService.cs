using LinkForge.Domain.Links;

namespace LinkForge.Application.Links.Services.Interfaces;

public interface ILinksLookupService
{
    Task<Link?> FindLinkAsync(
        string code,
        CancellationToken ct = default);
}
