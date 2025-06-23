using Asp.Versioning;

using LinkForge.Application.Entities;
using LinkForge.Application.Services.Interfaces;

namespace LinkForge.API.Endpoints;

public static class RefreshTokenEndpoint
{
    public static string GetNameWithVersion(ApiVersion? version)
        => $"refresh-token-v-{version?.MajorVersion ?? 0}";

    public static RouteGroupBuilder MapRefreshTokenEndpoint(
        this RouteGroupBuilder group,
        ApiVersion? apiVersion = null)
    {
        group
            .MapPost("refresh-token", HandleAsync)
            .WithName(GetNameWithVersion(apiVersion));
        return group;
    }

    private static async Task<IResult> HandleAsync(
        RefreshTokenRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Results.Problem(
                title: "Invalid Request",
                detail: "Refresh token is required.",
                statusCode: StatusCodes.Status400BadRequest);

        context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        var tokenPair = await authService.RefreshTokensAsync(
            request.RefreshToken, new UserAgent(userAgent), ct);

        if (tokenPair is null)
            return Results.Unauthorized();

        return Results.Ok(
            new
            {
                accessToken = tokenPair.AccessToken,
                refreshToken = tokenPair.RefreshToken,
            });
    }

    private record RefreshTokenRequest(string RefreshToken);
}
