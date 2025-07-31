using LinkForge.Application.Auth.Settings;

namespace LinkForge.UnitTests.Builders;

public class AuthSettingsBuilder
{
    private string _issuer = string.Empty;
    private string _audience = string.Empty;
    private AuthTokenSettings _accessToken = new AuthTokenSettingsBuilder().Build();
    private AuthTokenSettings _refreshToken = new AuthTokenSettingsBuilder().Build();
    private PasswordRestrictions _passwordRestrictions = new PasswordRestrictionsBuilder().Build();

    public AuthSettings Build()
    {
        return new AuthSettings
        {
            Issuer = _issuer,
            Audience = _audience,
            AccessToken = _accessToken,
            RefreshToken = _refreshToken,
            PasswordRestrictions = _passwordRestrictions
        };
    }

    public AuthSettingsBuilder WithIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public AuthSettingsBuilder WithAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    public AuthSettingsBuilder WithAccessToken(Action<AuthTokenSettingsBuilder> builder)
    {
        var authTokenSettingsBuilder = new AuthTokenSettingsBuilder();
        builder(authTokenSettingsBuilder);
        _accessToken = authTokenSettingsBuilder.Build();
        return this;
    }

    public AuthSettingsBuilder WithRefreshToken(Action<AuthTokenSettingsBuilder> builder)
    {
        var authTokenSettingsBuilder = new AuthTokenSettingsBuilder();
        builder(authTokenSettingsBuilder);
        _refreshToken = authTokenSettingsBuilder.Build();
        return this;
    }

    public AuthSettingsBuilder WithPasswordRestrictions(Action<PasswordRestrictionsBuilder> builder)
    {
        var restrictionsBuilder = new PasswordRestrictionsBuilder();
        builder(restrictionsBuilder);
        _passwordRestrictions = restrictionsBuilder.Build();
        return this;
    }
}