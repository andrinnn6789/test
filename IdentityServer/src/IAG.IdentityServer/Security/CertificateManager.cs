using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using IAG.IdentityServer.Configuration.Model.Config;

namespace IAG.IdentityServer.Security;

public class CertificateManager : ICertificateManager
{
    private readonly ICertificateManagerConfig _config;

    public CertificateManager(ICertificateManagerConfig config)
    {
        _config = config;
        var store = new X509Store(_config.StoreName, _config.StoreLocation);
        store.Open(OpenFlags.ReadOnly);
        foreach (var certificate in store.Certificates)
        {
            if (certificate.Issuer.IndexOf(config.CertificateName, StringComparison.Ordinal) <= 0) 
                continue;
            SigningCertificate = certificate;
            return;
        }
        SigningCertificate = GetSigningCertificate();
    }


    public X509Certificate2 SigningCertificate { get; }


    private X509Certificate2 GetSigningCertificate()
    {
        if (!File.Exists(_config.CertificateFileName))
        {
            return CreateNewCertificate();
        }

        var certificateData = File.ReadAllBytes(_config.CertificateFileName);
        var certificate = new X509Certificate2(certificateData, _config.CertificatePassword);
        return certificate.NotAfter.AddYears(-1) >= DateTime.UtcNow ? certificate : CreateNewCertificate();
    }


    private X509Certificate2 CreateNewCertificate()
    {
        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
        sanBuilder.AddDnsName(Dns.GetHostName());
        sanBuilder.AddDnsName(Environment.MachineName);

        var distinguishedName = new X500DistinguishedName($"CN={_config.CertificateName}");
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));

        request.CertificateExtensions.Add(sanBuilder.Build());

        var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddMinutes(-1)), new DateTimeOffset(DateTime.UtcNow.AddYears(10)));

        var path = Path.GetDirectoryName(_config.CertificateFileName);
        if (path != null && !Directory.Exists(path))
            Directory.CreateDirectory(path);

        using var fileStream = File.Create(_config.CertificateFileName);
        fileStream.Write(certificate.Export(X509ContentType.Pfx, _config.CertificatePassword));
        fileStream.Close();

        return certificate;
    }
}