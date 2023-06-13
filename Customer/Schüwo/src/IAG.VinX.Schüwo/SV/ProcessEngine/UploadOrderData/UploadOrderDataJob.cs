using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.Resource;

namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadOrderData;

[JobInfo("40C1B910-8982-4FF7-8EBC-C6F84C3CB0DE", JobName)]
public class UploadOrderDataJob : SvBaseJob<UploadOrderDataJobConfig, JobParameter, UploadOrderDataJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SVUploadOrderData";

    public UploadOrderDataJob(ISybaseConnectionFactory sybaseConnectionFactory)
        : base(sybaseConnectionFactory)
    {
    }

    protected override void Sync()
    {
        Result.ArchiveOrdersCount = FormatAndUploadData(Extractor.ExtractArchiveOrders(), Config.FtpPathConfig.ArchiveOrdersDataName);
    }
}