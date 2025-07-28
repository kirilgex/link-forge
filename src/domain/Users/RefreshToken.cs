using LinkForge.Domain.Users.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.Domain.Users;

public class RefreshToken
{
    public ObjectId Id { get; init; }

    public required User User { get; init; }

    public required UserAgent UserAgent { get; init; }

    public required string TokenHash { get; init; }
}
