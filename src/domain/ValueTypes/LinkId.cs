namespace LinkForge.Domain.ValueTypes;

public record struct LinkId(string Value)
{
    public static explicit operator LinkId(string source) => new(source);
}
