using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Users;

public record User
{
    public EntityId Id { get; set; }

    public required UserEmail Email { get; set; }

    public required string PasswordHash { get; set; }
}
