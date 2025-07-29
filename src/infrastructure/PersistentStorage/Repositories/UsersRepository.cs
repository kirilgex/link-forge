using LinkForge.Application.Auth.Errors;
using LinkForge.Application.Auth.PersistentStorageAccess;
using LinkForge.Domain.Shared;
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
    public async Task<Result<User>> FindAsync(
        UserEmail email,
        CancellationToken ct = default)
    {
        var result = await Collection
            .Find(x => x.Email == email)
            .FirstOrDefaultAsync(ct);

        return result is null
            ? Result<User>.Failure(new NotAuthenticatedError())
            : Result<User>.Success(mapper.ToModel(result));
    }
    
    public async Task<Result> InsertAsync(
        User user,
        CancellationToken ct = default)
    {
        try
        {
            await Collection.InsertOneAsync(
                mapper.ToDocument(user),
                options: null,
                ct);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category is ServerErrorCategory.DuplicateKey)
        {
            return Result.Failure(new UserAlreadyExistsError());
        }

        return Result.Success();
    }
}
