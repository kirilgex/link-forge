namespace LinkForge.Domain.Links.ValueTypes;

public record struct LinkOriginalUrl(string Value)
{
    public static bool TryParseFromUserInput(string input, out LinkOriginalUrl result)
    {
        result = default;

        input = input.Trim();

        if (!input.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
            && !input.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
        {
            input = "https://" + input;
        }

        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri)) return false;

        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme.ToLowerInvariant(),
            Host = uri.Host.ToLowerInvariant(),
            Port = uri is { Scheme: "http", Port: 80 } or { Scheme: "https", Port: 443 }
                ? -1
                : uri.Port,
        };

        result = new LinkOriginalUrl(builder.ToString().TrimEnd('/'));

        return true;
    }

    public static explicit operator LinkOriginalUrl(string source) => new(source);

    public static implicit operator string(LinkOriginalUrl source) => source.Value;
}
