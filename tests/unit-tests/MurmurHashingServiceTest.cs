using LinkForge.UnitTests.Builders;

namespace LinkForge.UnitTests;

using Application.Links.Services.Implementations;
using Microsoft.Extensions.Options;
using Xunit;

public class MurmurHashingServiceTest
{
    [Fact]
    public void ComputeHashAsHexString_ShouldReturnConsistentHash()
    {
        var hashingSettings = new HashingSettingsBuilder().Build();
        var sut = new MurmurHashingService(Options.Create(hashingSettings));
        
        const string input = "test-value";
        
        var result = sut.ComputeHashAsHexString(input);
        var secondResult = sut.ComputeHashAsHexString(input);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(result, secondResult);
    }
}