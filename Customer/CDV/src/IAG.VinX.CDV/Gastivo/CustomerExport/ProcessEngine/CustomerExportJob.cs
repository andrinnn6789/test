using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;

namespace IAG.VinX.CDV.Gastivo.CustomerExport.ProcessEngine;

[JobInfo("61EF3349-429D-4BC2-AC94-A1AFF8FF1AAE", JobName)]
public class CustomerExportJob : JobBase<CustomerExportJobConfig, JobParameter, GastivoExportJobResult>
{
    private const string JobName = Resource.ResourceIds.GastivoCustomerExportJobName;

    private readonly ICustomerExporter _customerExporter;

    public CustomerExportJob(ICustomerExporter customerExporter)
    {
        _customerExporter = customerExporter;
    }

    protected override void ExecuteJob()
    {
        _customerExporter.SetConfig(Config.GastivoFtpConfig, Config.ConnectionString, this);

        var result = _customerExporter.ExportCustomers();

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}