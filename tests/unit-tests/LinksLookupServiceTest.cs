using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Errors;
using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Domain.Links;
using LinkForge.Domain.Links.ValueObjects;
using LinkForge.Domain.Shared;
using LinkForge.UnitTests.Builders;

using Moq;

namespace LinkForge.UnitTests;

public class LinksLookupServiceTest
{
    [Fact]
    public async Task FindLinkAsync_WithValidCode_ReturnsSuccessResult()
    {
        const string code = "test123";
        const string expectedUrl = "https://example.com";
        
        var link = new LinkBuilder()
            .WithCode(LinkCode.FromTrusted(code))
            .WithUrl(LinkUrl.FromTrusted(expectedUrl))
            .Build();
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        linksRepositoryMock
            .Setup(x => x.FindAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Link>.Success(link));
        
        var sut = new LinksLookupServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.FindLinkAsync(code);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUrl, result.Value!.Link);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task FindLinkAsync_WithNullOrWhitespaceCode_ReturnsLinkNotFoundError(string? code)
    {
        var linksRepositoryMock = new Mock<ILinksRepository>();

        var sut = new LinksLookupServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.FindLinkAsync(code!);

        Assert.False(result.IsSuccess);
        Assert.IsType<LinkNotFoundError>(result.Error);
    }

    [Fact]
    public async Task FindLinkAsync_WhenRepositoryReturnsFailure_ReturnsFailureResult()
    {
        const string code = "nonexistent";
        var expectedError = new LinkNotFoundError();

        var linksRepositoryMock = new Mock<ILinksRepository>();
        linksRepositoryMock
            .Setup(x => x.FindAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Link>.Failure(expectedError));

        var sut = new LinksLookupServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.FindLinkAsync(code);

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public async Task FindLinkAsync_MapsLinkUrlCorrectly_ReturnsCorrectResponse()
    {
        const string code = "mapping_test";
        const string linkUrl = "https://mapping-test.com/path?query=value";

        var link = new LinkBuilder()
            .WithCode(LinkCode.FromTrusted(code))
            .WithUrl(LinkUrl.FromTrusted(linkUrl))
            .Build();

        var linksRepositoryMock = new Mock<ILinksRepository>();
        linksRepositoryMock
            .Setup(x => x.FindAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Link>.Success(link));

        var sut = new LinksLookupServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.FindLinkAsync(code);

        Assert.True(result.IsSuccess);
        Assert.IsType<FindLinkResponse>(result.Value);
        Assert.Equal(linkUrl, result.Value!.Link);
    }
}