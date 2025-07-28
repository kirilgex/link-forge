namespace LinkForge.Application.Auth.Settings;

public class AuthSettings
{
    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required AuthTokenSettings AccessToken { get; init; }

    public required AuthTokenSettings RefreshToken { get; init; }
    
    public required PasswordRestrictions PasswordRestrictions { get; init; }
};

public class AuthTokenSettings
{
    public required string SecretKey { get; init; }
    
    public required int ExpirationTimeInMinutes { get; init; }
}

public record PasswordRestrictions
{
    public required uint MinimalLength { get; init; }
    
    public required bool UppercaseLetters { get; init; }
    
    public required bool LowercaseLetters { get; init; }
    
    public required bool Digits { get; init; }
}
