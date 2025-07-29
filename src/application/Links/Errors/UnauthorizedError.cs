using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Errors;

public record UnauthorizedError() : Error("", HttpStatusCode.Unauthorized);