using System.Text.Json.Serialization;

using Asp.Versioning;

using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Links.ValueTypes;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints;

public static class GetLinkEndpoint
{
    public static string GetNameWithVersion(ApiVersion? version)
        => $"get-link-v-{version?.MajorVersion ?? 0}";

    public static RouteGroupBuilder MapGetLinkEndpoint(
        this RouteGroupBuilder group,
        ApiVersion? apiVersion = null)
    {
        group
            .MapGet("links/{code?}", HandleAsync)
            .WithName(GetNameWithVersion(apiVersion))
            .RequireAuthorization();
        return group;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string? code,
        ILinksLookupService linksLookupService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Results.Problem(
                title: "Invalid Request",
                detail: "The 'code' parameter is required and cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);

        var result = await linksLookupService.FindLinkAsync(
            LinkCode.FromUserInput(code), ct);

        if (result is null) return Results.NotFound();

        return Results.Ok(new Response(result.OriginalUrl));
    }

    private record Response(string Link)
    {
        [JsonPropertyName("link")]
        public string Link { get; init; } = Link;
    };
}
