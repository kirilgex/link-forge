namespace LinkForge.Domain.Links.ValueObjects;

public readonly struct LinkUrl
{
    private string Value { get; }

    private LinkUrl(string value)
    {
        Value = value;
    }
    
    public static bool TryParseFromUserInput(string input, out LinkUrl result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        input = input.ToLowerInvariant().Trim();

        if (!Uri.TryCreate(input, UriKind.Absolute, out _))
        {
            return false;
        }

        result = new LinkUrl(input);

        return true;
    }
    
    public static LinkUrl FromTrusted(string value)
    {
        return new LinkUrl(value);
    }

    public override string ToString() => Value;
    
    public static implicit operator string(LinkUrl url) => url.Value;
}