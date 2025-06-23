using LinkForge.Application.Entities;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record RefreshTokenDto
{
    public ObjectId Id { get; init; }
    public ObjectId UserId { get; init; }
    public required string UserAgent { get; init; }
    public required string TokenHash { get; init; }

    public static explicit operator RefreshTokenDto(RefreshToken source)
        => new()
        {
            Id = source.Id.Value is null ? ObjectId.Empty : ObjectId.Parse(source.Id.ToString()),
            UserId = ObjectId.Parse(source.User.Id.ToString()),
            UserAgent = source.UserAgent.ToString(),
            TokenHash = source.TokenHash,
        };
}
