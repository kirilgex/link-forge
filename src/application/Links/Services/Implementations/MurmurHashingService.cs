using System.Data.HashFunction.MurmurHash;
using System.Text;

using LinkForge.Application.Links.Services.Interfaces;
using LinkForge.Application.Links.Settings;

using Microsoft.Extensions.Options;

namespace LinkForge.Application.Links.Services.Implementations;

public class MurmurHashingService(IOptions<HashingSettings> settings)
    : IHashingService
{
    public string ComputeHashAsHexString(string value)
    {
        var hasher = MurmurHash3Factory.Instance.Create(
            new MurmurHash3Config
            {
                HashSizeInBits = settings.Value.HashSizeInBits,
                Seed = settings.Value.Seed,
            });
        
        var valueBytes = Encoding.UTF8.GetBytes(value);
        
        var hashBytes = hasher.ComputeHash(valueBytes);

        return hashBytes.AsHexString();
    }
}
