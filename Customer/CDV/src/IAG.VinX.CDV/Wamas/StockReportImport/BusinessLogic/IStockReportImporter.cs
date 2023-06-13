using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;

public interface IStockReportImporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasImportJobResult ImportStockReports();
}