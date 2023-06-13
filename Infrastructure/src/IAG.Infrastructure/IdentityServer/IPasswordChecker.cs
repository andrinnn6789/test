using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Model;

using Microsoft.AspNetCore.Identity;

namespace IAG.Infrastructure.IdentityServer;

public interface IPasswordChecker
{
    string SpecialCharacters { get; set; }
    List<MessageStructure> PasswordPolicyMessages { get; }

    bool IsValidPassword(string password, PasswordOptions options);
}