using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.Users;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Domain.Links;

public record Link
{
    public EntityId Id { get; init; }

    public EntityId OwnerId { get; init; }

    public User? Owner { get; init; }

    public LinkCode Code { get; init; }

    public LinkUrl Url { get; init; }

    public Link(
        EntityId ownerId,
        LinkCode code,
        LinkUrl url)
    {
        OwnerId = ownerId;
        Code = code;
        Url = url;
    }

    public Link(
        EntityId id,
        EntityId ownerId,
        User? owner,
        LinkCode code,
        LinkUrl url)
    {
        Id = id;
        OwnerId = ownerId;
        Owner = owner;
        Code = code;
        Url = url;   
    }
}
