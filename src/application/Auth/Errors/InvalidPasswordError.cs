using System.Net;

using LinkForge.Domain.Shared;

namespace LinkForge.Application.Auth.Errors;

public record InvalidPasswordError
    : Error
{
    public InvalidPasswordError(string passwordRestrictions)
        : base(passwordRestrictions, HttpStatusCode.BadRequest)
    {
    }
}
