using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.Users;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Links;

public record Link
{
    public EntityId Id { get; set; }

    public EntityId OwnerId { get; set; }

    public User? Owner { get; set; }

    public LinkCode Code { get; set; }

    public LinkOriginalUrl OriginalUrl { get; set; }
}
