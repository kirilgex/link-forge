using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueObjects;
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
            OwnerId = model.OwnerId,
            Code = model.Code,
            Url = model.Url,
        };
    }

    public Link ToModel(LinkWithOwnerDto document)
    {
        return new Link
        {
            Id = document.Id,
            OwnerId = document.OwnerId,
            Owner = userMapper.ToModel(document.Owner),
            Code = LinkCode.FromTrusted(document.Code),
            Url = LinkUrl.FromTrusted(document.Url),
        };
    }
}