namespace LinkForge.Application.Settings;

public record AuthSettings
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required AuthTokenSettings AccessToken { get; init; }
    public required AuthTokenSettings RefreshToken { get; init; }
}

public record AuthTokenSettings
{
    public required string SecretKey { get; init; }
    public required int ExpirationTimeInMinutes { get; init; }
}
