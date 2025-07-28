using MongoDB.Bson.Serialization.Attributes;

namespace LinkForge.Infrastructure.PersistentStorage.Documents;

internal class UserDocument : AbstractDocument
{
    public const string CollectionName = "users";
    
    [BsonElement("email")]
    public required string Email { get; init; }
    
    [BsonElement("passwordHash")]
    public required string PasswordHash { get; init; }
}