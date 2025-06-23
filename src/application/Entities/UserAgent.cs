namespace LinkForge.Application.Entities;

public class UserAgent(string? value)
{
    public string Value { get; init; } = string.IsNullOrWhiteSpace(value) ? "unspecified" : value;

    public override string ToString() => Value;
}
