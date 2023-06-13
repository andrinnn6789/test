using System.Linq;

using IAG.IdentityServer.Authentication;

using Microsoft.AspNetCore.Identity;

using Xunit;

namespace IAG.IdentityServer.Test.Authentication;

public class PasswordGeneratorTest
{
    [Fact]
    public void GeneratePasswordTest()
    {
        PasswordGenerator generator = new PasswordGenerator();
        string password = generator.GenerateRandomPassword();

        Assert.NotNull(password);
        Assert.NotEmpty(password);
    }

    [Fact]
    public void GeneratePasswordWithOptionsTest()
    {
        var options = new PasswordOptions
        {
            RequiredLength = 13,
            RequiredUniqueChars = 4,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        PasswordGenerator generator = new PasswordGenerator();
        string password = generator.GenerateRandomPassword(options);

        Assert.NotNull(password);
        Assert.True(password.Length >= options.RequiredLength);
        Assert.True(password.Distinct().ToArray().Length >= options.RequiredUniqueChars);
        Assert.Contains(password, char.IsDigit);
        Assert.Contains(password, char.IsLower);
        Assert.Contains(password, char.IsUpper);
        Assert.Contains(password, c => !char.IsLetterOrDigit(c));
    }

    [Fact]
    public void GenerateSpecialPasswordTest()
    {
        var options = new PasswordOptions
        {
            RequiredLength = 100,
            RequiredUniqueChars = 1,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        PasswordGenerator generator = new PasswordGenerator();
        var unusedDigits = generator.Digits.Except("1");
        var unusedUppercases = generator.UppercaseCharacters.Except("I");
        var unusedLowercases = generator.LowercaseCharacters.Except("l");
        var unusedSpecialChars = generator.SpecialCharacters.Except("!");

        generator.Digits = "1";
        generator.UppercaseCharacters = "I";
        generator.LowercaseCharacters = "l";
        generator.SpecialCharacters = "!";
            
        string password = generator.GenerateRandomPassword(options);

        Assert.NotNull(password);
        Assert.DoesNotContain(password, unusedDigits.Contains);
        Assert.DoesNotContain(password, unusedUppercases.Contains);
        Assert.DoesNotContain(password, unusedLowercases.Contains);
        Assert.DoesNotContain(password, unusedSpecialChars.Contains);
    }
}