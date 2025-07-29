namespace LinkForge.Domain.Links.ValueObjects;

public readonly struct LinkCode
{
    private string Value { get; }

    private LinkCode(string value)
    {
        Value = value;
    }

    public static LinkCode FromUserInput(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new LinkCode(string.Empty);
        }

        return new LinkCode(raw.ToLowerInvariant().Trim());
    }

    public static LinkCode FromTrusted(string value)
    {
        return new LinkCode(value);
    }

    public override string ToString() => Value;
    
    public static implicit operator string(LinkCode code) => code.Value;
}
