using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.StockAdjustmentImport.ProcessEngine;

[JobInfo("B71A2B22-4F72-46A8-96E5-861C50F20A22", JobName)]
public class StockAdjustmentImportJob : JobBase<StockAdjustmentImportJobConfig, JobParameter, WamasImportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasStockAdjustmentImportJobName;

    private readonly IStockAdjustmentImporter _stockAdjustmentImporter;

    public StockAdjustmentImportJob(IStockAdjustmentImporter stockAdjustmentImporter)
    {
        _stockAdjustmentImporter = stockAdjustmentImporter;
    }

    protected override void ExecuteJob()
    {
        _stockAdjustmentImporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _stockAdjustmentImporter.ImportStockAdjustments();

        Result.Result = result.Result;
        Result.ImportedCount = result.ImportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}