﻿using IAG.Infrastructure.Ftp;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.Common.DataAccess;

public class FtpConnectorTest
{
    [Fact]
    public void SetConfig_ShouldSetConfig_ShouldConnect()
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig { Url = "fakeUrl", User = "fakeUser", Password = "fakePassword" };
        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.Connect());

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);

        // Act
        ftpConnector.SetConfig(ftpConfig);

        // Assert
        fakeFtpClient.Verify(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password), Times.Once);
        fakeFtpClient.Verify(f => f.Connect(), Times.Once);
    }

    [Fact]
    public void UploadFile_ShouldCallUploadOnClient()
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig
            { Url = "fakeUrl", User = "fakeUser", Password = "fakePassword", ExportDir = "/fakeExportDir" };
        var fakeData = Array.Empty<byte>();
        var fakeStream = new MemoryStream(fakeData);
        var fakeFileName = "fakeFileName";

        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.Upload(fakeStream, ftpConfig.ExportDir + fakeFileName, true));

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);
        ftpConnector.SetConfig(ftpConfig);

        // Act
        ftpConnector.UploadFile(fakeData, fakeFileName);

        // Assert
        fakeFtpClient.Verify(
            f => f.Upload(It.IsAny<MemoryStream>(), Path.Combine(ftpConfig.ExportDir, fakeFileName), true), Times.Once);
    }

    [Fact]
    public void DownloadFile_ShouldReturnStream_ShouldCallDownloadOnClient()
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig
            { Url = "fakeUrl", User = "fakeUser", Password = "fakePassword", ExportDir = "/fakeExportDir" };
        var fakeStream = new MemoryStream();
        var fakeFile = "fakeFile/Path/File.csv";

        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.Download(fakeStream, fakeFile));

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);
        ftpConnector.SetConfig(ftpConfig);

        // Act
        var result = ftpConnector.DownloadFile(fakeFile);

        // Assert
        Assert.NotNull(result);
        fakeFtpClient.Verify(f => f.Download(It.IsAny<MemoryStream>(), fakeFile), Times.Once);
    }

    [Fact]
    public void GetFiles_ShouldReturnFileList_ShouldCallGetFileListOnClient()
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig
            { Url = "fakeUrl", User = "fakeUser", Password = "fakePassword", ImportDir = "/fakeImportrDir" };
        var searchPattern = "ImportFile";
        var fileList = new List<string> { "ImportFileA.csv", "ImportFileB.csv", "ExportFileA.csv", "ExportFileA.txt" };

        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.GetFileList(ftpConfig.ImportDir)).Returns(fileList);

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);
        ftpConnector.SetConfig(ftpConfig);

        // Act
        var result = ftpConnector.GetFiles(searchPattern, "csv");

        // Assert
        Assert.Equal(2, result.Count);
        fakeFtpClient.Verify(f => f.GetFileList(ftpConfig.ImportDir), Times.Once);
    }

    [Theory]
    [InlineData(true, "/fakeImportDir/success")]
    [InlineData(false, "/fakeImportDir/error")]
    public void ArchiveFile_ShouldCallRenameOnClient(bool wasSuccessful, string expectedArchiveDirectory)
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig
        {
            Url = "fakeUrl", User = "fakeUser", Password = "fakePassword", ImportDir = "/fakeImportDir",
            ImportSuccessDir = "/fakeImportDir/success",
            ImportErrorDir = "/fakeImportDir/error"
        };
        var fakeFile = "/File.csv";
        var fakeImportFile = ftpConfig.ImportDir + fakeFile;

        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.MoveFile(fakeImportFile, It.IsAny<string>()));

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);
        ftpConnector.SetConfig(ftpConfig);

        // Act
        ftpConnector.ArchiveFile(fakeImportFile, wasSuccessful);

        // Assert
        fakeFtpClient.Verify(f => f.MoveFile(fakeImportFile, expectedArchiveDirectory + fakeFile), Times.Once);
    }

    [Fact]
    public void Dispose_ShouldCallDisconnectOnClient()
    {
        // Arrange
        var fakeFtpClient = new Mock<ISecureFtpClient>();
        var ftpConfig = new WamasFtpConfig { Url = "fakeUrl", User = "fakeUser", Password = "fakePassword" };

        fakeFtpClient.Setup(f => f.SetConfig(ftpConfig.Url, ftpConfig.User, ftpConfig.Password));
        fakeFtpClient.Setup(f => f.Disconnect());

        var ftpConnector = new FtpConnector(fakeFtpClient.Object);
        ftpConnector.SetConfig(ftpConfig);

        // Act
        ftpConnector.Dispose();

        // Assert
        fakeFtpClient.Verify(f => f.Disconnect(), Times.Once);
    }
}