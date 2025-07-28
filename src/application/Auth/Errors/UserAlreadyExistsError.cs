using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record UserAlreadyExistsError()
    : Error("User already exists.", HttpStatusCode.Conflict);
