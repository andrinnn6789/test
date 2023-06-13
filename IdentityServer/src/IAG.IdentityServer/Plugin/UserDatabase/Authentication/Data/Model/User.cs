
using System.Globalization;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;

public class User
{
    private string _culture;

    public string Name { get; set; }
    public string Password { get; set; }
    public string InitPassword { get; set; }
    public bool ChangePasswordAfterLogin { get; set; }
    public string EMail { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Culture
    {
        get => _culture;
        set
        {
            if (value != null)
            {
                var unused = new CultureInfo(value);  // check valid culture
            }

            _culture = value;
        }
    }
}