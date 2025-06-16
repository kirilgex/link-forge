namespace LinkForge.Domain.ValueTypes;

public record struct EntityId(string Value)
{
    public override string ToString() => Value;
}
