using System;

using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;

public interface IPickListExporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasExportJobResult ExportPickLists(DateTime exportUntil, DateTime minimumLeadDate);
}