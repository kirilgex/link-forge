using LinkForge.Domain.Links.ValueObjects;
using LinkForge.Domain.Users;

using MongoDB.Bson;

namespace LinkForge.Domain.Links;

public class Link
{
    public ObjectId Id { get; init; }

    public required ObjectId OwnerId { get; init; }

    public User? Owner { get; init; }

    public required LinkCode Code { get; init; }

    public required LinkUrl Url { get; init; }
}
