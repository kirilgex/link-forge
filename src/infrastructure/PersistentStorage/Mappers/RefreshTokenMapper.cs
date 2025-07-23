using LinkForge.Application.Entities;
using LinkForge.Domain.ValueTypes;
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
            Id = model.Id is null ? default : ObjectId.Parse(model.Id.ToString()),
            UserId = ObjectId.Parse(model.User.Id.ToString()),
            UserAgent = model.UserAgent.ToString(),
            TokenHash = model.TokenHash,
        };
    }

    public RefreshToken ToDomainModel(RefreshTokenWithUserDto document)
    {
        return new RefreshToken(
            id: new EntityId(document.Id.ToString()),
            user: userMapper.ToDomainModel(document.User),
            userAgent: new UserAgent(document.UserAgent),
            tokenHash: document.TokenHash);
    }
}