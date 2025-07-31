using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Implementations;
using LinkForge.Application.Links.Services.Interfaces;

using Moq;

namespace LinkForge.UnitTests.Builders;

public class LinksProcessServiceBuilder
{
    private IHashingService _hashingService = Mock.Of<IHashingService>();
    private ILinksRepository _linksRepository = Mock.Of<ILinksRepository>();
    
    public LinksProcessService Build()
    {
        return new LinksProcessService(_hashingService, _linksRepository);
    }
    
    public LinksProcessServiceBuilder WithHashingService(IHashingService hashingService)
    {
        _hashingService = hashingService;
        return this;
    }
    
    public LinksProcessServiceBuilder WithLinksRepository(ILinksRepository linksRepository)
    {
        _linksRepository = linksRepository;
        return this;
    }
}