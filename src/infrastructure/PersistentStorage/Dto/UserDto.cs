using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record UserDto
{
    public ObjectId Id { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }

    public virtual User ToUser()
        => new(
            id: new EntityId(Id.ToString()),
            email: new UserEmail(Email),
            passwordHash: PasswordHash);

    public static explicit operator UserDto(User source)
        => new()
        {
            Email = source.Email.ToString(),
            PasswordHash  = source.PasswordHash!,
        };
}
