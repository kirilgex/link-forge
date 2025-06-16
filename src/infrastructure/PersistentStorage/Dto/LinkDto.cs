using System.Text.Json.Serialization;

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

    public required string OriginalUrl { get; init; }

    public static explicit operator LinkDto(Link source)
        => new()
        {
            OwnerId = ObjectId.Parse(source.OwnerId.ToString()),
            Code = source.Code,
            OriginalUrl  = source.OriginalUrl,
        };

    public static explicit operator Link?(LinkDto? source)
        => source is null
            ? null
            : new Link
            {
                Id = (EntityId)source.Id.ToString(),
                OwnerId = (EntityId)source.OwnerId.ToString(),
                Owner = (User?)source.Owner,
                Code = (LinkCode)source.Code,
                OriginalUrl = (LinkOriginalUrl)source.OriginalUrl,
            };
}
