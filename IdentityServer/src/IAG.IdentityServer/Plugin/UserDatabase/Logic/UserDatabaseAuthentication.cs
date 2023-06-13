using System;
using System.Security.Cryptography;

using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IAG.IdentityServer.Plugin.UserDatabase.Logic;

public class UserDatabaseAuthentication
{
    public bool CheckPassword(User user, string password)
    {
        var saltBytes = Convert.FromBase64String(user.Salt);
        var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

        return hashedPassword == user.Password;
    }

    public void UpdatePassword(User user, string newPassword)
    {
        byte[] saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(newPassword, saltBytes, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));
        user.Salt = Convert.ToBase64String(saltBytes);
        user.Password = hashedPassword;
    }
}