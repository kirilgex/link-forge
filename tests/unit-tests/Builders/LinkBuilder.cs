using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueObjects;

using MongoDB.Bson;

namespace LinkForge.UnitTests.Builders;

public class LinkBuilder
{
    private ObjectId _id = ObjectId.GenerateNewId();
    private ObjectId _ownerId = ObjectId.GenerateNewId();
    private LinkCode _code = LinkCode.FromTrusted(string.Empty);
    private LinkUrl _url = LinkUrl.FromTrusted(string.Empty);

    public Link Build()
    {
        return new Link
        {
            Id = _id,
            OwnerId = _ownerId,
            Code = _code,
            Url = _url,
        };
    }

    public LinkBuilder WithId(ObjectId id)
    {
        _id = id;
        return this;
    }

    public LinkBuilder WithOwnerId(ObjectId ownerId)
    {
        _ownerId = ownerId;
        return this;
    }
    
    public LinkBuilder WithCode(LinkCode code)
    {
        _code = code;
        return this;
    }
    
    public LinkBuilder WithUrl(LinkUrl url)
    {
        _url = url;
        return this;
    }
}