using System.Security.Cryptography.X509Certificates;

namespace IAG.IdentityServer.Configuration.Model.Config;

public interface ICertificateManagerConfig
{
    string CertificateName { get; set; }

    string CertificatePassword { get; set; }

    string CertificateFileName { get; set; }

    StoreName StoreName { get; set; }

    StoreLocation StoreLocation { get; set; }
}