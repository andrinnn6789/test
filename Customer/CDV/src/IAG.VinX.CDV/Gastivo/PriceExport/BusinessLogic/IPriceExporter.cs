using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;

public interface IPriceExporter
{
    void SetConfig(GastivoFtpConfig gastivoFtpConfig, string connectionString, IMessageLogger messageLogger);
    
    GastivoExportJobResult ExportPrices();
}