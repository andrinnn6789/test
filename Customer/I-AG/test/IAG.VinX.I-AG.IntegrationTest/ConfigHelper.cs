using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;

using Moq;

namespace IAG.VinX.IAG.IntegrationTest;

public static class ConfigHelper
{
    //private const string UrlCc = "http://testing-server:8086/";
    private const string UrlCc = "http://localhost:8086/";

    public static readonly HttpConfig JiraRestConfigProd = new()
    {
        BaseUrl = "https://jira.i-ag.ch/rest/api/2/",
        //Authentication = new BasicAuthentication {User = "sync", Password = "6Yrh1C2QepUZB49m"}
        // We had to change the user to vinxsupport due to a licensing issue with User "sync" on "VS"-board
        Authentication = new BasicAuthentication {User = "vinxsupport", Password = "vinx15lu"}
    };
        
    public static readonly HttpConfig JiraRestConfigTest = new()
    {
        BaseUrl = "http://jiratest/rest/api/2/",
        //Authentication = new BasicAuthentication {User = "sync", Password = "6Yrh1C2QepUZB49m"}
        // We had to change the user to vinxsupport due to a licensing issue with User "sync" on "VS"-board
        Authentication = new BasicAuthentication {User = "vinxsupport", Password = "vinx15lu"}
    };

    public static readonly BackendConfig CcConfigTestingServer = new()
    {
        UrlControlCenter = UrlCc,
        UrlAuth = UrlCc,
        Username = "support",
        Password = "Luz3rn!cc"
    };

    public static IControlCenterTokenRequest GetMockedTokenRequest()
    {
        var requestTokenMock = new Mock<IControlCenterTokenRequest>();
        requestTokenMock.Setup(m => m.GetConfig(It.IsAny<BackendConfig>(), It.IsAny<string>(), It.IsAny<IRequestResponseLogger>()))
            .Returns<BackendConfig, string, IRequestResponseLogger>((config, endpoint, _) => new HttpConfig
            {
                BaseUrl = config.UrlControlCenter + endpoint
            });
        return requestTokenMock.Object;
    }
}