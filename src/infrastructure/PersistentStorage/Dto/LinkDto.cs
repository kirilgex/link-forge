using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record LinkDto
{
    public ObjectId Id { get; init; }
    public required ObjectId OwnerId { get; init; }
    public required string Code { get; init; }
    public required string Url { get; init; }

    public virtual Link ToLink()
        => new(
            id: new EntityId(Id.ToString()),
            ownerId: new EntityId(OwnerId.ToString()),
            owner: null,
            code: new LinkCode(Code),
            url: new LinkUrl(Url));

    public static explicit operator LinkDto(Link source)
        => new()
        {
            OwnerId = ObjectId.Parse(source.OwnerId.ToString()),
            Code = source.Code.ToString(),
            Url = source.Url.ToString(),
        };
}
