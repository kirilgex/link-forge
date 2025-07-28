using LinkForge.Application.Auth.Services.Implementations;
using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Application.Links.Services.Implementations;
using LinkForge.Application.Links.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace LinkForge.Application;

public static class ApplicationLayerConfiguration
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashingService, MurmurHashingService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILinksProcessService, LinksProcessService>();
        services.AddScoped<ILinksLookupService, LinksLookupService>();

        return services;
    }
}
