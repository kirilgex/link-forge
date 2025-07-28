using LinkForge.Infrastructure.PersistentStorage.Documents;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

internal class RefreshTokenWithUserDto : RefreshTokenDocument
{
    public required UserDocument User { get; init; }
}
