using System;

using IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Mobile.DataLayer.Model;

using Xunit;

namespace IAG.ControlCenter.Test.Mobile.BusinessLayer.ObjectMapper;

public class ModuleMapperTest
{
    private readonly MobileModuleMapper _mapper;
    private readonly MobileModule _sourceModule;
    private readonly MobileModule _destinationModule;

    public ModuleMapperTest()
    {
        _mapper = new MobileModuleMapper();

        _sourceModule = new MobileModule
        {
            Id = Guid.Empty,
            TenantId = Guid.NewGuid(),
            Licence = Guid.NewGuid().ToString(),
            ModuleName = "Inventory",
            LicencedUntil = DateTime.Now
        };

        _destinationModule = new MobileModule();
    }

    [Fact]
    public void UpdateDestination_WithMobileModule_ReturnsModule()
    {
        _mapper.UpdateDestination(_destinationModule, _sourceModule);

        Assert.Equal(_sourceModule.Id, _destinationModule.Id);
        Assert.Equal(_sourceModule.TenantId, _destinationModule.TenantId);
        Assert.Equal(_sourceModule.Licence, _destinationModule.Licence);
        Assert.Equal(_sourceModule.ModuleName, _destinationModule.ModuleName);
        Assert.Equal(_sourceModule.LicencedUntil, _destinationModule.LicencedUntil);
    }
}