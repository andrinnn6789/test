using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FluentFTP;

namespace IAG.Infrastructure.Ftp;

public interface ISimpleAsyncFtpClient
{
    IAsyncFtpClient FtpClient { set; }
    void SetConfig(string host, string username, string password);
    Task Connect(CancellationToken token = default);
    Task Disconnect(CancellationToken token = default);
    Task Download(Stream stream, string remotePath, CancellationToken token = default);
    Task<List<string>> GetFileList(string remotePath, bool recursive, CancellationToken token = default);
    Task Upload(Stream stream, string remotePath, bool overrideExistingFile, bool createRemoteDir, CancellationToken token = default);
    Task DeleteFile(string remotePath, CancellationToken token = default);
    Task MoveFile(string remotePath, string remoteDest, bool overrideExistingFile, CancellationToken token = default);
}