using System;

using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;

public interface IPurchaseOrderExporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasExportJobResult ExportPurchaseOrders(DateTime exportUntil);
}