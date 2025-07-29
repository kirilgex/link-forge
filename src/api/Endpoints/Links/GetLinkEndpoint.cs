using LinkForge.API.Extensions;
using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links.ValueObjects;

namespace LinkForge.API.Endpoints.Links;

public static class GetLinkEndpoint
{
    public static IEndpointRouteBuilder MapGetLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapGet(LinksEndpointsSettings.GetLinkEndpointPattern, HandleAsync)
            .WithName(LinksEndpointsSettings.GetLinkEndpointName)
            .WithTags(LinksEndpointsSettings.Tags)
            .WithSummary("Retrieve link")
            .WithDescription("Retrieves link by provided code.")
            .Produces<FindLinkResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        return app;
    }

    private static async Task<IResult> HandleAsync(
        string code,
        ILinksLookupService linksLookupService,
        CancellationToken ct = default)
    {
        var result = await linksLookupService.FindLinkAsync(LinkCode.FromUserInput(code), ct);
        return result.ToHttpResponse();
    }
}