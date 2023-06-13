using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.GoodsIssueImport.ProcessEngine;

[JobInfo("BA534E74-AF1C-4F95-839F-05CB3F480420", JobName)]
public class GoodsIssueImportJob : JobBase<GoodsIssueImportJobConfig, JobParameter, WamasImportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasGoodsIssueImportJobName;

    private readonly IGoodsIssueImporter _goodsIssueImporter;

    public GoodsIssueImportJob(IGoodsIssueImporter goodsIssueImporter)
    {
        _goodsIssueImporter = goodsIssueImporter;
    }

    protected override void ExecuteJob()
    {
        _goodsIssueImporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _goodsIssueImporter.ImportGoodsIssues();

        Result.Result = result.Result;
        Result.ImportedCount = result.ImportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}