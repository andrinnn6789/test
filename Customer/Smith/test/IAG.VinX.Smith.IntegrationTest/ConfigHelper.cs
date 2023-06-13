using IAG.Common.Dto;
using IAG.Common.WoD.Connectors;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

using Moq;

namespace IAG.VinX.Smith.IntegrationTest;

public static class ConfigHelper
{
    // prod
    public static IWodConnector WodConnector =>
        new WodConnector(new MockILogger<WodConnector>(), GetWodConfigLoader());

    public static readonly HelloTessSystemConfig SmithAndSmithSystemConfig = new()
    {
        Name = "SmithAndSmith",
        Url = "http://smithandsmith.staging.hellotess.com/v1/",
        ApiKey = "4cc8d495a9d09ebaa278",
        PriceGroupForProdCost = 3,
        CustomerForProdCost = 15085
    };

    private static ProviderSetting ProviderSettingWod() =>
        new()
        {
            BaseUrl = "https://prod-001.worldofdocuments.ch/wod",
            Password = "EcTxH2XysWB71BahhRLf",
            UserName = "vinx-test",
            ParticipantId = "vinx-test"

        };

    private static IWodConfigLoader GetWodConfigLoader()
    {
        var mockIWodConfigLoader = new Mock<IWodConfigLoader>();
        mockIWodConfigLoader.Setup(m => m.ProviderSetting()).Returns(ProviderSettingWod);
        return mockIWodConfigLoader.Object;
    }
}

public static class ConfigHelperMock
{
    public static ProviderSetting ProviderSettingWod(int port) =>
        new()
        {
            BaseUrl = $"http://localhost:{port}/wod",
            Password = "xx",
            UserName = "me",
            ParticipantId = "t"

        };
}