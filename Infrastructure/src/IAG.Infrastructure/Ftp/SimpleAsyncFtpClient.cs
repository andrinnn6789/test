using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using FluentFTP;

namespace IAG.Infrastructure.Ftp;

public class SimpleAsyncFtpClient : ISimpleAsyncFtpClient
{
    public IAsyncFtpClient FtpClient
    {
        set => _ftpClient = value;
    }

    private IAsyncFtpClient _ftpClient = new AsyncFtpClient();
        
    public void SetConfig(string host, string username, string password)
    {
        _ftpClient.Host = host;
        _ftpClient.Credentials = new NetworkCredential(username, password);
    }

    public async Task Connect(CancellationToken token = default)
    {
        await _ftpClient.Connect(token);
    }

    public async Task Disconnect(CancellationToken token = default)
    {
        await _ftpClient.Disconnect(token);
    }

    public async Task Download(Stream stream, string remotePath, CancellationToken token = default)
    {
        var succeed = await _ftpClient.DownloadStream(stream, remotePath, 0L, null, token);

        if (!succeed)
            throw new SimpleFtpClientException("download failed");
    }

    public async Task<List<string>> GetFileList(string remotePath, bool recursive, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(remotePath))
            throw new SimpleFtpClientException("path cannot be null or empty");
        
        var ftpOption = recursive ? FtpListOption.Recursive : FtpListOption.AllFiles;
        var list = new List<string>();

        foreach (var item in await _ftpClient.GetListing(remotePath, ftpOption, token))
        {
            if (item.Type != FtpObjectType.File)
                continue;
            
            list.Add(item.FullName.Replace(remotePath, ""));
        }

        return list;
    }

    public async Task Upload(Stream stream, string remotePath, bool overrideExistingFile, bool createRemoteDir, CancellationToken token = default)
    {
        var overrideExisting = overrideExistingFile ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
        var succeed = await _ftpClient.UploadStream(stream, remotePath, overrideExisting, createRemoteDir, null, token);

        if (succeed == FtpStatus.Failed)
            throw new SimpleFtpClientException("upload failed");
    }

    public async Task DeleteFile(string remotePath, CancellationToken token = default)
    {
        try
        {
            await _ftpClient.DeleteFile(remotePath, token);
        }
        catch (System.Exception e)
        {
            throw new SimpleFtpClientException("error deleting file", e);
        }
    }

    public async Task MoveFile(string remotePath, string remoteDest, bool overrideExistingFile, CancellationToken token = default)
    {
        var overrideExisting = overrideExistingFile ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
        var succeed = await _ftpClient.MoveFile(remotePath, remoteDest, overrideExisting, token);

        if (!succeed)
            throw new SimpleFtpClientException("moving file failed");
    }
}