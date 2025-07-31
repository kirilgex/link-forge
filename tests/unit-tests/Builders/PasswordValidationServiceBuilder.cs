using LinkForge.Application.Auth.Services.Implementations;
using LinkForge.Application.Auth.Settings;
using Microsoft.Extensions.Options;

namespace LinkForge.UnitTests.Builders;

public class PasswordValidationServiceBuilder
{
    private AuthSettings _authSettings = new AuthSettingsBuilder().Build();
    
    public PasswordValidationService Build()
    {
        return new PasswordValidationService(Options.Create(_authSettings));
    }

    public PasswordValidationServiceBuilder WithAuthSettings(AuthSettings authSettings)
    {
        _authSettings = authSettings;
        return this;
    }

    public PasswordValidationServiceBuilder WithAuthSettings(Action<AuthSettingsBuilder> builder)
    {
        var authSettingsBuilder = new AuthSettingsBuilder();
        builder(authSettingsBuilder);
        _authSettings = authSettingsBuilder.Build();
        return this;
    }
}