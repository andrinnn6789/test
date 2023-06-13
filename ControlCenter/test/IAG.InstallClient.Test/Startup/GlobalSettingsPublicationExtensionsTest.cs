using System;
using System.Collections.Generic;

using IAG.Infrastructure.Settings;
using IAG.InstallClient.Startup;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.Startup;

public class GlobalSettingsPublicationExtensionsTest
{
    [Fact(Skip = "Only executed manually and VS started as admin!")]
    public void PublishGlobalSettingsTest()
    {
        var testKey = "PublishGlobalSettingsTest";
        var testValue = Guid.NewGuid().ToString();
        var globalSettingMock = new Mock<IConfigurationSection>();
        globalSettingMock.Setup(m => m.Key).Returns(testKey);
        globalSettingMock.Setup(m => m.Value).Returns(testValue);

        var globalSettingsMock = new Mock<IConfigurationSection>();
        globalSettingsMock.Setup(m => m.GetChildren())
            .Returns(new List<IConfigurationSection>() { globalSettingMock.Object});
        var configSectionMock = new Mock<IConfiguration>();
        configSectionMock.Setup(m => m.GetSection(SettingsConst.GlobalSettingsKey))
            .Returns(globalSettingsMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(m => m.GetService(typeof(IConfiguration)))
            .Returns(configSectionMock.Object);
        var appBuilderMock = new Mock<IApplicationBuilder>();
        appBuilderMock.Setup(m => m.ApplicationServices)
            .Returns(serviceProviderMock.Object);

        CleanEnvironmentVariable(testKey);

        var valueBefore = Environment.GetEnvironmentVariable(testKey);

        appBuilderMock.Object.PublishGlobalSettings();
        var valueAfter = Environment.GetEnvironmentVariable(testKey);

        CleanEnvironmentVariable(testKey);

        Assert.Null(valueBefore);
        Assert.Equal(testValue, valueAfter);
    }

    private void CleanEnvironmentVariable(string key)
    {
        foreach (var target in Enum.GetValues<EnvironmentVariableTarget>())
        {
            Environment.SetEnvironmentVariable(key, null, target);
        }
    }
}