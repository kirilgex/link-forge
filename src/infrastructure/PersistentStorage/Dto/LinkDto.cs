using LinkForge.Domain;
using LinkForge.Domain.ValueTypes;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Dto;

public record class LinkDto
{
    public ObjectId Id { get; init; }
    public required string Code { get; init; }
    public required string OriginalUrl { get; init; }

    public static explicit operator LinkDto(Link source)
        => new()
        {
            Code = source.Code,
            OriginalUrl  = source.OriginalUrl,
        };

    public static explicit operator Link?(LinkDto? source)
        => source is null
            ? null
            : new Link
            {
                Id = (LinkId)source.Id.ToString(),
                Code = (LinkCode)source.Code,
                OriginalUrl = (LinkOriginalUrl)source.OriginalUrl,
            };
}
