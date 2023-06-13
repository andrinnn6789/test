using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.GoodsReceiptImport.ProcessEngine;

[JobInfo("64D52943-6483-4269-9C37-4255BDC93E9B", JobName)]
public class GoodsReceiptImportJob : JobBase<GoodsReceiptImportJobConfig, JobParameter, WamasImportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasGoodsReceiptImportJobName;

    private readonly IGoodsReceiptImporter _goodsReceiptImporter;

    public GoodsReceiptImportJob(IGoodsReceiptImporter goodsReceiptImporter)
    {
        _goodsReceiptImporter = goodsReceiptImporter;
    }

    protected override void ExecuteJob()
    {
        _goodsReceiptImporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _goodsReceiptImporter.ImportGoodsReceipts();

        Result.Result = result.Result;
        Result.ImportedCount = result.ImportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}