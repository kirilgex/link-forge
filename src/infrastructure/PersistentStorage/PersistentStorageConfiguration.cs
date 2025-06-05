using LinkForge.Application.Repositories;
using LinkForge.Infrastructure.PersistentStorage.Dto;
using LinkForge.Infrastructure.PersistentStorage.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage;

public static class PersistentStorageConfiguration
{
    public static IServiceCollection AddPersistentStorageServices(this IServiceCollection services)
    {
        services.AddScoped<ILinksRepository, LinksRepository>();
        return services;
    }

    public static async Task ConfigurePersistentStorageAsync(this IServiceProvider app)
    {
        using var scope = app.CreateScope();

        var settings = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();

        RegisterLinkClassMap();
        
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        await EnsureLinksCollectionIndexes(
            database.GetCollection<LinkDto>(LinksRepository.CollectionName));
    }
    
    private static void RegisterLinkClassMap()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(LinkDto)))
        {
            BsonClassMap.RegisterClassMap<LinkDto>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.Id);
                cm.MapMember(x => x.Code).SetElementName("code");
                cm.MapMember(x => x.OriginalUrl).SetElementName("originalUrl");
            });
        }
    }

    private static async Task EnsureLinksCollectionIndexes(IMongoCollection<LinkDto> collection)
    {
        var indexes = new[]
        {
            new CreateIndexModel<LinkDto>(
                Builders<LinkDto>.IndexKeys.Ascending(x => x.Code),
                new CreateIndexOptions { Unique = true, })
        };
        
        await collection.Indexes.CreateManyAsync(indexes);
    }

}
