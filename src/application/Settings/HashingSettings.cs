namespace LinkForge.Application.Settings;

public record HashingSettings
{
    public required int HashSizeInBits { get; init; }
    public required uint Seed { get; init; }
}
