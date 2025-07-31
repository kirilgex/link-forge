using LinkForge.Application.Auth.Settings;

namespace LinkForge.UnitTests.Builders;

public class PasswordRestrictionsBuilder
{
    private uint _minimalLength;
    private bool _uppercaseLetters;
    private bool _lowercaseLetters;
    private bool _digits;

    public PasswordRestrictions Build()
    {
        return new PasswordRestrictions
        {
            MinimalLength = _minimalLength,
            UppercaseLetters = _uppercaseLetters,
            LowercaseLetters = _lowercaseLetters,
            Digits = _digits
        };
    }

    public PasswordRestrictionsBuilder WithMinimalLength(uint minimalLength)
    {
        _minimalLength = minimalLength;
        return this;
    }

    public PasswordRestrictionsBuilder WithUppercaseLetters()
    {
        _uppercaseLetters = true;
        return this;
    }

    public PasswordRestrictionsBuilder WithLowercaseLetters()
    {
        _lowercaseLetters = true;
        return this;
    }

    public PasswordRestrictionsBuilder WithDigits()
    {
        _digits = true;
        return this;
    }
}