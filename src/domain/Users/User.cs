using LinkForge.Domain.Users.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.Domain.Users;

public class User
{
    public ObjectId Id { get; init; }

    public required UserEmail Email { get; init; }

    public string PasswordHash { get; set; } = string.Empty;
}
