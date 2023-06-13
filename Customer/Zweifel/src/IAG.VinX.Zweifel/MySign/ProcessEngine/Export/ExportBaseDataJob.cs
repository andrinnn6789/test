using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Zweifel.Resource;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

[JobInfo("4FFB824F-B835-4A90-B22C-92DABF61C8AA", JobName)]
public class ExportBaseDataJob : JobBase<ExportBaseDataConfig, JobParameter, ExportResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "MySignExportBaseData";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    public ExportBaseDataJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        Result.Name = "Stammdaten";
        using (var helper = new GenericExporter(_sybaseConnectionFactory.CreateConnection(Config.Config.ConnectionString), this, Config.Config.ExportFolder))
        {
            var artBase = helper.VinxConnector.GetArticles();
            Result.SyncResult.Add(new ExportResultDetail {Name = "Artikelstamm", ExportCount = artBase.Items.Count});
            helper.ExportFile("Artikel_Export", artBase);

            HeartbeatAndCheckCancellation();
            var articleDescs = helper.VinxConnector.GetArticleDescs();
            Result.SyncResult.Add(new ExportResultDetail {Name = "Artikel-Beschreibung", ExportCount = articleDescs.Items.Count});
            helper.ExportFile("Artikel_Beschreib_Export", articleDescs);

            HeartbeatAndCheckCancellation();
            var prices = helper.VinxConnector.GetPrice();
            Result.SyncResult.Add(new ExportResultDetail {Name = "Preis-Export", ExportCount = prices.Items.Count});
            helper.ExportFile("Preise_Export", prices);

            Result.Result =
                helper.ErrorCount == 0 ? JobResultEnum.Success :
                helper.SuccessCount == 0 ? JobResultEnum.Failed :
                JobResultEnum.PartialSuccess;
        }

        base.ExecuteJob();
    }
}