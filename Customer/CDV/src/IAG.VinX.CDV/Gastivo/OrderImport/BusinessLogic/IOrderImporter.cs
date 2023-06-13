using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;

public interface IOrderImporter
{
    void SetConfig(GastivoFtpConfig gastivoFtpConfig, string connectionString, IMessageLogger messageLogger);
    
    GastivoImportJobResult ImportOrders();
}