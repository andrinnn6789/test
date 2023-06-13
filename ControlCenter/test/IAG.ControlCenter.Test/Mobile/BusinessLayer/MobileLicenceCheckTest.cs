using System;
using System.Linq;

using IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;

using Xunit;

namespace IAG.ControlCenter.Test.Mobile.BusinessLayer;

public class MobileLicenceCheckTest : IDisposable
{
    private readonly TestContextBuilder _builder;

    public MobileLicenceCheckTest()
    {
        _builder = new TestContextBuilder();
        Checker = new LicenceCheck(_builder.Context);
    }

    public void Dispose()
    {
        _builder.Dispose();
    }

    private LicenceCheck Checker { get; }

    [Fact]
    public void CheckInvalidTest()
    {
        var response = Checker.Check(new LicenceRequest());
        Assert.Equal(LicenceStatusAppEnum.Invalid, response.LicenceStatus);
    }

    [Fact]
    public void CheckNewTest()
    {
        var request = new LicenceRequest { DeviceInfo = "info", DeviceId = "1", Licence = "new" };
        var response = Checker.Check(request);

        Assert.Equal(LicenceStatusAppEnum.Inuse, response.LicenceStatus);
        Assert.Equal(2, response.Backends.Count);
        foreach (var installation in response.Backends)
        {
            Assert.True(_builder.InstallationGuid1 == installation.Id ||
                        _builder.InstallationGuid2 == installation.Id);
            Assert.StartsWith("Tenant1", installation.Url);
            Assert.StartsWith("inst", installation.Name);
            Assert.Equal(10, installation.SyncInterval);
            Assert.Equal("#0000FF", installation.Color);
        }
        Assert.Single(response.Modules);
        foreach (var module in response.Modules)
        {
            Assert.True(module.Name == "ArticleInformation" || module.Name == "Inventory");
            Assert.True(module.ValidUntil is null ||
                        (Convert.ToDateTime(module.ValidUntil) > DateTime.Now.AddDays(360) && module.ValidUntil < DateTime.Now.AddDays(370)));
        }
        var lic = _builder.Context.MobileLicences.First(l => l.Licence == request.Licence);
        Assert.Equal(lic.Licence, request.Licence);
        Assert.Equal("info", lic.DeviceInfo);
        Assert.Equal("1", lic.DeviceId);
    }

    [Fact]
    public void CheckInUseOkTest()
    {
        var request = new LicenceRequest { DeviceInfo = "info2", DeviceId = "2", Licence = "inUse2" };
        var response = Checker.Check(request);
        Assert.Equal(LicenceStatusAppEnum.Inuse, response.LicenceStatus);
        Assert.Empty(response.Backends);
        var lic = _builder.Context.MobileLicences.First(l => l.Licence == request.Licence);
        Assert.Equal("info2", lic.DeviceInfo);
        Assert.Equal("2", lic.DeviceId);
        Assert.Equal(2, response.Modules.Count);
    }

    [Fact]
    public void CheckInUseNOkTest()
    {
        var request = new LicenceRequest { DeviceInfo = "info2", DeviceId = "2", Licence = "inUse1" };
        var response = Checker.Check(request);
        Assert.Equal(LicenceStatusAppEnum.Invalid, response.LicenceStatus);
        Assert.Null(response.Backends);
        var lic = _builder.Context.MobileLicences.First(l => l.Licence == request.Licence);
        Assert.Equal("info1", lic.DeviceInfo);
        Assert.Equal("1", lic.DeviceId);
        Assert.Null(response.Modules);
    }

    [Fact]
    public void CheckRevokedTest()
    {
        var request = new LicenceRequest { DeviceInfo = "info", DeviceId = "1", Licence = "revoked" };
        var response = Checker.Check(request);
        Assert.Equal(LicenceStatusAppEnum.Revoked, response.LicenceStatus);
        Assert.Null(response.Backends);
        var lic = _builder.Context.MobileLicences.First(l => l.Licence == request.Licence);
        Assert.True(string.IsNullOrWhiteSpace(lic.DeviceInfo));
        Assert.True(string.IsNullOrWhiteSpace(lic.DeviceId));
        Assert.Null(response.Modules);
    }

    [Fact]
    public void FreeInvalidTest()
    {
        var response = Checker.Free(new LicenceRequest());
        Assert.Equal(LicenceStatusAppEnum.Invalid, response.LicenceStatus);
    }

    [Fact]
    public void FreeOkTest()
    {
        var request = new LicenceRequest { DeviceInfo = "info2", DeviceId = "2", Licence = "inUse2" };
        var response = Checker.Free(request);
        Assert.Equal(LicenceStatusAppEnum.New, response.LicenceStatus);
        var lic = _builder.Context.MobileLicences.First(l => l.Licence == request.Licence);
        Assert.Null(lic.DeviceInfo);
        Assert.Null(lic.DeviceId);
    }
}