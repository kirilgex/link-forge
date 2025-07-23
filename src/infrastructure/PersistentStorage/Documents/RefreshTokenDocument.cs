using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkForge.Infrastructure.PersistentStorage.Documents;

internal class RefreshTokenDocument : AbstractDocument
{
    public const string CollectionName = "refreshTokens";
    
    [BsonElement("userId")]
    public ObjectId UserId { get; init; }
    
    [BsonElement("userAgent")]
    public required string UserAgent { get; init; }
    
    [BsonElement("tokenHash")]
    public required string TokenHash { get; init; }
}