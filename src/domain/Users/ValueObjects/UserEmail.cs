using System.Net.Mail;

namespace LinkForge.Domain.Users.ValueObjects;

public readonly struct UserEmail : IEquatable<UserEmail>
{
    private string Value { get; }
    
    private UserEmail(string value)
    {
        Value = value;
    }

    public static UserEmail ParseFromUserInput(string raw)
    {
        return new UserEmail(raw.ToLowerInvariant().Trim());
    }

    public static bool TryParseFromUserInput(string input, out UserEmail result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if (!MailAddress.TryCreate(input, out var address))
        {
            return false;
        }

        result = new UserEmail(address.Address);

        return true;
    }
    
    public static UserEmail FromTrusted(string value)
    {
        return new UserEmail(value);
    }

    public bool Equals(UserEmail other) => Value.Equals(other.Value);

    public override bool Equals(object? obj) => obj is UserEmail other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
    
    public static implicit operator string(UserEmail email) => email.Value;

    public static bool operator ==(UserEmail left, UserEmail right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UserEmail left, UserEmail right)
    {
        return !(left == right);
    }
}
