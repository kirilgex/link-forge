using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

public abstract class BaseRepository<T>
{
    protected IMongoCollection<T> Collection { get; }

    protected BaseRepository(IOptions<DatabaseSettings> settings, string collectionName)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        Collection = database.GetCollection<T>(collectionName);
    }
}
