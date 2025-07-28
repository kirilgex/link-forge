namespace LinkForge.Domain.Users.ValueObjects;

public readonly struct UserAgent
{
    private string Value { get; }

    public UserAgent(string? value)
    {
        Value = value = string.IsNullOrWhiteSpace(value) ? "unspecified" : value;
    }
    
    public override string ToString() => Value;
    
    public static implicit operator string(UserAgent agent) => agent.Value;
}
