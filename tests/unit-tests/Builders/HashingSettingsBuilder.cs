using LinkForge.Application.Links.Settings;

namespace LinkForge.UnitTests.Builders;

public class HashingSettingsBuilder
{
    private int _hashSizeInBits = 32;
    private uint _seed = 1;
    
    public HashingSettings Build()
    {
        return new HashingSettings
        {
            HashSizeInBits = _hashSizeInBits,
            Seed = _seed,
        };
    }

    public HashingSettingsBuilder WithHashSizeInBits(int hashSizeInBits)
    {
        _hashSizeInBits = hashSizeInBits;
        return this;
    }
    
    public HashingSettingsBuilder WithSeed(uint seed)
    {
        _seed = seed;
        return this;
    }
}