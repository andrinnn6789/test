using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class LoginViewModel
{
    public string Username { get; set; }

    public string Password { get; set; }

    public string ErrorMessage { get; set; }
}