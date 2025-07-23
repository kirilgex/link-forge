using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueTypes;
using LinkForge.Domain.ValueTypes;
using LinkForge.Infrastructure.PersistentStorage.Documents;

namespace LinkForge.Infrastructure.PersistentStorage.Mappers;

internal class UserMapper
{
    public UserDocument ToDocument(User model)
    {
        return new UserDocument
        {
            Email = model.Email.ToString(),
            PasswordHash = model.PasswordHash,
        };
    }

    public User ToDomainModel(UserDocument document)
    {
        return new User(
            id: new EntityId(document.Id.ToString()),
            email: new UserEmail(document.Email),
            passwordHash: document.PasswordHash);
    }
}