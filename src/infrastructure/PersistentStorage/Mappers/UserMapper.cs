using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;
using LinkForge.Infrastructure.PersistentStorage.Documents;

namespace LinkForge.Infrastructure.PersistentStorage.Mappers;

internal class UserMapper
{
    public UserDocument ToDocument(User model)
    {
        return new UserDocument
        {
            Email = model.Email,
            PasswordHash = model.PasswordHash,
        };
    }

    public User ToModel(UserDocument document)
    {
        return new User
        {
            Id = document.Id,
            Email = UserEmail.FromTrusted(document.Email),
            PasswordHash = document.PasswordHash,
        };
    }
}