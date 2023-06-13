using System.IO;

using IAG.VinX.Schüwo.SV.Config;

namespace IAG.VinX.Schüwo.IntegrationTest;

public static class ConfigHelper
{
    private static readonly bool IsTestIag = true;
    private const string FtpRootTest = "/test/Schuewo_SV";

    public static FtpEndpointConfig FtpEndpointTest
    {
        get
        {
            var config = new FtpEndpointConfig();
            if (IsTestIag)
            {
                config.Url = "ftp.i-ag.ch";
                config.User = "iag.mj";
                config.Password = "ftmj35fr";
            }
            else
            {
                config.Url = "sandbox.popscan.ch";
                config.User = "schuewo";
                config.Password = "h92hds9cpw8q";
            }

            return config;
        }
    }

    public static FtpPathConfig FtpPathConfigTest
    {
        get
        {
            var config = new FtpPathConfig();
            if (!IsTestIag) 
                return config;

            // test-config iag
            config.ImageDir = $"{FtpRootTest}{config.ImageDir}";
            config.WorkingDir = $"{FtpRootTest}{config.WorkingDir}";
            config.FinalDir = $"{FtpRootTest}{config.FinalDir}";
            config.DownloadOrderDir = $"{FtpRootTest}{config.DownloadOrderDir}";

            return config;
        }
    }

    public static string ArticleRootImagePathTestIag
    {
        get
        {
            var path = Path.Combine(Path.GetTempPath(), "Schüwo\\Artikel");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string ArticleImageRootPathArchive 
    {
        get
        {
            var path = Path.Combine(Path.GetTempPath(), "Schüwo\\Inaktive Artikel");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}