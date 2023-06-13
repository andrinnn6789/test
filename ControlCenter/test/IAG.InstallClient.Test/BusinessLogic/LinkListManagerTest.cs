using IAG.Infrastructure.Exception;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class LinkListManagerTest
{
    [Fact]
    public void NoCcBaseUrlConfigTest()
    {
        Assert.Throws<LocalizableException>(() =>
        {
            _ = new LinkListManager(new Mock<IConfiguration>().Object);
        });
    }
}