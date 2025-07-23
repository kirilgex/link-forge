using LinkForge.Infrastructure.PersistentStorage.Documents;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

internal class LinkWithOwnerDto : LinkDocument
{
    public required UserDocument Owner { get; init; }
}
