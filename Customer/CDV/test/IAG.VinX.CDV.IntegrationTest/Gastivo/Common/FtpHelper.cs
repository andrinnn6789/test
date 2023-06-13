using IAG.Infrastructure.Ftp;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.Ftp;

namespace IAG.VinX.CDV.IntegrationTest.Gastivo.Common;

public static class FtpHelper
{
    public static GastivoFtpConfig CreateFtpConfig()
    {
       return new GastivoFtpConfig
        {
            Url = "dataconnect.gastivo.de",
            User = "999002u",
            Password = "}7a%3Mc'zAm?",
            ImportDir = "/OUTGOING",
            ExportDir = "/INCOMING",
            ArchiveDir = "/ARCHIVE"
        };
    }

    public static IFtpConnector CreateFtpConnector()
    {
        var ftpClient = new SecureFtpClient();
        return new FtpConnector(ftpClient);
    }
}