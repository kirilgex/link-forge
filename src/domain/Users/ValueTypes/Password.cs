using System.Text;

namespace LinkForge.Domain.Users.ValueTypes;

public record struct Password(string Value)
{
    private const int MinimalLength = 8;
    private const bool UppercaseLetters = true;
    private const bool LowercaseLetters = true;
    private const bool Digits = true;

    public static string GetPasswordRestrictions()
    {
        var sb = new StringBuilder();

        sb.Append("Password is required and must: ");

        if (MinimalLength > 0)
            sb.Append($"be at least {MinimalLength} characters long; ");

        if (UppercaseLetters)
            sb.Append("contain at least one uppercase character; ");

        if (LowercaseLetters)
            sb.Append("contain at least one lowercase character; ");

        if (Digits)
            sb.Append("contain at least one digit; ");

        return sb.ToString().Trim();
    }

    public static bool TryParseFromUserInput(string input, out Password result)
    {
        result = default;

        input = input.Trim();

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (MinimalLength > 0 && input.Length < MinimalLength)
            return false;

        if (UppercaseLetters && !input.Any(char.IsUpper))
            return false;

        if (LowercaseLetters && !input.Any(char.IsLower))
            return false;

        if (Digits && !input.Any(char.IsDigit))
            return false;

        return true;
    }

    public static implicit operator string(Password source) => source.Value;
}
