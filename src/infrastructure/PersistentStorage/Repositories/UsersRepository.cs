using LinkForge.Application.Repositories;
using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Mappers;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

internal sealed class UsersRepository(
    IOptions<DatabaseSettings> settings,
    UserMapper mapper)
    :
        AbstractRepository<UserDocument>(settings, UserDocument.CollectionName),
        IUsersRepository
{
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
        var result = await Collection
            .Find(x => x.Email == email)
            .FirstOrDefaultAsync(ct);
        
        return result is null ? null : mapper.ToModel(result);
    }
    
    public async Task InsertAsync(
        User user,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            mapper.ToDocument(user),
            options: null,
            ct);
    }
}
