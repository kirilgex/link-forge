using LinkForge.Application.Links.Dto;
using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Services.Interfaces;

public interface ILinksLookupService
{
    Task<Result<FindLinkResponse>> FindLinkAsync(
        string code,
        CancellationToken ct = default);
}
