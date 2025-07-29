using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Errors;
using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Services.Implementations;

public class LinksLookupService(
    ILinksRepository linksRepository)
    : ILinksLookupService
{
    public async Task<Result<FindLinkResponse>> FindLinkAsync(
        string code,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<FindLinkResponse>.Failure(new LinkNotFoundError());
        }
        
        var result = await linksRepository.FindAsync(code, ct);
        return result.Map(link => new FindLinkResponse(link.Url));
    }
}
