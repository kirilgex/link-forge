using System.Security.Claims;

using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Errors;
using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueObjects;
using LinkForge.Domain.Shared;

using MongoDB.Bson;

namespace LinkForge.Application.Links.Services.Implementations;

public class LinksProcessService(
    IHashingService hashingService,
    ILinksRepository linksRepository)
    : ILinksProcessService
{
    public async Task<Result<LinkCode>> ProcessLinkAsync(
        CreateLinkRequest request,
        ClaimsPrincipal user,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Url)
            || !LinkUrl.TryParseFromUserInput(request.Url, out var url))
        {
            return Result<LinkCode>.Failure(new InvalidUrlError());
        }
        
        var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(nameIdentifier?.Value)
            || !ObjectId.TryParse(nameIdentifier.Value, out var userId))
        {
            return Result<LinkCode>.Failure(new UnauthorizedError());
        }
        
        var guid = Guid.CreateVersion7().ToString();
        var code = hashingService.ComputeHashAsHexString(guid);

        var link = new Link { OwnerId = userId, Code = LinkCode.FromTrusted(code), Url = url, };
    
        await linksRepository.InsertAsync(link, ct);

        return Result<LinkCode>.Success(link.Code);
    }
}
