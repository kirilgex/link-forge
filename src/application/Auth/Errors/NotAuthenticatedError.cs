using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record NotAuthenticatedError()
    : Error("Invalid email or password", HttpStatusCode.Unauthorized);