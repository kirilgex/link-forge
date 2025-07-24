using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Links.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints.Links;

public static class GetLinkEndpoint
{
    public static IEndpointRouteBuilder MapGetLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapGet(LinksEndpointsSettings.GetLinkEndpointPattern, HandleAsync)
            .WithName(LinksEndpointsSettings.GetLinkEndpointName)
            .WithTags(LinksEndpointsSettings.Tags)
            .Produces<GetLinkResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string? code,
        ILinksLookupService linksLookupService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "The 'code' parameter is required and cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await linksLookupService.FindLinkAsync(
            LinkCode.FromUserInput(code), ct);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetLinkResponse(result.Url));
    }

    private record GetLinkResponse(string Link);
}
