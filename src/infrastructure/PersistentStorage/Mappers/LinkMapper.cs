using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueTypes;
using LinkForge.Domain.ValueTypes;
using LinkForge.Infrastructure.PersistentStorage.Documents;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using MongoDB.Bson;

namespace LinkForge.Infrastructure.PersistentStorage.Mappers;

internal class LinkMapper(
    UserMapper userMapper)
{
    public LinkDocument ToDocument(Link model)
    {
        return new LinkDocument
        {
            OwnerId = ObjectId.Parse(model.OwnerId.ToString()),
            Code = model.Code.ToString(),
            Url = model.Url.ToString(),
        };
    }

    public Link ToDomainModel(LinkWithOwnerDto document)
    {
        return new Link(
            id: new EntityId(document.Id.ToString()),
            ownerId: new EntityId(document.OwnerId.ToString()),
            owner: userMapper.ToDomainModel(document.Owner),
            code: new LinkCode(document.Code),
            url: new LinkUrl(document.Url));
    }
}