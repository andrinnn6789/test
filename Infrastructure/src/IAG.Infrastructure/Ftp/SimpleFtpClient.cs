using System.Collections.Generic;
using System.IO;
using System.Net;

using FluentFTP;

namespace IAG.Infrastructure.Ftp;

public class SimpleFtpClient : ISimpleFtpClient
{
    public IFtpClient FtpClient
    {
        set => _ftpClient = value;
    }

    private IFtpClient _ftpClient = new FtpClient();

    public SimpleFtpClient()
    {
            
    }
        
    // TODO: remove this ctor and use SetConfig instead
    public SimpleFtpClient(string host, string username, string password)
    {
        _ftpClient.Host = host;
        _ftpClient.Credentials = new NetworkCredential(username, password);
    }
        
    public void SetConfig(string host, string username, string password)
    {
        _ftpClient.Host = host;
        _ftpClient.Credentials = new NetworkCredential(username, password);
    }

    public void Connect()
    {
        _ftpClient.Connect();
    }

    public void Disconnect()
    {
        _ftpClient.Disconnect();
    }

    public void Download(Stream stream, string remotePath)
    {
        var succeed = _ftpClient.DownloadStream(stream, remotePath);

        if (!succeed)
            throw new SimpleFtpClientException("download failed");
    }

    public List<string> GetFileList(string remotePath, bool recursive)
    {
        if (string.IsNullOrEmpty(remotePath))
        {
            throw new SimpleFtpClientException("path cannot be null or empty");
        }

        var ftpOption = recursive ? FtpListOption.Recursive : FtpListOption.AllFiles;
        var list = new List<string>();

        foreach (var item in _ftpClient.GetListing(remotePath, ftpOption))
        {
            if (item.Type != FtpObjectType.File)
                continue;
            
            list.Add(item.FullName.Replace(remotePath, ""));
        }

        return list;
    }

    public void Upload(Stream stream, string remotePath, bool overrideExistingFile, bool createRemoteDir)
    {
        var overrideExisting = overrideExistingFile ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
        var succeed = _ftpClient.UploadStream(stream, remotePath, overrideExisting, createRemoteDir);

        if (succeed == FtpStatus.Failed)
            throw new SimpleFtpClientException("upload failed");
    }

    public void DeleteFile(string remotePath)
    {
        try
        {
            _ftpClient.DeleteFile(remotePath);
        }
        catch (System.Exception e)
        {
            throw new SimpleFtpClientException("error deleting file", e);
        }
    }

    public void MoveFile(string remotePath, string remoteDest, bool overrideExistingFile)
    {
        var overrideExisting = overrideExistingFile ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
        var succeed = _ftpClient.MoveFile(remotePath, remoteDest, overrideExisting);

        if (!succeed)
            throw new SimpleFtpClientException("moving file failed");
    }
}
