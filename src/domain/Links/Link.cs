using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Links;

public record Link
{
    public EntityId Id { get; set; }

    public LinkCode Code { get; set; }

    public LinkOriginalUrl OriginalUrl { get; set; }
}
