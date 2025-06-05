using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Application.Services.Implementations;

public class LinksProcessService(
    IHashingService hashingService,
    ILinksRepository linksRepository)
    : ILinksProcessService
{
    public async Task<string> ProcessLinkAsync(
        LinkOriginalUrl url,
        CancellationToken ct = default)
    {
        var guid = Guid.CreateVersion7().ToString();
        var code = hashingService.ComputeHashAsHexString(guid);

        var link = new Link
        {
            Code = (LinkCode)code,
            OriginalUrl = url,
        };
    
        await linksRepository.InsertAsync(link, ct);

        return code;
    }
}
