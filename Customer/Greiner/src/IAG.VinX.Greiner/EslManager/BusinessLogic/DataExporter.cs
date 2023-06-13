using System;
using System.IO;
using System.Text;

using IAG.Infrastructure.Atlas;
using IAG.VinX.Greiner.EslManager.Config;

namespace IAG.VinX.Greiner.EslManager.BusinessLogic;

public class DataExporter
{
    private readonly EslExportConfig _config;

    public DataExporter(EslExportConfig config)
    {
        _config = config;
        if (!Directory.Exists(_config.ExportRoot))
            Directory.CreateDirectory(_config.ExportRoot);
    }

    public void ExportFile<T>(string prefix, T data)
    {
        var encoding = Encoding.GetEncoding("utf-8");
        var bytesChecked = XmlCleaner.Serialize(data, encoding);
        var filename = prefix + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
        File.WriteAllBytes(Path.Combine(_config.ExportRoot, filename), bytesChecked);
    }
}