using LinkForge.Application.Repositories;
using LinkForge.Domain;
using LinkForge.Infrastructure.PersistentStorage.Dto;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace LinkForge.Infrastructure.PersistentStorage.Repositories;

public class LinksRepository(IOptions<DatabaseSettings> settings)
    : BaseRepository<LinkDto>(settings, CollectionName), ILinksRepository
{
    public const string CollectionName = "links";

    public async Task<Link?> FindAsync(
        string code,
        CancellationToken ct = default)
    {
        return (Link?) await Collection
            .Find(x => x.Code == code)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task InsertAsync(
        Link link,
        CancellationToken ct = default)
    {
        await Collection.InsertOneAsync(
            (LinkDto) link,
            options: null,
            ct);
    }
}
