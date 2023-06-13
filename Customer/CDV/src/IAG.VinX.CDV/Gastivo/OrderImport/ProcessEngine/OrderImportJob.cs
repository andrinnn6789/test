using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.OrderImport.ProcessEngine;

[JobInfo("D57373A7-BE58-4EC7-A2CE-9ACEE03B5EFD", JobName)]
public class OrderImportJob : JobBase<OrderImportJobConfig, JobParameter, GastivoImportJobResult>
{
    private const string JobName = Resource.ResourceIds.GastivoOrderImportJobName;

    private readonly IOrderImporter _orderImporter;

    public OrderImportJob(IOrderImporter priceExporter)
    {
        _orderImporter = priceExporter;
    }

    protected override void ExecuteJob()
    {
        _orderImporter.SetConfig(Config.GastivoFtpConfig, Config.ConnectionString, this);

        var result = _orderImporter.ImportOrders();

        Result.Result = result.Result;
        Result.ImportedCount = result.ImportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}