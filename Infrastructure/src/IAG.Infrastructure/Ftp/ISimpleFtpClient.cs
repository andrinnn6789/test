using System.Collections.Generic;
using System.IO;

using FluentFTP;

namespace IAG.Infrastructure.Ftp;

public interface ISimpleFtpClient
{
    IFtpClient FtpClient { set; }
    void SetConfig(string host, string username, string password);
    void Connect();
    void Disconnect();
    void Download(Stream stream, string remotePath);
    List<string> GetFileList(string remotePath, bool recursive);
    void Upload(Stream stream, string remotePath, bool overrideExistingFile, bool createRemoteDir);
    void DeleteFile(string remotePath);
    void MoveFile(string remotePath, string remoteDest, bool overrideExistingFile);
}