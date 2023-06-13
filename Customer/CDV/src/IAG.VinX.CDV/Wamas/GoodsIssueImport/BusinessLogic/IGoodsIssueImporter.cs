using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;

public interface IGoodsIssueImporter
{
    void SetConfig(WamasFtpConfig wamasFtpConfig, string connectionString, IMessageLogger messageLogger);

    WamasImportJobResult ImportGoodsIssues();
}