using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Links.Errors;

public record InvalidUrlError()
    : Error("Invalid url.", HttpStatusCode.BadRequest);