using Asp.Versioning;

using LinkForge.Application.Services.Interfaces;
using LinkForge.Domain.Users.ValueTypes;

namespace LinkForge.API.Endpoints;

public static class RegisterEndpoint
{
    public static string GetNameWithVersion(ApiVersion? version)
        => $"register-v-{version?.MajorVersion ?? 0}";

    public static RouteGroupBuilder MapRegisterEndpoint(
        this RouteGroupBuilder group,
        ApiVersion? apiVersion = null)
    {
        group
            .MapPost("register", HandleAsync)
            .WithName(GetNameWithVersion(apiVersion));
        return group;
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        HttpContext context,
        IAuthService authService,
        LinkGenerator linkGenerator,
        CancellationToken ct = default)
    {
        if (!UserEmail.TryParseFromUserInput(request.Email, out var email))
            return Results.Problem(
                title: "Invalid Request",
                detail: "Valid email is required.",
                statusCode: StatusCodes.Status400BadRequest);

        if (!UserPassword.TryParseFromUserInput(request.Password, out var password))
            return Results.Problem(
                title: "Invalid Request",
                detail: UserPassword.GetPasswordRestrictions(),
                statusCode: StatusCodes.Status400BadRequest);

        if (await authService.UserExistsAsync(email, ct))
            return Results.Problem(
                title: "Invalid Request",
                detail: "User already exists.",
                statusCode: StatusCodes.Status400BadRequest);

        await authService.CreateUserAsync(email, password);

        var version = context.GetRequestedApiVersion() ?? new ApiVersion(majorVersion: 0);
        var endpointName = LoginEndpoint.GetNameWithVersion(version);
        context.Response.Headers.Location = linkGenerator.GetUriByName(context, endpointName);

        return Results.Created();
    }

    private record RegisterRequest(string Email, string Password);
}
