using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.LoginEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.LoginEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "Email and password required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var user = await authService.AuthenticateUserAsync(
            UserEmail.ParseFromUserInput(request.Email),
            UserPassword.ParseFromUserInput(request.Password),
            ct);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        var tokenPair = await authService.CreateAuthTokensAsync(
            user, new UserAgent(userAgent), ct);

        return TypedResults.Ok(new LoginResponse(tokenPair.AccessToken, tokenPair.RefreshToken));
    }

    private record LoginRequest(string Email, string Password);

    private record LoginResponse(string AccessToken, string RefreshToken);
}
