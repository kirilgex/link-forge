using LinkForge.API.Extensions;
using LinkForge.Application.Auth.Dto;
using LinkForge.Application.Auth.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.LoginEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.LoginEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .WithSummary("Authenticate user")
            .WithDescription("Verifies provided credentials and returns access and refresh tokens if authentication succeeds.")
            .Accepts<LoginRequest>("application/json")
            .Produces<AuthTokenPairResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        HttpContext context,
        IAuthService authService,
        CancellationToken ct = default)
    {
        var result = await authService.AuthenticateUserAsync(request, context.Request.GetUserAgent(), ct);
        return result.ToHttpResponse();
    }
}
