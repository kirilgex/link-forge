using LinkForge.Application.Auth.Services.Interfaces;
using LinkForge.Application.Auth.Settings;

using Microsoft.Extensions.Options;

namespace LinkForge.Application.Auth.Services.Implementations;

public class PasswordValidationService(
    IOptions<AuthSettings> authSettings)
    : IPasswordValidationService
{
    public string GetReadablePasswordRestrictions()
    {
        var passwordRestrictions = authSettings.Value.PasswordRestrictions;

        List<string> restrictions = [];
        
        if (passwordRestrictions.MinimalLength > 0)
            restrictions.Add($"be at least {passwordRestrictions.MinimalLength} characters long");

        if (passwordRestrictions.UppercaseLetters)
            restrictions.Add("contain at least one uppercase character");

        if (passwordRestrictions.LowercaseLetters)
            restrictions.Add("contain at least one lowercase character");

        if (passwordRestrictions.Digits)
            restrictions.Add("contain at least one digit");

        return restrictions.Count > 0
            ? $"Password is required and must: {string.Join("; ", restrictions)}."
            : "Password is required.";
    }

    public bool ValidatePassword(string input, out string password)
    {
        var passwordRestrictions = authSettings.Value.PasswordRestrictions;
        
        password = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }
        
        input = input.Trim();

        if (passwordRestrictions.MinimalLength > 0
            && input.Length < passwordRestrictions.MinimalLength)
        {
            return false;
        }

        if (passwordRestrictions.UppercaseLetters
            && !input.Any(char.IsUpper))
        {
            return false;
        }

        if (passwordRestrictions.LowercaseLetters
            && !input.Any(char.IsLower))
        {
            return false;
        }

        if (passwordRestrictions.Digits
            && !input.Any(char.IsDigit))
        {
            return false;
        }
        
        password = input;

        return true;
    }
}