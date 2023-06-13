using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Ftp;
using IAG.VinX.CDV.Wamas.Common.Config;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess;

public class FtpConnector : IDisposable, IFtpConnector
{
    private readonly ISecureFtpClient _ftpClient;
    private WamasFtpConfig _wamasFtpConfig;

    public FtpConnector(ISecureFtpClient ftpClient)
    {
        _ftpClient = ftpClient;
    }

    public void SetConfig(WamasFtpConfig wamasFtpConfig)
    {
        _wamasFtpConfig = wamasFtpConfig;

        _ftpClient.SetConfig(wamasFtpConfig.Url, wamasFtpConfig.User, wamasFtpConfig.Password);
        _ftpClient.Connect();
    }

    public void UploadFile(byte[] data, string fileName)
    {
        var path = _wamasFtpConfig.ExportDir;
        var fullPath = Path.Combine(path, fileName);
        var stream = new MemoryStream(data);
        _ftpClient.Upload(stream, fullPath, true);
    }

    public byte[] DownloadFile(string filePath)
    {
        var stream = new MemoryStream();
        _ftpClient.Download(stream, filePath);

        return stream.ToArray();
    }

    public List<string> GetFiles(string searchPattern, string fileEnding)
    {
        var path = _wamasFtpConfig.ImportDir;
        return _ftpClient.GetFileList(path)
            .Where(f => f.Contains($"{searchPattern}") && f.Contains($".{fileEnding}"))
            .ToList();
    }

    public void ArchiveFile(string file, bool wasSuccessful)
    {
        var destinationDirectory = wasSuccessful ? _wamasFtpConfig.ImportSuccessDir : _wamasFtpConfig.ImportErrorDir;
        var destinationFileName = file.Replace(_wamasFtpConfig.ImportDir, destinationDirectory);
        _ftpClient.MoveFile(file, destinationFileName);
    }

    public void Dispose()
    {
        _ftpClient?.Disconnect();
        GC.SuppressFinalize(this);
    }
}