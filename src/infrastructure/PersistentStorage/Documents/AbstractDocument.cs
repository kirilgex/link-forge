using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkForge.Infrastructure.PersistentStorage.Documents;

internal abstract class AbstractDocument
{
    [BsonId]
    public ObjectId Id { get; init; }
}