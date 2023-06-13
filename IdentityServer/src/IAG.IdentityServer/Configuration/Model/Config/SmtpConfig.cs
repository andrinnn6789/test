
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Configuration.Model.Config;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class SmtpConfig : ISmtpConfig
{
    public static readonly string ConfigName = "IdentityServer.Smtp";

    public string Server { get; set; }
    public int? Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool UseSsl { get; set; }
}