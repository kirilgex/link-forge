using LinkForge.UnitTests.Builders;

namespace LinkForge.UnitTests;

public class PasswordValidationServiceTest
{
    [Fact]
    public void GetReadablePasswordRestrictions_NoRestrictions_ReturnsBasicMessage()
    {
        var sut = new PasswordValidationServiceBuilder().Build();

        var result = sut.GetReadablePasswordRestrictions();

        Assert.Equal("Password is required.", result);
    }

    [Fact]
    public void GetReadablePasswordRestrictions_WithAllRestrictions_ReturnsFullMessage()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithMinimalLength(8).WithUppercaseLetters().WithLowercaseLetters().WithDigits()))
            .Build();

        var result = sut.GetReadablePasswordRestrictions();

        Assert.Equal(
            "Password is required and must: " +
            "be at least 8 characters long; " +
            "contain at least one uppercase character; " +
            "contain at least one lowercase character; " +
            "contain at least one digit.",
            result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ValidatePassword_EmptyOrWhitespace_ReturnsFalse(string? input)
    {
        var sut = new PasswordValidationServiceBuilder().Build();

        var result = sut.ValidatePassword(input!, out var password);

        Assert.False(result);
        Assert.Empty(password);
    }

    [Fact]
    public void ValidatePassword_TooShort_ReturnsFalse()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithMinimalLength(12)))
            .Build();
        
        const string input = "TooShort123";

        var result = sut.ValidatePassword(input, out var password);

        Assert.False(result);
        Assert.Empty(password);
    }

    [Fact]
    public void ValidatePassword_NoUppercase_ReturnsFalse()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithUppercaseLetters()))
            .Build();

        const string input = "lowercase123";

        var result = sut.ValidatePassword(input, out var password);

        Assert.False(result);
        Assert.Empty(password);
    }

    [Fact]
    public void ValidatePassword_NoLowercase_ReturnsFalse()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithLowercaseLetters()))
            .Build();

        const string input = "UPPERCASE123";

        var result = sut.ValidatePassword(input, out var password);

        Assert.False(result);
        Assert.Empty(password);
    }

    [Fact]
    public void ValidatePassword_NoDigits_ReturnsFalse()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithDigits()))
            .Build();
        
        const string input = "NoDigitsHere";

        var result = sut.ValidatePassword(input, out var password);

        Assert.False(result);
        Assert.Empty(password);
    }

    [Fact]
    public void ValidatePassword_ValidPassword_ReturnsTrue()
    {
        var sut = new PasswordValidationServiceBuilder()
            .WithAuthSettings(
                authSettingsBuilder => authSettingsBuilder.WithPasswordRestrictions(
                    passwordRestrictionsBuilder => passwordRestrictionsBuilder
                        .WithMinimalLength(8).WithUppercaseLetters().WithUppercaseLetters().WithDigits()))
            .Build();
        
        const string input = "ValidPass123";

        var result = sut.ValidatePassword(input, out var password);

        Assert.True(result);
        Assert.Equal(input, password);
    }
}