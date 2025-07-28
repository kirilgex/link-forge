using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal abstract class AbstractRepository<T>
{
    protected IMongoCollection<T> Collection { get; }

    protected AbstractRepository(IOptions<DatabaseSettings> settings, string collectionName)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        Collection = database.GetCollection<T>(collectionName);
    }
}
