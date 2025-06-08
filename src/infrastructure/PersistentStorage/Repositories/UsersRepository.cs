using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using Microsoft.Extensions.Options;

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
            .Find(x => x.Email == email)
            .AnyAsync(ct);
    }

    public async Task<User?> FindAsync(
        UserEmail email,
        CancellationToken ct = default)
    {
        return (User?)await Collection
            .Find(x => x.Email == email)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task InsertAsync(
        User user,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            (UserDto) user,
            options: null,
            ct);
    }
}
