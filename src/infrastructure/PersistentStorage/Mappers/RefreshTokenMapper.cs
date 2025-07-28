using LinkForge.Domain.Users;
using LinkForge.Domain.Users.ValueObjects;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Mappers;

internal class RefreshTokenMapper(
    UserMapper userMapper)
{
    public RefreshTokenDocument ToDocument(RefreshToken model)
    {
        return new RefreshTokenDocument
        {
            Id = model.Id,
            UserId = ObjectId.Parse(model.User.Id.ToString()),
            UserAgent = model.UserAgent.ToString(),
            TokenHash = model.TokenHash,
        };
    }

    public RefreshToken ToModel(RefreshTokenWithUserDto document)
    {
        return new RefreshToken
        {
            Id = document.Id,
            User = userMapper.ToModel(document.User),
            UserAgent = new UserAgent(document.UserAgent),
            TokenHash = document.TokenHash,
        };
    }
}