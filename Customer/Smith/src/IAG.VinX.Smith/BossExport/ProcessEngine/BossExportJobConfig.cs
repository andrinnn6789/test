using IAG.Common.MailSender.Config;
using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Smith.BossExport.ProcessEngine;

public class BossExportJobConfig : JobConfig<BossExportJob>
{
    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public string MailReceiver { get; set; } = "$$mailReceiverBossExport$";

    public MailSenderConfig MailConfig { get; set; } = new();
}