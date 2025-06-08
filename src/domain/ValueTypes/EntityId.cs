namespace LinkForge.Domain.ValueTypes;

public record struct EntityId(string Value)
{
    public static explicit operator EntityId(string source) => new(source);
}
