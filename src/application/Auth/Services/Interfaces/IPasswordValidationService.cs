namespace LinkForge.Application.Auth.Services.Interfaces;

public interface IPasswordValidationService
{
    string GetReadablePasswordRestrictions();
    
    bool ValidatePassword(string input, out string password);
}