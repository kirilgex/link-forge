using System.Net.Mail;

namespace LinkForge.Domain.Users.ValueTypes;

public record struct UserEmail(string Value)
{
    public static bool TryParseFromUserInput(string input, out UserEmail result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!MailAddress.TryCreate(input, out var address))
            return false;

        result = new UserEmail(address.Address);

        return true;
    }

    public static explicit operator UserEmail(string source) => new(source);

    public static implicit operator string(UserEmail source) => source.Value;
}
