namespace LinkForge.Domain.Links.ValueTypes;

public record struct LinkUrl(string Value)
{
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

        result = new LinkUrl(builder.ToString().TrimEnd('/'));

        return true;
    }

    public override string ToString() => Value;
}
