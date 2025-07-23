using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkForge.Infrastructure.PersistentStorage.Documents;

internal class LinkDocument : AbstractDocument
{
    public const string CollectionName = "links";
    
    [BsonElement("ownerId")]
    public ObjectId OwnerId { get; init; }

    [BsonElement("code")]
    public required string Code { get; init; }

    [BsonElement("url")]
    public required string Url { get; init; }
}