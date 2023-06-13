using System.Collections.Generic;
using System.IO;

namespace IAG.Infrastructure.Ftp;

public interface ISecureFtpClient
{
    void SetConfig(string host, string username, string password);
    void Connect();
    void Disconnect();
    void Download(Stream stream, string remotePath);
    List<string> GetFileList(string remotePath);
    void Upload(Stream stream, string remotePath, bool overrideExistingFile);
    void MoveFile(string remotePath, string remoteDest);
    void DeleteFile(string remotePath);
}