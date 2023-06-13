using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentFTP;

using IAG.Infrastructure.Ftp;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Ftp;

public class SimpleAsyncFtpClientTest
{
    public SimpleAsyncFtpClientTest()
    {
        SimpleFtpClientMock();
    }

    private Mock<IAsyncFtpClient> FtpClientMock { get; } = new();
    private SimpleAsyncFtpClient SimpleFtpClient { get; set; }

    [Fact]
    public void ShouldReturnSimpleFtpClient()
    {
        Assert.NotNull(SimpleFtpClient);
    }

    [Fact]
    public void SetConfig()
    {
        SimpleFtpClient.SetConfig("test", "test", "test");
    }

    [Fact]
    public async Task ConnectDisconnect()
    {
        await SimpleFtpClient.Connect();
        await SimpleFtpClient.Disconnect();
    }

    [Fact]
    public async Task FtpDownloadSuccess()
    {
        SetupDownloadStream(true);

        using var stream = new MemoryStream();
        await SimpleFtpClient.Download(stream, null);
        Assert.NotNull(stream);
    }

    [Fact]
    public async Task FtpDownloadFail()
    {
        SetupDownloadStream(false);

        using var stream = new MemoryStream();
        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.Download(stream, null));
    }

    [Fact]
    public async Task FtpGetFileList()
    {
        SetupGetListingEmptyData();

        var fileList = await SimpleFtpClient.GetFileList("test", true);
        Assert.False(fileList.Any());
    }

    [Fact]
    public async Task FtpGetFileListFail()
    {
        SetupGetListingEmptyData();

        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.GetFileList(null, false));
        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.GetFileList("", true));
    }

    [Fact]
    public async Task FtpGetFileListRecursive()
    {
        SetupGetListingWithData();

        var fileList = await SimpleFtpClient.GetFileList("test", true);
        Assert.True(fileList.Count == 1);
    }

    [Fact]
    public async Task FtpGetFileListNoneRecursive()
    {
        SetupGetListingWithData();

        var fileList = await SimpleFtpClient.GetFileList("test", false);
        Assert.True(fileList.Count == 1);
    }

    [Fact]
    public async Task FtpUploadSuccess()
    {
        SetupUploadStream(FtpStatus.Success);
        await SimpleFtpClient.Upload(null, null, false, true);
    }

    [Fact]
    public async Task FtpUploadFail()
    {
        SetupUploadStream(FtpStatus.Failed);
        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.Upload(null, null, false, true));
    }

    [Fact]
    public async Task FtpDeleteFileSuccess()
    {
        FtpClientMock.Setup(client => client
            .DeleteFile(
                It.IsAny<string>(), It.IsAny<CancellationToken>()));

        await SimpleFtpClient.DeleteFile(null);
    }

    [Fact]
    public async Task FtpDeleteFileFail()
    {
        FtpClientMock.Setup(client => client
                .DeleteFile(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception());

        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.DeleteFile(null));
    }

    [Fact]
    public async Task FtpMoveFileSuccess()
    {
        SetupMoveFile(true);

        await SimpleFtpClient.MoveFile(null, null, false);
    }
    
    [Fact]
    public async Task FtpMoveFileFail()
    {
        SetupMoveFile(false);

        await Assert.ThrowsAsync<SimpleFtpClientException>(() => SimpleFtpClient.MoveFile(null, null, false));
    }

    private void SimpleFtpClientMock()
    {
        SimpleFtpClient = new SimpleAsyncFtpClient
        {
            FtpClient = FtpClientMock.Object
        };
    }

    private void SetupDownloadStream(bool returnValue)
    {
        FtpClientMock.Setup(client => client.DownloadStream(It.IsAny<MemoryStream>(), It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<IProgress<FtpProgress>>(), It.IsAny<CancellationToken>())).ReturnsAsync(returnValue);
    }

    private void SetupGetListingEmptyData()
    {
        FtpClientMock.Setup(client =>
                client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<FtpListItem>());
    }

    private void SetupGetListingWithData()
    {
        var ftpList = new[]
        {
            new FtpListItem { Type = FtpObjectType.File, FullName = "" },
            new FtpListItem { Type = FtpObjectType.Directory, FullName = "" },
            new FtpListItem { Type = FtpObjectType.Link, FullName = "" }
        };

        FtpClientMock.Setup(client =>
                client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ftpList);
    }
    
    private void SetupUploadStream(FtpStatus ftpStatus)
    {
        FtpClientMock.Setup(client => client
                .UploadStream(
                    It.IsAny<MemoryStream>(),
                    It.IsAny<string>(),
                    It.IsAny<FtpRemoteExists>(),
                    It.IsAny<bool>(),
                    It.IsAny<IProgress<FtpProgress>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(ftpStatus);
    }

    private void SetupMoveFile(bool returnValue)
    {
        FtpClientMock.Setup(client => client
                .MoveFile(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<FtpRemoteExists>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnValue);
    }
}