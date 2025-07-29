using LinkForge.Domain.Users.ValueObjects;

namespace LinkForge.API.Extensions;

public static class HttpRequestExtensions
{
    public static UserAgent GetUserAgent(this HttpRequest request)
    {
        request.Headers.TryGetValue("User-Agent", out var userAgent);
        return new UserAgent(userAgent);
    }
}