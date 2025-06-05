namespace LinkForge.Infrastructure.PersistentStorage;

public record DatabaseSettings
{
    public required string ConnectionString { get; init; }
    public required string DatabaseName { get; init; }
}
