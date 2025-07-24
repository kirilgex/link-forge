using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Dto;
using LinkForge.Infrastructure.PersistentStorage.Mappers;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class RefreshTokensRepository(
    IOptions<DatabaseSettings> settings,
    RefreshTokenMapper mapper)
    :
        AbstractRepository<RefreshTokenDocument>(settings, RefreshTokenDocument.CollectionName),
        IRefreshTokensRepository
{
    public async Task<RefreshToken?> FindAsync(
        ObjectId userId,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        BsonDocument[] pipeline =
        [
            new("$match", new BsonDocument
            {
                { "userId", userId },
                { "userAgent", userAgent.ToString() },
            }),
            new("$lookup", new BsonDocument
            {
                { "from", "users" },
                { "localField", "userId" },
                { "foreignField", "_id" },
                { "as", "User" },
            }),
            new("$unwind", "$User"),
            new("$limit", 1)
        ];

        var result = await Collection
            .Aggregate<RefreshTokenWithUserDto>(pipeline, cancellationToken: ct)
            .FirstOrDefaultAsync(ct);

        return result is null ? null : mapper.ToModel(result);
    }
    
    public async Task InsertAsync(
        RefreshToken token,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            mapper.ToDocument(token),
            options: null,
            ct);
    }

    public async Task ReplaceOneAsync(
        RefreshToken token,
        CancellationToken ct = default)
    {
        var document = mapper.ToDocument(token);
        
        var filter = Builders<RefreshTokenDocument>.Filter.And(
            Builders<RefreshTokenDocument>.Filter.Eq(x => x.UserId, document.UserId),
            Builders<RefreshTokenDocument>.Filter.Eq(x => x.UserAgent, document.UserAgent));

        await Collection.ReplaceOneAsync(
            filter: filter,
            replacement: document,
            options: new ReplaceOptions { IsUpsert = true, },
            cancellationToken: ct);
    }
}
