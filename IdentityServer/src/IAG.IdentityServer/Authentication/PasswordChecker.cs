using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Resource;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer;

using Microsoft.AspNetCore.Identity;

namespace IAG.IdentityServer.Authentication;

/// <summary>
/// Checks the password against a policy (PasswortOptions)
/// inspired by: https://www.ryadel.com/en/passwordcheck-c-sharp-password-class-calculate-password-strength-policy-aspnet/
/// </summary>
public class PasswordChecker : IPasswordChecker
{
    private static string _defaultSpecialCharacters = "!@#$%^&*?_~-£().,";

    public PasswordChecker()
    {
        SpecialCharacters = _defaultSpecialCharacters;
    }

    public string SpecialCharacters { get; set; }
    public List<MessageStructure> PasswordPolicyMessages { get; } = new();


    public bool IsValidPassword(string password, PasswordOptions options)
    {
        var valid = true;
        password = password ?? string.Empty;
        if (!HasMinimumLength(password, options.RequiredLength))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequiredLength, options.RequiredLength));
            valid = false;
        }
        if (!HasMinimumUniqueChars(password, options.RequiredUniqueChars))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequiredUniqueChars, options.RequiredUniqueChars));
            valid = false;
        }
        if (options.RequireNonAlphanumeric && !HasSpecialChar(password))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequireNonAlphanumeric));
            valid = false;
        }
        if (options.RequireLowercase && !HasLowerCaseLetter(password))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequireLowercase));
            valid = false;
        }
        if (options.RequireUppercase && !HasUpperCaseLetter(password))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequireUppercase));
            valid = false;
        }
        if (options.RequireDigit && !HasDigit(password))
        {
            PasswordPolicyMessages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.PasswordPolicyErrorRequireDigit));
            valid = false;
        }
        return valid;
    }

    private bool HasMinimumLength(string password, int minLength)
    {
        return password.Length >= minLength;
    }

    private bool HasMinimumUniqueChars(string password, int minUniqueChars)
    {
        return password.Distinct().Count() >= minUniqueChars;
    }

    private bool HasDigit(string password)
    {
        return password.Any(char.IsDigit);
    }

    private bool HasSpecialChar(string password)
    {
        return password.IndexOfAny(SpecialCharacters.ToArray()) != -1;
    }

    private bool HasUpperCaseLetter(string password)
    {
        return password.Any(char.IsUpper);
    }

    private bool HasLowerCaseLetter(string password)
    {
        return password.Any(char.IsLower);
    }
}