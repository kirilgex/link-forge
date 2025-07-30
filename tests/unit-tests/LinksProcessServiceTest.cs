using System.Security.Claims;

using LinkForge.Application.Links.Dto;
using LinkForge.Application.Links.Errors;
using LinkForge.Application.Links.PersistentStorageAccess;
using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Domain.Links;
using LinkForge.UnitTests.Builders;

using MongoDB.Bson;

using Moq;

namespace LinkForge.UnitTests;

public class LinksProcessServiceTest
{
    [Theory]
    [InlineData("http://example.com/")]
    [InlineData("https://example.com")]
    [InlineData("ftp://example.com")]
    public async Task ProcessLinkAsync_WithValidRequest_ReturnsSuccessResult(string url)
    {
        const string hashedCode = "abc123";
        var userId = ObjectId.GenerateNewId();
        
        var request = new CreateLinkRequest(url);
        var user = CreateClaimsPrincipal(userId.ToString());
        
        var hashingServiceMock = new Mock<IHashingService>();
        hashingServiceMock
            .Setup(x => x.ComputeHashAsHexString(It.IsAny<string>()))
            .Returns(hashedCode);
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        
        var sut = new LinksProcessServiceBuilder()
            .WithHashingService(hashingServiceMock.Object)
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.ProcessLinkAsync(request, user);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(hashedCode, result.Value);
        
        linksRepositoryMock.Verify(
            x => x.InsertAsync(
                It.Is<Link>(link =>
                    link.OwnerId == userId && link.Code.ToString() == hashedCode && link.Url.ToString() == url), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task ProcessLinkAsync_WithNullOrWhitespaceUrl_ReturnsInvalidUrlError(string? url)
    {
        var userId = ObjectId.GenerateNewId();
        var request = new CreateLinkRequest(url!);
        var user = CreateClaimsPrincipal(userId.ToString());
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        
        var sut = new LinksProcessServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.ProcessLinkAsync(request, user);
        
        Assert.False(result.IsSuccess);
        Assert.IsType<InvalidUrlError>(result.Error);
        
        linksRepositoryMock.Verify(
            x => x.InsertAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("not-a-url")]
    [InlineData("://example.com")]
    public async Task ProcessLinkAsync_WithInvalidUrl_ReturnsInvalidUrlError(string invalidUrl)
    {
        var userId = ObjectId.GenerateNewId();
        var request = new CreateLinkRequest(invalidUrl);
        var user = CreateClaimsPrincipal(userId.ToString());
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        
        var sut = new LinksProcessServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.ProcessLinkAsync(request, user);
        
        Assert.False(result.IsSuccess);
        Assert.IsType<InvalidUrlError>(result.Error);
        
        linksRepositoryMock.Verify(
            x => x.InsertAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task ProcessLinkAsync_WithMissingNameIdentifierClaim_ReturnsUnauthorizedError()
    {
        const string url = "https://example.com";
        var request = new CreateLinkRequest(url);
        var user = new ClaimsPrincipal();
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        
        var sut = new LinksProcessServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.ProcessLinkAsync(request, user);
        
        Assert.False(result.IsSuccess);
        Assert.IsType<UnauthorizedError>(result.Error);
        
        linksRepositoryMock.Verify(
            x => x.InsertAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-objectid")]
    [InlineData("not-an-objectid")]
    public async Task ProcessLinkAsync_WithInvalidUserIdClaim_ReturnsUnauthorizedError(string? invalidUserId)
    {
        const string url = "https://example.com";
        var request = new CreateLinkRequest(url);
        var user = CreateClaimsPrincipal(invalidUserId);
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        
        var sut = new LinksProcessServiceBuilder()
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        var result = await sut.ProcessLinkAsync(request, user);
        
        Assert.False(result.IsSuccess);
        Assert.IsType<UnauthorizedError>(result.Error);
        
        linksRepositoryMock.Verify(
            x => x.InsertAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task ProcessLinkAsync_CallsHashingServiceWithGuid()
    {
        const string url = "https://example.com";
        const string hashedCode = "hashed-guid";
        var userId = ObjectId.GenerateNewId();
        
        var request = new CreateLinkRequest(url);
        var user = CreateClaimsPrincipal(userId.ToString());
        
        var hashingServiceMock = new Mock<IHashingService>();
        hashingServiceMock
            .Setup(x => x.ComputeHashAsHexString(It.IsAny<string>()))
            .Returns(hashedCode);
        
        var sut = new LinksProcessServiceBuilder()
            .WithHashingService(hashingServiceMock.Object)
            .Build();

        await sut.ProcessLinkAsync(request, user);
        
        hashingServiceMock.Verify(
            x => x.ComputeHashAsHexString(It.IsAny<string>()), 
            Times.Once);
    }

    [Fact]
    public async Task ProcessLinkAsync_CreatesLinkWithCorrectProperties()
    {
        const string url = "https://test-url.com/path?query=value";
        const string hashedCode = "test-hash-code";
        var userId = ObjectId.GenerateNewId();
        
        var request = new CreateLinkRequest(url);
        var user = CreateClaimsPrincipal(userId.ToString());
        
        var hashingServiceMock = new Mock<IHashingService>();
        hashingServiceMock
            .Setup(x => x.ComputeHashAsHexString(It.IsAny<string>()))
            .Returns(hashedCode);
        
        var linksRepositoryMock = new Mock<ILinksRepository>();
        Link? capturedLink = null;
        linksRepositoryMock
            .Setup(x => x.InsertAsync(It.IsAny<Link>(), It.IsAny<CancellationToken>()))
            .Callback<Link, CancellationToken>((link, _) => capturedLink = link);
        
        var sut = new LinksProcessServiceBuilder()
            .WithHashingService(hashingServiceMock.Object)
            .WithLinksRepository(linksRepositoryMock.Object)
            .Build();

        await sut.ProcessLinkAsync(request, user);
        
        Assert.NotNull(capturedLink);
        Assert.Equal(userId, capturedLink.OwnerId);
        Assert.Equal(hashedCode, capturedLink.Code);
        Assert.Equal(url, capturedLink.Url);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(string? userId)
    {
        var claims = new List<Claim>();
        
        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }
        
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
}