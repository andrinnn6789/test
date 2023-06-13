using Microsoft.AspNetCore.Identity;

namespace IAG.Infrastructure.IdentityServer;

public interface IPasswordGenerator
{
    string GenerateRandomPassword(PasswordOptions opts = null);
}