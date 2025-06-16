using System.Security.Claims;

using Asp.Versioning;

using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.API.Endpoints;

public static class PostLinkEndpoint
{
    public static string GetNameWithVersion(ApiVersion? version)
        => $"post-link-v-{version?.MajorVersion ?? 0}";

    public static RouteGroupBuilder MapPostLinkEndpoint(
        this RouteGroupBuilder group,
        ApiVersion? apiVersion = null)
    {
        group
            .MapPost("links", HandleAsync)
            .WithName(GetNameWithVersion(apiVersion))
            .RequireAuthorization();
        return group;
    }

    private static async Task<IResult> HandleAsync(
        PostLinkRequest request,
        HttpContext context,
        ILinksProcessService linksProcessService,
        LinkGenerator linkGenerator,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return Results.Problem(
                title: "Invalid Request",
                detail: "The 'url' field is required and cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);

        if (!LinkOriginalUrl.TryParseFromUserInput(request.Url, out var url))
            return Results.Problem(
                title: "Invalid Request",
                detail: "The 'url' field must be a valid url.",
                statusCode: StatusCodes.Status400BadRequest);

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId?.Value))
            return Results.Problem(
                title: "Invalid Request",
                detail: "Invalid auth token.",
                statusCode: StatusCodes.Status400BadRequest);

        var code = await linksProcessService.ProcessLinkAsync(url, (EntityId)userId.Value, ct);

        var version = context.GetRequestedApiVersion() ?? new ApiVersion(majorVersion: 0);
        var endpointName = GetLinkEndpoint.GetNameWithVersion(version);
        var location = linkGenerator.GetUriByName(context, endpointName, new { code, });

        context.Response.StatusCode = StatusCodes.Status201Created;
        context.Response.Headers.Location = location;

        return Results.Json(new { code, url = location, });
    }

    private record PostLinkRequest(string Url);
}
