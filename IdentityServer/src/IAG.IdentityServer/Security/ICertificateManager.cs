using System.Security.Cryptography.X509Certificates;

namespace IAG.IdentityServer.Security;

public interface ICertificateManager
{
    X509Certificate2 SigningCertificate { get; }
}