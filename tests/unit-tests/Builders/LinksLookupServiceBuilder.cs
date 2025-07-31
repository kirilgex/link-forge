using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Implementations;

using Moq;

namespace LinkForge.UnitTests.Builders;

public class LinksLookupServiceBuilder
{
    private ILinksRepository _linksRepository = Mock.Of<ILinksRepository>();
    
    public LinksLookupService Build()
    {
        return new LinksLookupService(_linksRepository);
    }
    
    public LinksLookupServiceBuilder WithLinksRepository(ILinksRepository linksRepository)
    {
        _linksRepository = linksRepository;
        return this;
    }
}