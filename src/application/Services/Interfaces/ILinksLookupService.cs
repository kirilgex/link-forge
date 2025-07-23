using LinkForge.Domain.Links;

namespace LinkForge.Application.Services.Interfaces;

public interface ILinksLookupService
{
    Task<Link?> FindLinkAsync(
        string code,
        CancellationToken ct = default);
}
