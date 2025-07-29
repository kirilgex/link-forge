using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record InvalidCredentialsError()
    : Error("Invalid credentials.", HttpStatusCode.BadRequest);