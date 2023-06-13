using System.Collections.Generic;

using IAG.VinX.CDV.Gastivo.Common.Config;

namespace IAG.VinX.CDV.Gastivo.Common.Ftp;

public interface IFtpConnector
{
    void SetConfig(GastivoFtpConfig gastivoFtpConfig);

    void UploadFile(byte[] data, string fileName);

    byte[] DownloadFile(string fileName);

    List<string> GetFiles(string searchPattern);

    void DeleteFile(string file);
}