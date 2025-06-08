using LinkForge.Domain.Links.ValueTypes;

namespace LinkForge.Application.Services.Interfaces;

public interface ILinksProcessService
{
    Task<string> ProcessLinkAsync(
        LinkOriginalUrl url,
        CancellationToken ct = default);
}
