using LinkForge.API.Extensions;
using LinkForge.Application.Auth.Dto;
using LinkForge.Application.Auth.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints.Auth;

public static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.RefreshTokenEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.RefreshTokenEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .WithSummary("Refresh access token")
            .WithDescription("Generates a new access token for valid refresh token.")
            .Accepts<RefreshTokenRequest>("application/json")
            .Produces<AuthTokenPairResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        RefreshTokenRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        var result = await authService.RefreshTokensAsync(request, context.Request.GetUserAgent(), ct);
        return result.ToHttpResponse();
    }
}