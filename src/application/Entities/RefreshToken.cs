using LinkForge.Domain.Users;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Application.Entities;

public record RefreshToken
{
    public EntityId? Id { get; init; }

    public User User { get; init; }

    public UserAgent UserAgent { get; init; }

    public string TokenHash { get; init; }

    public RefreshToken(
        EntityId? id,
        User user,
        UserAgent userAgent,
        string tokenHash)
    {
        Id = id;
        User = user;
        UserAgent = userAgent;
        TokenHash = tokenHash;
    }
}
