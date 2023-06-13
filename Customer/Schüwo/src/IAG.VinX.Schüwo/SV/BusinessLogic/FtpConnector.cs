using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using IAG.Infrastructure.Ftp;
using IAG.VinX.Schüwo.SV.Config;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class FtpConnector: IDisposable
{
    private readonly FtpPathConfig _pathConfig;
    private readonly SimpleAsyncFtpClient _ftpClient;

    public FtpConnector(FtpEndpointConfig endpoint, FtpPathConfig pathConfig, CancellationToken token = default)
    {
        _pathConfig = pathConfig;
        _ftpClient = new SimpleAsyncFtpClient();
        _ftpClient.SetConfig(endpoint.Url, endpoint.User, endpoint.Password);
        _ftpClient.Connect(token).Wait(token);
    }

    public void UploadFile(Stream data, string fileName, CancellationToken token = default)
    {
        UploadData(data,fileName, true, token);
    }

    public void UploadImage(Stream data, string fileName, CancellationToken token = default)
    {
        UploadData(data, fileName, false, token);
    }

    public void DeleteImage(string fileName, CancellationToken token = default)
    {
        var fullPathImage = Path.Combine(_pathConfig.ImageDir, fileName);
        _ftpClient.DeleteFile(fullPathImage, token).Wait(token);
    }

    public List<string> GetImageList(CancellationToken token = default)
    {
        return _ftpClient.GetFileList(_pathConfig.ImageDir + "/", true, token).Result;
    }

    public void DownloadFile(Stream data, string fileName, CancellationToken token = default)
    {
        _ftpClient.Download(data, $"{_pathConfig.DownloadOrderDir}{fileName}", token).Wait(token);
    }

    public List<string> GetDownloadList(CancellationToken token = default)
    {
        return _ftpClient.GetFileList(_pathConfig.DownloadOrderDir, false, token).Result;
    }

    public void DeleteFile(string fileName, CancellationToken token = default)
    {
        _ftpClient.DeleteFile($"{_pathConfig.DownloadOrderDir}{fileName}", token).Wait(token);
    }

    private void UploadData(Stream data, string fileName, bool withMove, CancellationToken token = default)                  
    {
        var path = withMove ? _pathConfig.WorkingDir : _pathConfig.ImageDir;
        var fullPath = Path.Combine(path, fileName);

        _ftpClient.Upload(data, fullPath, true, false, token).Wait(token);
        if (!withMove) 
            return;
            
        var fullPathFinal = Path.Combine(_pathConfig.FinalDir, fileName);
        _ftpClient.MoveFile(fullPath, fullPathFinal, true, token).Wait(token);
    }

    public void Dispose()
    {
        _ftpClient.Disconnect().Wait();
        GC.SuppressFinalize(this);
    }
}