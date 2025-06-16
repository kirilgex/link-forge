using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Users;

public record User
{
    public EntityId Id { get; init; }

    public UserEmail Email { get; init; }

    public string? PasswordHash { get; set; }

    public User(UserEmail email)
    {
        Email = email;
    }

    public User(
        EntityId id,
        UserEmail email,
        string? passwordHash)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;    
    }
}
