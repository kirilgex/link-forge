using System.Security.Claims;

using LinkForge.Application.Links.Dto;
using LinkForge.Domain.Links.ValueObjects;
using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Services.Interfaces;

public interface ILinksProcessService
{
    Task<Result<LinkCode>> ProcessLinkAsync(
        CreateLinkRequest request,
        ClaimsPrincipal user,
        CancellationToken ct = default);
}
