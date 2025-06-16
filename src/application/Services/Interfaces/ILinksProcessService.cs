using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Application.Services.Interfaces;

public interface ILinksProcessService
{
    Task<string> ProcessLinkAsync(
        LinkUrl url,
        EntityId ownerId,
        CancellationToken ct = default);
}
