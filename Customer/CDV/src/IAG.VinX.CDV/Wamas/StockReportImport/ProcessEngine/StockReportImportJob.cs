using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.StockReportImport.ProcessEngine;

[JobInfo("3BD836C7-F185-4BD6-A94C-4EDCE517253F", JobName)]
public class StockReportImportJob : JobBase<StockReportImportJobConfig, JobParameter, WamasImportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasStockReportImportJobName;

    private readonly IStockReportImporter _stockReportImporter;

    public StockReportImportJob(IStockReportImporter stockReportImporter)
    {
        _stockReportImporter = stockReportImporter;
    }

    protected override void ExecuteJob()
    {
        _stockReportImporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _stockReportImporter.ImportStockReports();

        Result.Result = result.Result;
        Result.ImportedCount = result.ImportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}