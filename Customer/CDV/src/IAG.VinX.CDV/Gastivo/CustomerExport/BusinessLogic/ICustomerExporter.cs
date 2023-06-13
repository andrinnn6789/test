using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;

public interface ICustomerExporter
{
    void SetConfig(GastivoFtpConfig gastivoFtpConfig, string connectionString, IMessageLogger messageLogger);
    
    GastivoExportJobResult ExportCustomers();
}