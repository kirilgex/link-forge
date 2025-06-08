using Asp.Versioning;

using LinkForge.Application.Repositories;
using LinkForge.Application.Services.Interfaces;

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
        IUsersRepository usersRepository, // TODO: to separate service
        IJwtTokenService jwtTokenService,
        HttpContext context,
        CancellationToken ct = default)
    {
        var user = await usersRepository.FindAsync(request.Email, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Results.Unauthorized();

        var token = jwtTokenService.GenerateToken(user);

        return Results.Ok(new { token });
    }

    private record LoginRequest(string Email, string Password);
}
