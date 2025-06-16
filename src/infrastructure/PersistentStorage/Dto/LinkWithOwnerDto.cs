using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record LinkWithOwnerDto : LinkDto
{
    public UserDto? Owner { get; init; }

    public override Link ToLink()
        => new(
            id: new EntityId(Id.ToString()),
            ownerId: new EntityId(OwnerId.ToString()),
            owner: Owner?.ToUser(),
            code: new LinkCode(Code),
            url: new LinkUrl(Url));

    public static explicit operator Link?(LinkWithOwnerDto? source)
        => source is null
            ? null
            : new Link(
                new EntityId(source.Id.ToString()),
                new EntityId(source.OwnerId.ToString()),
                source.Owner?.ToUser(),
                new LinkCode(source.Code),
                new LinkUrl(source.Url));
}
