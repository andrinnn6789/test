using System.Collections.Generic;

using IAG.VinX.CDV.Wamas.Common.Config;

namespace IAG.VinX.CDV.Wamas.Common.DataAccess;

public interface IFtpConnector
{
    void SetConfig(WamasFtpConfig wamasFtpConfig);

    void UploadFile(byte[] data, string fileName);

    byte[] DownloadFile(string fileName);

    List<string> GetFiles(string searchPattern, string fileEnding);

    void ArchiveFile(string file, bool wasSuccessful);
}