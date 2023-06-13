using IAG.IdentityServer.Authentication;

using Microsoft.AspNetCore.Identity;

using Xunit;

namespace IAG.IdentityServer.Test.Authentication;

public class PasswordCheckerTest
{
    [Fact]
    public void CheckPasswordTest()
    {
        PasswordChecker checker = new PasswordChecker();
        var options = new PasswordOptions
        {
            RequiredLength = 8,
            RequiredUniqueChars = 5,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        Assert.False(checker.IsValidPassword(null, options));   // empty password
        Assert.False(checker.IsValidPassword(string.Empty, options));   // empty password
        Assert.False(checker.IsValidPassword("2short", options));   // too short
        Assert.False(checker.IsValidPassword("no_Digits", options));   // No digits
        Assert.False(checker.IsValidPassword("NO_LOWER_CA5E5", options));   // No lower cases
        Assert.False(checker.IsValidPassword("no_upper_ca5e5", options));   // No upper cases
        Assert.False(checker.IsValidPassword("NoNonAlphaNumer1c", options));   // No non alphanumeric
        Assert.False(checker.IsValidPassword("1!iI1!iI", options));   // too less unique characters
        Assert.True(checker.IsValidPassword("G00d-Pwd", options));
    }

    [Fact]
    public void CheckSimplePasswordTest()
    {
        PasswordChecker checker = new PasswordChecker();
        var options = new PasswordOptions
        {
            RequiredLength = 2,
            RequiredUniqueChars = 1,
            RequireDigit = false,
            RequireLowercase = false,
            RequireNonAlphanumeric = false,
            RequireUppercase = false
        };

        Assert.False(checker.IsValidPassword(null, options));   // empty password
        Assert.False(checker.IsValidPassword(string.Empty, options));   // empty password
        Assert.False(checker.IsValidPassword("a", options));
        Assert.True(checker.IsValidPassword("ab", options));
        Assert.True(checker.IsValidPassword("aa", options));
        Assert.True(checker.IsValidPassword("a1", options));
        Assert.True(checker.IsValidPassword("a!", options));
        Assert.True(checker.IsValidPassword("1!", options));
        Assert.True(checker.IsValidPassword("*!", options));
        Assert.True(checker.IsValidPassword("ABC1!", options));
    }
}