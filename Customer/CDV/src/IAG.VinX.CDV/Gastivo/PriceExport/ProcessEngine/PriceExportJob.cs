using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.PriceExport.ProcessEngine;

[JobInfo("9FBABE10-64AC-45DB-8261-AFD5A2248576", JobName)]
public class PriceExportJob : JobBase<PriceExportJobConfig, JobParameter, GastivoExportJobResult>
{
    private const string JobName = Resource.ResourceIds.GastivoPriceExportJobName;

    private readonly IPriceExporter _priceExporter;

    public PriceExportJob(IPriceExporter priceExporter)
    {
        _priceExporter = priceExporter;
    }

    protected override void ExecuteJob()
    {
        _priceExporter.SetConfig(Config.GastivoFtpConfig, Config.ConnectionString, this);

        var result = _priceExporter.ExportPrices();

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}