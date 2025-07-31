using LinkForge.Application.Auth.PersistentStorageAccess;
using LinkForge.Application.Auth.Services.Implementations;
using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Application.Auth.Settings;

using Microsoft.Extensions.Options;

using Moq;

namespace LinkForge.UnitTests.Builders;

public class AuthServiceBuilder
{
    private AuthSettings _authSettings = new AuthSettingsBuilder().Build();
    private IPasswordValidationService _passwordValidationService = new PasswordValidationServiceBuilder().Build();
    private IUsersRepository _usersRepository = Mock.Of<IUsersRepository>();
    private IRefreshTokensRepository _refreshTokensRepository = Mock.Of<IRefreshTokensRepository>();
    
    public AuthService Build()
    {
        return new AuthService(
            Options.Create(_authSettings),
            _passwordValidationService,
            _usersRepository,
            _refreshTokensRepository);
    }

    public AuthServiceBuilder WithAuthSettings(AuthSettings authSettings)
    {
        _authSettings = authSettings;
        return this;
    }

    public AuthServiceBuilder WithPasswordValidationService(IPasswordValidationService passwordValidationService)
    {
        _passwordValidationService = passwordValidationService;
        return this;
    }
    
    public AuthServiceBuilder WithUsersRepository(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        return this;
    }

    public AuthServiceBuilder WithRefreshTokensRepository(IRefreshTokensRepository refreshTokensRepository)
    {
        _refreshTokensRepository = refreshTokensRepository;
        return this;
    }
}