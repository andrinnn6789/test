using System;
using System.IO;
using System.Text;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Atlas;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Resource;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class GenericExporter: IDisposable
{
    public int ErrorCount { get; private set; }
    public int SuccessCount { get; private set; }

    private readonly IMessageLogger _messageLogger;

    private readonly string _exportFolder;

    public VinXConnector VinxConnector { get; }

    public GenericExporter(ISybaseConnection sybaseConnection, IMessageLogger messageLogger, string exportFolder)
    {
        _messageLogger = messageLogger;
        _exportFolder = exportFolder;

        VinxConnector = new VinXConnector(sybaseConnection);
    }
 
    public void ExportFile<T>(string prefix, T data)
    {
        try
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var bytesChecked = XmlCleaner.Serialize(data, encoding);
            var filename = prefix + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
            File.WriteAllBytes(Path.Combine(_exportFolder, filename), bytesChecked);

            SuccessCount++;
        }
        catch (Exception e)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GenericError, e.Message);
            _messageLogger.AddMessage(MessageTypeEnum.Debug, ResourceIds.GenericError, e);
            ErrorCount++;
        }
    }

    public void Dispose()
    {
        VinxConnector?.Dispose();
    }
}