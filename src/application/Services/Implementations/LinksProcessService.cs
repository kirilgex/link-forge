using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Application.Services.Implementations;

public class LinksProcessService(
    IHashingService hashingService,
    ILinksRepository linksRepository)
    : ILinksProcessService
{
    public async Task<string> ProcessLinkAsync(
        LinkUrl url,
        EntityId ownerId,
        CancellationToken ct = default)
    {
        var guid = Guid.CreateVersion7().ToString();
        var code = hashingService.ComputeHashAsHexString(guid);

        var link = new Link(ownerId, new LinkCode(code), url);
    
        await linksRepository.InsertAsync(link, ct);

        return code;
    }
}
