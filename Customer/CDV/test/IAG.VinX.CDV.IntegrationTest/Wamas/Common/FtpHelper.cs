using IAG.Infrastructure.Ftp;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.Common;

public static class FtpHelper
{
    public static WamasFtpConfig CreateFtpConfig()
    {
       return new WamasFtpConfig
        {
            Url = "ftptest.transgourmet.ch",
            User = "sftp.vinxtest",
            Password = "8J2zM0QJ",
            ImportDir = "/toVinX",
            ExportDir = "/fromVinX",
            ImportSuccessDir = "/toVinX/success",
            ImportErrorDir = "/toVinX/error"
        };
    }

    public static IFtpConnector CreateFtpConnector()
    {
        var ftpClient = new SecureFtpClient();
        return new FtpConnector(ftpClient);
    }
}