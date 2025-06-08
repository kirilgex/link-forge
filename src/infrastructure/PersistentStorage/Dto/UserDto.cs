using LinkForge.Domain.Users;
using LinkForge.Domain.ValueTypes;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record UserDto
{
    public ObjectId Id { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }

    public static explicit operator UserDto(User source)
        => new()
        {
            Email = source.Email,
            PasswordHash  = source.PasswordHash,
        };

    public static explicit operator User?(UserDto? source)
        => source is null
            ? null
            : new User
            {
                Id = (EntityId)source.Id.ToString(),
                Email = source.Email,
                PasswordHash = source.PasswordHash,
            };
}
