namespace LinkForge.Domain.ValueTypes;

public record EntityId(string Value)
{
    public override string ToString() => Value;
}
