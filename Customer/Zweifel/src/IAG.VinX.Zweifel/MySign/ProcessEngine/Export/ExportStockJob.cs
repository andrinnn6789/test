using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Zweifel.Resource;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

[JobInfo("2815B3D8-E9F6-425C-8E67-78F66C66324D", JobName)]
public class ExportStockJob : JobBase<ExportStockConfig, JobParameter, ExportResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "MySignExportStock";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    public ExportStockJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        Result.Name = "Stammdaten";
        using (var helper = new GenericExporter(_sybaseConnectionFactory.CreateConnection(Config.Config.ConnectionString), this, Config.Config.ExportFolder))
        {
            var stocks = helper.VinxConnector.GetStock();
            Result.SyncResult.Add(new ExportResultDetail { Name = "Bestand", ExportCount = stocks.Items.Count });
            helper.ExportFile("Bestand_Export", stocks);

            Result.Result =
                helper.ErrorCount == 0 ? JobResultEnum.Success :
                helper.SuccessCount == 0 ? JobResultEnum.Failed :
                JobResultEnum.PartialSuccess;
        }

        base.ExecuteJob();
    }
}