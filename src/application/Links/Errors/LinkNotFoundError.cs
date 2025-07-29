using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Errors;

public record LinkNotFoundError()
    : Error("", HttpStatusCode.NotFound);