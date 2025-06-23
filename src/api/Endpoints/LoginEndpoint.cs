using Asp.Versioning;

using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users.ValueTypes;

namespace LinkForge.API.Endpoints;

public static class LoginEndpoint
{
    public static string GetNameWithVersion(ApiVersion? version)
        => $"login-v-{version?.MajorVersion ?? 0}";

    public static RouteGroupBuilder MapLoginEndpoint(
        this RouteGroupBuilder group,
        ApiVersion? apiVersion = null)
    {
        group
            .MapPost("login", HandleAsync)
            .WithName(GetNameWithVersion(apiVersion));
        return group;
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.Problem(
                title: "Invalid Request",
                detail: "Email and password required.",
                statusCode: StatusCodes.Status400BadRequest);

        var user = await authService.AuthenticateUserAsync(
            UserEmail.ParseFromUserInput(request.Email),
            UserPassword.ParseFromUserInput(request.Password),
            ct);

        if (user is null)
            return Results.Unauthorized();

        context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        var tokenPair = await authService.CreateAuthTokensAsync(
            user, new Application.Entities.UserAgent(userAgent), ct);

        return Results.Ok(
            new
            {
                accessToken = tokenPair.AccessToken,
                refreshToken = tokenPair.RefreshToken,
            });
    }

    private record LoginRequest(string Email, string Password);
}
