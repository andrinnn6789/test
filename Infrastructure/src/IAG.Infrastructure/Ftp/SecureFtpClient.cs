using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Renci.SshNet;

namespace IAG.Infrastructure.Ftp;

[ExcludeFromCodeCoverage]
public class SecureFtpClient : ISecureFtpClient
{
    private SftpClient _sftpClient;
    
    public void SetConfig(string host, string username, string password)
    {
        _sftpClient = new SftpClient(host, username, password);
    }

    public void Connect()
    {
        _sftpClient.Connect();
    }

    public void Disconnect()
    {
        _sftpClient.Disconnect();
    }

    public void Download(Stream stream, string remotePath)
    {
        _sftpClient.DownloadFile(remotePath, stream);
    }

    public List<string> GetFileList(string remotePath)
    {
        return _sftpClient.ListDirectory(remotePath).Select(s => s.FullName).ToList();
    }

    public void Upload(Stream stream, string remotePath, bool overrideExistingFile)
    {
        stream.Position = 0;
        _sftpClient.UploadFile(stream, remotePath, overrideExistingFile);
    }

    public void MoveFile(string remotePath, string remoteDest)
    {
        _sftpClient.RenameFile(remotePath, remoteDest);
    }
    
    public void DeleteFile(string remotePath)
    {
        _sftpClient.DeleteFile(remotePath);
    }
}