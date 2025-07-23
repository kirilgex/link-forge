namespace LinkForge.Domain.Links.ValueTypes;

public record struct LinkCode(string Value)
{
    public static LinkCode FromUserInput(string input)
        => new(input.ToLowerInvariant().Trim());

    public override string ToString() => Value;
}
