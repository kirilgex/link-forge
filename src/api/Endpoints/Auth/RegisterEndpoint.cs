using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users.ValueTypes;

namespace LinkForge.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app
            .MapPost(AuthEndpointsSettings.RegisterEndpointPattern, HandleAsync)
            .WithName(AuthEndpointsSettings.RegisterEndpointName)
            .WithTags(AuthEndpointsSettings.Tags)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        HttpContext context,
        IAuthService authService,
        LinkGenerator linkGenerator,
        CancellationToken ct = default)
    {
        if (!UserEmail.TryParseFromUserInput(request.Email, out var email))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "Valid email is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (!UserPassword.TryParseFromUserInput(request.Password, out var password))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: UserPassword.GetPasswordRestrictions(),
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (await authService.UserExistsAsync(email, ct))
        {
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "User already exists.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await authService.CreateUserAsync(email, password, ct);

        return TypedResults.CreatedAtRoute(AuthEndpointsSettings.LoginEndpointName, null);
    }

    private record RegisterRequest(string Email, string Password);
}
