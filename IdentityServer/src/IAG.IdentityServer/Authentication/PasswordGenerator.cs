using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.IdentityServer;

using Microsoft.AspNetCore.Identity;

namespace IAG.IdentityServer.Authentication;

/// <summary>
/// Generates a Random Password
/// respecting the given strength requirements.
/// (see https://www.ryadel.com/en/c-sharp-random-password-generator-asp-net-core-mvc/)
/// </summary>
public class PasswordGenerator : IPasswordGenerator
{
    private static string _defaultUppercaseCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
    private static string _defaultLowercaseCharacters = "abcdefghijkmnopqrstuvwxyz";
    private static string _defaultDigits = "0123456789";
    private static string _defaultSpecialCharacters = "!@#$%&*?_-()";

    public PasswordGenerator()
    {
        UppercaseCharacters = _defaultUppercaseCharacters;
        LowercaseCharacters = _defaultLowercaseCharacters;
        Digits = _defaultDigits;
        SpecialCharacters = _defaultSpecialCharacters;
    }

    public string UppercaseCharacters { get; set; }

    public string LowercaseCharacters { get; set; }

    public string Digits { get; set; }

    public string SpecialCharacters { get; set; }
        
    public string GenerateRandomPassword(PasswordOptions opts = null)
    {
        if (opts == null)
            opts = new PasswordOptions
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

        string[] randomChars =
        {
            UppercaseCharacters,
            LowercaseCharacters,
            Digits,
            SpecialCharacters
        };

        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        if (opts.RequireUppercase)
            chars.Insert(rand.Next(0, chars.Count), UppercaseCharacters[rand.Next(0, UppercaseCharacters.Length)]);

        if (opts.RequireLowercase)
            chars.Insert(rand.Next(0, chars.Count), LowercaseCharacters[rand.Next(0, LowercaseCharacters.Length)]);

        if (opts.RequireDigit)
            chars.Insert(rand.Next(0, chars.Count), Digits[rand.Next(0, Digits.Length)]);

        if (opts.RequireNonAlphanumeric)
            chars.Insert(rand.Next(0, chars.Count), SpecialCharacters[rand.Next(0, SpecialCharacters.Length)]);

        for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        {
            string rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
}