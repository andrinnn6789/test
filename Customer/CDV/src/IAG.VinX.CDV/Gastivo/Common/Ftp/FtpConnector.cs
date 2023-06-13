using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Ftp;
using IAG.VinX.CDV.Gastivo.Common.Config;

namespace IAG.VinX.CDV.Gastivo.Common.Ftp;

public class FtpConnector : IDisposable, IFtpConnector
{
    private readonly ISecureFtpClient _ftpClient;
    private GastivoFtpConfig _gastivoFtpConfig;

    public FtpConnector(ISecureFtpClient ftpClient)
    {
        _ftpClient = ftpClient;
    }

    public void SetConfig(GastivoFtpConfig gastivoFtpConfig)
    {
        _gastivoFtpConfig = gastivoFtpConfig;

        _ftpClient.SetConfig(gastivoFtpConfig.Url, gastivoFtpConfig.User, gastivoFtpConfig.Password);
        _ftpClient.Connect();
    }

    public void UploadFile(byte[] data, string fileName)
    {
        var fullPath = Path.Combine(_gastivoFtpConfig.ExportDir, fileName).Replace(@"\", "/");
        var stream = new MemoryStream(data);
        _ftpClient.Upload(stream, fullPath, true);
    }

    public byte[] DownloadFile(string filePath)
    {
        var stream = new MemoryStream();
        _ftpClient.Download(stream, filePath);
        return stream.ToArray();
    }

    public List<string> GetFiles(string searchPattern)
    {
        return _ftpClient.GetFileList(_gastivoFtpConfig.ImportDir)
            .Where(f => f.ToLower().Contains($"{searchPattern.ToLower()}"))
            .ToList();
    }

    public void DeleteFile(string filePath)
    {
        _ftpClient.DeleteFile(filePath);
    }

    public void Dispose()
    {
        _ftpClient?.Disconnect();
        GC.SuppressFinalize(this);
    }
}