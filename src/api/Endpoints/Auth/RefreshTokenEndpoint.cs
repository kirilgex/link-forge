using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.API.Endpoints.Auth;

public static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.RefreshTokenEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.RefreshTokenEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        RefreshTokenRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "Refresh token is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        var tokenPair = await authService.RefreshTokensAsync(
            request.RefreshToken, new UserAgent(userAgent), ct);

        if (tokenPair is null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(new RefreshTokenResponse(tokenPair.AccessToken, tokenPair.RefreshToken));
    }

    private record RefreshTokenRequest(string RefreshToken);
    
    private record RefreshTokenResponse(string AccessToken, string RefreshToken);
}
