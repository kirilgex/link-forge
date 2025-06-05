namespace LinkForge.Domain.ValueTypes;

public record struct LinkCode(string Value)
{
    public static LinkCode FromUserInput(string input)
        => new(input.ToLowerInvariant().Trim());

    public static explicit operator LinkCode(string source) => new(source);

    public static implicit operator string(LinkCode source) => source.Value;
}
