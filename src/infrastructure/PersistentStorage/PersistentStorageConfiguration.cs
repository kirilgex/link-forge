using LinkForge.Application.Repositories;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Mappers;
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
        services.AddSingleton<UserMapper>();
        services.AddSingleton<RefreshTokenMapper>();
        services.AddSingleton<LinkMapper>();
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        services.AddScoped<ILinksRepository, LinksRepository>();
        
        return services;
    }

    public static async Task ConfigurePersistentStorageAsync(this IServiceProvider app)
    {
        using var scope = app.CreateScope();

        var settings = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();

        RegisterUserClassMap();
        RegisterLinkClassMap();
        RegisterRefreshTokenClassMap();

        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        await EnsureUsersCollectionIndexes(
            database.GetCollection<UserDocument>(UserDocument.CollectionName));
        await EnsureLinksCollectionIndexes(
            database.GetCollection<LinkDocument>(LinkDocument.CollectionName));
        await EnsureRefreshTokensCollectionIndexes(
            database.GetCollection<RefreshTokenDocument>(RefreshTokenDocument.CollectionName));
    }

    private static void RegisterUserClassMap()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(UserDocument)))
        {
            BsonClassMap.RegisterClassMap<UserDocument>(cm =>
            {
                cm.AutoMap();
            });
        }
    }

    private static void RegisterLinkClassMap()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(LinkDocument)))
        {
            BsonClassMap.RegisterClassMap<LinkDocument>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
    
    private static void RegisterRefreshTokenClassMap()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(RefreshTokenDocument)))
        {
            BsonClassMap.RegisterClassMap<RefreshTokenDocument>(cm =>
            {
                cm.AutoMap();
            });
        }
    }

    private static async Task EnsureUsersCollectionIndexes(IMongoCollection<UserDocument> collection)
    {
        var indexes = new[]
        {
            new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions { Unique = true, })
        };
        
        await collection.Indexes.CreateManyAsync(indexes);
    }

    private static async Task EnsureLinksCollectionIndexes(IMongoCollection<LinkDocument> collection)
    {
        var indexes = new[]
        {
            new CreateIndexModel<LinkDocument>(
                Builders<LinkDocument>.IndexKeys.Ascending(x => x.Code),
                new CreateIndexOptions { Unique = true, })
        };
        
        await collection.Indexes.CreateManyAsync(indexes);
    }

    private static async Task EnsureRefreshTokensCollectionIndexes(IMongoCollection<RefreshTokenDocument> collection)
    {
        var indexes = new[]
        {
            new CreateIndexModel<RefreshTokenDocument>(
                Builders<RefreshTokenDocument>.IndexKeys.Ascending(x => x.UserId).Ascending(x => x.UserAgent),
                new CreateIndexOptions { Unique = true, })
        };
        
        await collection.Indexes.CreateManyAsync(indexes);
    }
}
