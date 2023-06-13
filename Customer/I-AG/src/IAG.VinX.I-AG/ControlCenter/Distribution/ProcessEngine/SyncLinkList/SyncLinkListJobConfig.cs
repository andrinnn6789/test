using System;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.IAG.ControlCenter.Config;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncLinkList;

[UsedImplicitly]
public class SyncLinkListJobConfig : JobConfig<SyncLinkListJob>
{
    public SyncLinkListJobConfig()
    {
        HeartbeatTimeout = new TimeSpan(0, 0, 5, 0);
    }

    public BackendConfig Backend { get; set; } = new();
    public string LinkListsPath { get; set; } = "L:\\I-AG Software\\BPE\\Link-Listen";
}