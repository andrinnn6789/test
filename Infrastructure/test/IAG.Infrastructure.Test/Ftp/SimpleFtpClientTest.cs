using System;
using System.IO;
using System.Linq;

using FluentFTP;

using IAG.Infrastructure.Ftp;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Ftp;

public class SimpleFtpClientTest
{
    private Mock<IFtpClient> FtpClientMock { get; } = new();
    private SimpleFtpClient SimpleFtpClient { get; set; }

    public SimpleFtpClientTest()
    {
        SimpleFtpClientMock();
    }

    [Fact]
    public void ShouldReturnSimpleFtpClient()
    {
        Assert.NotNull(SimpleFtpClient);
    }
        
    [Fact]
    public void EmptyCtor()
    {
        _ = new SimpleFtpClient();
    }
        
    [Fact]
    public void SetConfig()
    {
        SimpleFtpClient.SetConfig("test", "test", "test");
    }
        
    [Fact]
    public void ConnectDisconnect()
    {
        SimpleFtpClient.Connect();
        SimpleFtpClient.Disconnect();
    }

        [Fact]
        public void FtpDownloadSuccess()
        {
            FtpClientMock.Setup(client => client.DownloadStream(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<Action<FtpProgress>>())).Returns(true);

        using var stream = new MemoryStream();
        SimpleFtpClient.Download(stream, null);
        Assert.NotNull(stream);
    }

        [Fact]
        public void FtpDownloadFail()
        {
            FtpClientMock.Setup(client => client.DownloadStream(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<Action<FtpProgress>>())).Returns(false);

        using var stream = new MemoryStream();
        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.Download(stream, null));
    }

    [Fact]
    public void FtpGetFileList()
    {
        FtpClientMock.Setup(client => client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>())).Returns(Array.Empty<FtpListItem>());

        var fileList = SimpleFtpClient.GetFileList("test", true);
        Assert.False(fileList.Any());
    }

    [Fact]
    public void FtpGetFileListFail()
    {
        FtpClientMock.Setup(client => client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>())).Returns(Array.Empty<FtpListItem>());

        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.GetFileList(null, false));
        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.GetFileList("", true));
    }

        [Fact]
        public void FtpGetFileListRecursive()
        {
            var ftpList = new[]
            {
                new FtpListItem() {Type = FtpObjectType.File, FullName = ""},
                new FtpListItem() {Type = FtpObjectType.Directory, FullName = ""},
                new FtpListItem() {Type = FtpObjectType.Link, FullName = ""}
            };

        FtpClientMock.Setup(client => client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>())).Returns(ftpList);

            var fileList = SimpleFtpClient.GetFileList("test", true);
            Assert.True(fileList.Count == 1);
        }

        [Fact]
        public void FtpGetFileListNoneRecursive()
        {
            var ftpList = new[]
            {
                new FtpListItem() {Type = FtpObjectType.File, FullName = ""},
                new FtpListItem() {Type = FtpObjectType.Directory, FullName = ""},
                new FtpListItem() {Type = FtpObjectType.Link, FullName = ""}
            };

            FtpClientMock.Setup(client => client.GetListing(It.IsAny<string>(), It.IsAny<FtpListOption>())).Returns(ftpList);

            var fileList = SimpleFtpClient.GetFileList("test", false);
            Assert.True(fileList.Count == 1);
        }

        [Fact]
        public void FtpUploadSuccess()
        {
            FtpClientMock.Setup(client => client
                    .UploadStream(
                        It.IsAny<MemoryStream>(),
                        It.IsAny<string>(),
                        It.IsAny<FtpRemoteExists>(),
                        It.IsAny<bool>(),
                        It.IsAny<Action<FtpProgress>>()))
                .Returns(FtpStatus.Success);

        SimpleFtpClient.Upload(null, null, false, true);
    }

        [Fact]
        public void FtpUploadFail()
        {
            FtpClientMock.Setup(client => client
                .UploadStream(
                    It.IsAny<MemoryStream>(), 
                    It.IsAny<string>(), 
                    It.IsAny<FtpRemoteExists>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<Action<FtpProgress>>())).Returns(FtpStatus.Failed);

        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.Upload(null, null, false, true));
    }

    [Fact]
    public void FtpDeleteFileSuccess()
    {
        FtpClientMock.Setup(client => client
            .DeleteFile(
                It.IsAny<string>()));

        SimpleFtpClient.DeleteFile(null);
    }

    [Fact]
    public void FtpDeleteFileFail()
    {
        FtpClientMock.Setup(client => client
                .DeleteFile(
                    It.IsAny<string>()))
            .Throws(new System.Exception());

        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.DeleteFile(null));
    }

    [Fact]
    public void FtpMoveFileSuccess()
    {
        FtpClientMock.Setup(client => client
                .MoveFile(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<FtpRemoteExists>()))
            .Returns(true);

        SimpleFtpClient.MoveFile(null, null, false);
    }

    [Fact]
    public void FtpMoveFileFail()
    {
        FtpClientMock.Setup(client => client
                .MoveFile(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<FtpRemoteExists>()))
            .Returns(false);

        Assert.Throws<SimpleFtpClientException>(() => SimpleFtpClient.MoveFile(null, null, false));
    }

    private void SimpleFtpClientMock()
    {
        SimpleFtpClient = new SimpleFtpClient(string.Empty, string.Empty, string.Empty)
        {
            FtpClient = FtpClientMock.Object
        };
    }
}