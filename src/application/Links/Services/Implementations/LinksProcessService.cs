using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.Application.Links.Services.Implementations;

public class LinksProcessService(
    IHashingService hashingService,
    ILinksRepository linksRepository)
    : ILinksProcessService
{
    public async Task<string> ProcessLinkAsync(
        LinkUrl url,
        ObjectId ownerId,
        CancellationToken ct = default)
    {
        var guid = Guid.CreateVersion7().ToString();
        var code = hashingService.ComputeHashAsHexString(guid);

        var link = new Link { OwnerId = ownerId, Code = LinkCode.FromTrusted(code), Url = url, };
    
        await linksRepository.InsertAsync(link, ct);

        return code;
    }
}
