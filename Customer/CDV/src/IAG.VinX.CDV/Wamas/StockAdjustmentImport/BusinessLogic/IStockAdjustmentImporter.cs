using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;

public interface IStockAdjustmentImporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasImportJobResult ImportStockAdjustments();
}