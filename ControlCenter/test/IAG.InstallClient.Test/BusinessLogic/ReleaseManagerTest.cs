using IAG.Infrastructure.Exception;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class ReleaseManagerTest
{
    [Fact]
    public void NoCcBaseUrlConfigTest()
    {
        Assert.Throws<LocalizableException>(() =>
        {
            _ = new ReleaseManager(new Mock<IConfiguration>().Object);
        });
    }
}