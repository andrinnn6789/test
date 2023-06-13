using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;

public interface IGoodsReceiptImporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasImportJobResult ImportGoodsReceipts();
}