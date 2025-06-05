using LinkForge.Application.Services.Implementations;
using LinkForge.Application.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace LinkForge.Application;

public static class ApplicationLayerConfiguration
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashingService, MurmurHashingService>();

        services.AddScoped<ILinksProcessService, LinksProcessService>();
        services.AddScoped<ILinksLookupService, LinksLookupService>();

        return services;
    }
}
