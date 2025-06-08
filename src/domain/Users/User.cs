using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Users;

public record User
{
    public EntityId Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }
}
