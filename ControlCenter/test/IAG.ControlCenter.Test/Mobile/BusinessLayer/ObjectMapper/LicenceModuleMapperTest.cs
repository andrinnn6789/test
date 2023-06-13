using System;

using IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Mobile.DataLayer.Model;

using Xunit;

namespace IAG.ControlCenter.Test.Mobile.BusinessLayer.ObjectMapper;

public class LicenceModuleMapperTest
{
    private readonly MobileModule _mobileModule;

    public LicenceModuleMapperTest()
    {
        _mobileModule = new MobileModule
        {
            ModuleName = "ArticleInformation",
            LicencedUntil = DateTime.Now
        };
    }

    [Fact]
    public void MapToModule_WithMobileModule_ReturnsModule()
    {
        var module  = LicenceModuleMapper.MapToModule(_mobileModule);

        Assert.Equal(_mobileModule.ModuleName, module.Name);
        Assert.Equal(_mobileModule.LicencedUntil, module.ValidUntil);
    }
}