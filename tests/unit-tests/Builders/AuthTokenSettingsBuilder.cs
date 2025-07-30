using LinkForge.Application.Auth.Settings;

namespace LinkForge.UnitTests.Builders;

public class AuthTokenSettingsBuilder
{
    private string _secretKey = string.Empty;
    private int _expirationTimeInMinutes;

    public AuthTokenSettings Build()
    {
        return new AuthTokenSettings
        {
            SecretKey = _secretKey,
            ExpirationTimeInMinutes = _expirationTimeInMinutes
        };
    }

    public AuthTokenSettingsBuilder WithSecretKey(string secretKey)
    {
        _secretKey = secretKey;
        return this;
    }

    public AuthTokenSettingsBuilder WithExpirationTimeInMinutes(int expirationTimeInMinutes)
    {
        _expirationTimeInMinutes = expirationTimeInMinutes;
        return this;
    }
}