using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.Users;
using LinkForge.Domain.ValueTypes;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record LinkDto
{
    public ObjectId Id { get; init; }

    public required ObjectId OwnerId { get; init; }

    public UserDto? Owner { get; init; }

    public required string Code { get; init; }

    public required string Url { get; init; }

    public static explicit operator LinkDto(Link source)
        => new()
        {
            OwnerId = ObjectId.Parse(source.OwnerId.ToString()),
            Code = source.Code,
            Url  = source.Url,
        };

    public static explicit operator Link?(LinkDto? source)
        => source is null
            ? null
            : new Link(
                (EntityId)source.Id.ToString(),
                (EntityId)source.OwnerId.ToString(),
                (User?)source.Owner,
                (LinkCode)source.Code,
                (LinkUrl)source.Url);
}
