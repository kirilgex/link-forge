using System.Security.Claims;

using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.API.Endpoints.Links;

public static class PostLinkEndpoint
{
    public static IEndpointRouteBuilder MapPostLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(LinksEndpointsSettings.PostLinkEndpointPattern, HandleAsync)
            .WithName(LinksEndpointsSettings.PostLinkEndpointName)
            .WithTags(LinksEndpointsSettings.Tags)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
        return app;
    }

    private static async Task<IResult> HandleAsync(
        PostLinkRequest request,
        HttpContext context,
        ILinksProcessService linksProcessService,
        LinkGenerator linkGenerator,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "The 'url' field is required and cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }


        if (!LinkUrl.TryParseFromUserInput(request.Url, out var url))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "The 'url' field must be a valid url.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(nameIdentifier?.Value) || !ObjectId.TryParse(nameIdentifier.Value, out var userId))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "Invalid auth token.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var code = await linksProcessService.ProcessLinkAsync(url, userId, ct);

        var location = linkGenerator.GetUriByName(context, LinksEndpointsSettings.GetLinkEndpointName, new { code, });

        context.Response.Headers.Location = location;

        return TypedResults.CreatedAtRoute(LinksEndpointsSettings.GetLinkEndpointName, new { code, });
    }

    private record PostLinkRequest(string Url);
}
