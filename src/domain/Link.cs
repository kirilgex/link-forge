using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain;

public record Link
{
    public LinkId Id { get; set; }

    public LinkCode Code { get; set; }

    public LinkOriginalUrl OriginalUrl { get; set; }
}
