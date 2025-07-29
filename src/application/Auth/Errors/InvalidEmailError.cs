using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record InvalidEmailError()
    : Error("Invalid email address.", HttpStatusCode.BadRequest);