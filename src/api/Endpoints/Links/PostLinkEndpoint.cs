using LinkForge.API.Extensions;
using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints.Links;

public static class PostLinkEndpoint
{
    public static IEndpointRouteBuilder MapPostLinkEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(LinksEndpointsSettings.PostLinkEndpointPattern, HandleAsync)
            .WithName(LinksEndpointsSettings.PostLinkEndpointName)
            .WithTags(LinksEndpointsSettings.Tags)
            .WithSummary("Create link")
            .WithDescription("Generates a new short link for specified url.")
            .Accepts<CreateLinkRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        return app;
    }

    private static async Task<IResult> HandleAsync(
        CreateLinkRequest request,
        HttpContext context,
        ILinksProcessService linksProcessService,
        LinkGenerator linkGenerator,
        CancellationToken ct = default)
    {
        var result = await linksProcessService.ProcessLinkAsync(request, context.User, ct);
        return result.Match(
            onSuccess: code =>
            {
                context.Response.Headers.Location = linkGenerator.GetUriByName(
                    context, LinksEndpointsSettings.GetLinkEndpointName, new { Code = code.ToString(), });
                return TypedResults.CreatedAtRoute(
                    LinksEndpointsSettings.GetLinkEndpointName, new { Code = code.ToString(), });
            },
            onFailure: error => error.ToHttpProblemResponse());
    }
}