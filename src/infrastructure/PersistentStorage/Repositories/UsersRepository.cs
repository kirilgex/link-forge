using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class UsersRepository(IOptions<DatabaseSettings> settings)
    : BaseRepository<UserDto>(settings, CollectionName), IUsersRepository
{
    public const string CollectionName = "users";

    public async Task<bool> ExistsAsync(
        UserEmail email,
        CancellationToken ct = default)
    {
        return await Collection
            .Find(x => x.Email == email.ToString())
            .AnyAsync(ct);
    }

    public async Task<User?> FindAsync(
        UserEmail email,
        CancellationToken ct = default)
    {
        var result = await Collection
            .Find(x => x.Email == email.ToString())
            .FirstOrDefaultAsync(ct);
        return result.ToUser();
    }

    public async Task<User?> FindAsync(
        EntityId userId,
        CancellationToken ct = default)
    {
        var result = await Collection
            .Find(x => x.Id == ObjectId.Parse(userId.ToString()))
            .FirstOrDefaultAsync(ct);
        return result.ToUser();
    }
    
    public async Task InsertAsync(
        User user,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            (UserDto)user,
            options: null,
            ct);
    }
}
