using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class UsersRepository(IOptions<DatabaseSettings> settings)
    : BaseRepository<UserDto>(settings, CollectionName), IUsersRepository
{
    public const string CollectionName = "users";

    public async Task<User?> FindAsync(
        string email,
        CancellationToken ct = default)
    {
        return (User?) await Collection
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
