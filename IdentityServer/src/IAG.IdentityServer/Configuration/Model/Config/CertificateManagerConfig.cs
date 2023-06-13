using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Configuration.Model.Config;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class CertificateManagerConfig : ICertificateManagerConfig
{
    public static readonly string ConfigName = "IdentityServer.CertificateManager";

    public string CertificateName { get; set; }

    public string CertificatePassword { get; set; }

    public string CertificateFileName { get; set; }

    public StoreName StoreName { get; set; }

    public StoreLocation StoreLocation { get; set; }
}