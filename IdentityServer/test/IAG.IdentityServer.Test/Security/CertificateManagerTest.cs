using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Security;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Security;

public class CertificateManagerTest : IDisposable
{
    private readonly string _certificatePath;
    private readonly ICertificateManagerConfig _config;
    private readonly X509Store _store;

    public CertificateManagerTest()
    {
        _certificatePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        var configMock = new Mock<ICertificateManagerConfig>();
        configMock.Setup(m => m.CertificateFileName).Returns(Path.Combine(_certificatePath, "myCert.pfx"));
        configMock.Setup(m => m.CertificateName).Returns("testName");
        configMock.Setup(m => m.CertificatePassword).Returns("kRY!ZWOxtwIs.AhcAYYLJwS-0gDCwsZ4fS*IJyaELnXc");
        configMock.Setup(m => m.StoreLocation).Returns(StoreLocation.CurrentUser);
        configMock.Setup(m => m.StoreName).Returns(StoreName.My);
        _config = configMock.Object;

        _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        CleanUpCertificates();
    }

    [Fact]
    public void SimpleCertificateTest()
    {
        var certificateManager = new CertificateManager(_config);

        Assert.Single(Directory.EnumerateFiles(_certificatePath));
        Assert.NotNull(certificateManager.SigningCertificate);
        Assert.NotEmpty(certificateManager.SigningCertificate.RawData);
        Assert.NotEmpty(certificateManager.SigningCertificate.GetPublicKey());
        Assert.True(DateTime.TryParse(certificateManager.SigningCertificate.GetEffectiveDateString(), out var effectiveDate));
        Assert.True(DateTime.TryParse(certificateManager.SigningCertificate.GetExpirationDateString(), out var expirationDate));
        Assert.True(effectiveDate <= DateTime.Now);
        Assert.True(expirationDate > DateTime.Today);
    }

    [Fact]
    public void CheckCertificateReUseTest()
    {
        var certificateManager = new CertificateManager(_config);
        var newCreatedCertificate = certificateManager.SigningCertificate;

        var reUsedCertificate = new CertificateManager(_config).SigningCertificate;

        Assert.Single(Directory.EnumerateFiles(_certificatePath));
        Assert.Equal(newCreatedCertificate.RawData, reUsedCertificate.RawData);
    }

    [Fact]
    public void CheckRenewCertificateTest()
    {
        // create old certificate
        var request = new CertificateRequest(
            new X500DistinguishedName("CN=TestCertificate"),
            RSA.Create(2048),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        var oldCertificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddHours(-1));
        Directory.CreateDirectory(_certificatePath);
        using var fileStream = File.Create(_config.CertificateFileName);
        fileStream.Write(oldCertificate.Export(X509ContentType.Pfx, "kRY!ZWOxtwIs.AhcAYYLJwS-0gDCwsZ4fS*IJyaELnXc"));
        fileStream.Close();

        var certificateManager = new CertificateManager(_config);
        var newCreatedCertificate = certificateManager.SigningCertificate;

        Assert.Single(Directory.EnumerateFiles(_certificatePath));
        Assert.NotEqual(newCreatedCertificate.RawData, oldCertificate.RawData);
        Assert.True(DateTime.TryParse(certificateManager.SigningCertificate.GetEffectiveDateString(), out var effectiveDate));
        Assert.True(effectiveDate <= DateTime.Now);
    }

    [Fact]
    public void CheckStore()
    {
        try
        {
            var request = new CertificateRequest(
                new X500DistinguishedName($"CN={_config.CertificateName}"),
                RSA.Create(2048),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            var testCertificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow.AddHours(1));
            _store.Open(OpenFlags.ReadWrite);
            _store.Add(testCertificate);
            _store.Close();
            var certificateManager = new CertificateManager(_config);

            Assert.NotNull(certificateManager.SigningCertificate);
            Assert.Equal($"CN={_config.CertificateName}", certificateManager.SigningCertificate.Issuer);
        }
        finally
        {
            CleanUpCertificates();
        }
    }

    private void CleanUpCertificates()
    {
        if (_store.IsOpen)
            _store.Close();
        _store.Open(OpenFlags.ReadWrite);
        var certificateToRemove = new List<X509Certificate2>();
        foreach (var t in _store.Certificates)
        {
            if (t.Issuer.IndexOf(_config.CertificateName, StringComparison.Ordinal) <= 0) continue;
            certificateToRemove.Add(t);
        }
        foreach (var certificate2 in certificateToRemove)
        {
            _store.Remove(certificate2);
        }
        _store.Close();
    }

    public void Dispose()
    {
        if (Directory.Exists(_certificatePath))
            Directory.Delete(_certificatePath, true);
        _store.Dispose();
    }
}