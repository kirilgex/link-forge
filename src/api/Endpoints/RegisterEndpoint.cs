using Asp.Versioning;

using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
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
        IUsersRepository usersRepository, // TODO: to separate service
        HttpContext context,
        CancellationToken ct = default)
    {
        if (!UserEmail.TryParseFromUserInput(request.Email, out var email))
            return Results.Problem(
                title: "Invalid Request",
                detail: "Valid email is required.",
                statusCode: StatusCodes.Status400BadRequest);

        if (!Password.TryParseFromUserInput(request.Password, out var password))
            return Results.Problem(
                title: "Invalid Request",
                detail: Password.GetPasswordRestrictions(),
                statusCode: StatusCodes.Status400BadRequest);

        if (await usersRepository.FindAsync(request.Email, ct) is not null)
            return Results.Problem(
                title: "Invalid Request",
                detail: "User already exists.",
                statusCode: StatusCodes.Status400BadRequest);

        var user = new User()
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };

        await usersRepository.InsertAsync(user);

        return Results.Ok();
    }

    private record RegisterRequest(string Email, string Password);
}
