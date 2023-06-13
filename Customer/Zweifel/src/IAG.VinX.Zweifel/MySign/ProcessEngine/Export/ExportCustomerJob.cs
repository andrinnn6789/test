using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Zweifel.Resource;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

[JobInfo("DD2CCDD4-BF02-4C42-BF62-12D6BFBF5F64", JobName)]
public class ExportCustomerJob : JobBase<ExportCustomerConfig, JobParameter, ExportResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "MySignExportCustomer";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    public ExportCustomerJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        Result.Name = "Stammdaten";
        using (var helper = new GenericExporter(_sybaseConnectionFactory.CreateConnection(Config.Config.ConnectionString), this, Config.Config.ExportFolder))
        {
            var customers = helper.VinxConnector.GetCustomers();
            Result.SyncResult.Add(new ExportResultDetail { Name = "Kunden-Export", ExportCount = customers.Items.Count });
            helper.ExportFile("Kunden_Export", customers);

            Result.Result =
                helper.ErrorCount == 0 ? JobResultEnum.Success :
                helper.SuccessCount == 0 ? JobResultEnum.Failed :
                JobResultEnum.PartialSuccess;
        }

        base.ExecuteJob();
    }
}