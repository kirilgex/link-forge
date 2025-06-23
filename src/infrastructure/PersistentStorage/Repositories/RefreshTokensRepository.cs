using LinkForge.Application.Entities;
using LinkForge.Application.Repositories;
using LinkForge.Domain.ValueTypes;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class RefreshTokensRepository(IOptions<DatabaseSettings> settings)
    : BaseRepository<RefreshTokenDto>(settings, CollectionName), IRefreshTokensRepository
{
    public const string CollectionName = "refreshTokens";

    public async Task<RefreshToken?> FindAsync(
        EntityId userId,
        UserAgent userAgent,
        CancellationToken ct = default)
    {
        BsonDocument[] pipeline =
        [
            new("$match", new BsonDocument
            {
                { "userId", ObjectId.Parse(userId.ToString()) },
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

        return result?.ToRefreshToken();
    }

    public async Task ReplaceOneAsync(
        RefreshToken token,
        CancellationToken ct = default)
    {
        var dto = (RefreshTokenDto)token;

        var filter = Builders<RefreshTokenDto>.Filter.And(
            Builders<RefreshTokenDto>.Filter.Eq(x => x.UserId, dto.UserId),
            Builders<RefreshTokenDto>.Filter.Eq(x => x.UserAgent, dto.UserAgent));

        await Collection.ReplaceOneAsync(
            filter: filter,
            replacement: dto,
            options: new ReplaceOptions { IsUpsert = true, },
            cancellationToken: ct);
    }
}
