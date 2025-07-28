using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Domain.Links;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Dto;
using LinkForge.Infrastructure.PersistentStorage.Mappers;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class LinksRepository(
    IOptions<DatabaseSettings> settings,
    LinkMapper mapper)
    :
        AbstractRepository<LinkDocument>(settings, LinkDocument.CollectionName),
        ILinksRepository
{
    public async Task<Link?> FindAsync(
        string code,
        CancellationToken ct = default)
    {
        BsonDocument[] pipeline =
        [
            new("$match", new BsonDocument("code", code)),
            new("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "ownerId" },
                { "foreignField", "_id" },
                { "as", "Owner" },
            }),
            new("$unwind", "$Owner"),
            new("$limit", 1)
        ];

        var result = await Collection
            .Aggregate<LinkWithOwnerDto>(pipeline, cancellationToken: ct)
            .FirstOrDefaultAsync(ct);

        return result is null ? null : mapper.ToModel(result);
    }
    
    public async Task InsertAsync(
        Link link,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            mapper.ToDocument(link),
            options: null,
            ct);
    }
}
