using LinkForge.API.Extensions;
using LinkForge.Application.Auth.Dto;
using LinkForge.Application.Auth.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.RegisterEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.RegisterEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account if provided email and password are valid, and email is not already taken.")
            .Accepts<RegisterUserRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        RegisterUserRequest request,
        IAuthService authService,
        CancellationToken ct = default)
    {
        var result = await authService.RegisterUserAsync(request, ct);
        return result.ToHttpResponse();
    }
}
