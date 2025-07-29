using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record InvalidRefreshTokenError()
    : Error("Invalid refresh token.", HttpStatusCode.Unauthorized);