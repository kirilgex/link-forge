using LinkForge.Application.Entities;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record RefreshTokenWithUserDto : RefreshTokenDto
{
    public required UserDto User { get; init; }

    public virtual RefreshToken ToRefreshToken()
        => new(
            id: new EntityId(Id.ToString()),
            user: User.ToUser(),
            userAgent: new UserAgent(UserAgent),
            tokenHash: TokenHash);
}
