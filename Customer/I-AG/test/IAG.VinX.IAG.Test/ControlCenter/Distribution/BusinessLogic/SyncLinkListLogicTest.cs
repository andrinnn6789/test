using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class SyncLinkListLogicTest
{
    private readonly SyncLinkListLogic _logic;
    private readonly Mock<ILinkAdminClient> _linkAdminClient;
    private readonly Mock<ILinkListScanner> _linkListScannerMock;

    public SyncLinkListLogicTest()
    {
        _linkListScannerMock = new Mock<ILinkListScanner>();
        _linkAdminClient = new Mock<ILinkAdminClient>();
        _logic = new SyncLinkListLogic(_linkListScannerMock.Object, _linkAdminClient.Object);
    }

    [Fact]
    public async Task SyncLinkListTest()
    {
        _linkListScannerMock.Setup(m => m.ScanAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LinkData> { new()
            {
                Name = "TestLink",
                Link = "https://test.link",
                Description = "TestDescription"
            }});

        List<LinkData> syncedLinkList = null;
        _linkAdminClient.Setup(m => m.SyncLinksAsync(It.IsAny<IEnumerable<LinkData>>()))
            .Callback<IEnumerable<LinkData>>((links) => syncedLinkList = links.ToList())
            .ReturnsAsync(new List<LinkInfo> { new()
            {
                Name = "TestLink",
                Url = "https://test.link",
                Description = "TestDescription"
            }});

        var syncResult = new SyncResult();
        await _logic.SyncLinkListAsync("FakeLinkListsPath", syncResult, new Mock<IJobHeartbeatObserver>().Object);

        Assert.NotNull(syncedLinkList);
        var syncedLink = Assert.Single(syncedLinkList);
        Assert.Equal("TestLink", syncedLink.Name);
        Assert.Equal("https://test.link", syncedLink.Link);
        Assert.Equal("TestDescription", syncedLink.Description);
        Assert.Equal(1, syncResult.SuccessCount);
        Assert.Equal(0, syncResult.ErrorCount);
    }

    [Fact]
    public async Task SyncEmptyLinkListTest()
    {
        _linkListScannerMock.Setup(m => m.ScanAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LinkData>());

        var syncResult = new SyncResult();
        await _logic.SyncLinkListAsync("FakeLinkListsPath", syncResult, new Mock<IJobHeartbeatObserver>().Object);

        Assert.Equal(0, syncResult.SuccessCount);
        Assert.Equal(0, syncResult.ErrorCount);
    }
}