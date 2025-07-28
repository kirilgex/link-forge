namespace LinkForge.Application.Auth.Settings;

public record AuthSettings(
    string Issuer,
    string Audience,
    AuthTokenSettings AccessToken,
    AuthTokenSettings RefreshToken);

public record AuthTokenSettings(
    string SecretKey,
    int ExpirationTimeInMinutes);
