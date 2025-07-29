using System.Net;

namespace LinkForge.Domain.Shared;

public abstract record Error(
    string Message,
    HttpStatusCode StatusCode = HttpStatusCode.InternalServerError);
    