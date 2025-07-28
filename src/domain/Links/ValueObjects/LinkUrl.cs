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

        input = input.ToLowerInvariant().Trim();

        if (!input.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
            && !input.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
        {
            input = "https://" + input;
        }

        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri)) return false;

        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme,
            Host = uri.Host,
            Port = uri is { Scheme: "http", Port: 80 } or { Scheme: "https", Port: 443 }
                ? -1
                : uri.Port,
        };

        result = new LinkUrl(builder.ToString());

        return true;
    }
    
    public static LinkUrl FromTrusted(string value)
    {
        return new LinkUrl(value);
    }

    public override string ToString() => Value;
    
    public static implicit operator string(LinkUrl url) => url.Value;
}
