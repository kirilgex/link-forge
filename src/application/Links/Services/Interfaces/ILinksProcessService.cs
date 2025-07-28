using LinkForge.Domain.Links.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.Application.Links.Services.Interfaces;

public interface ILinksProcessService
{
    Task<string> ProcessLinkAsync(
        LinkUrl url,
        ObjectId ownerId,
        CancellationToken ct = default);
}
