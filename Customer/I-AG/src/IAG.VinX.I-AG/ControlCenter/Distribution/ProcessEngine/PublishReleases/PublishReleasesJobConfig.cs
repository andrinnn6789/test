using System;
using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.PublishReleases;

[UsedImplicitly]
public class PublishReleasesJobConfig : JobConfig<PublishReleasesJob>
{
    public PublishReleasesJobConfig()
    {
        HeartbeatTimeout = new TimeSpan(0, 0, 5, 0);
    }

    public BackendConfig Backend { get; set; } = new();
    public string ArtifactsPath { get; set; } = "A:\\DotNet\\IAG";
    public string SettingsPath { get; set; } = "L:\\I-AG Software\\BPE\\Settings-Templates";
    public Dictionary<string, string> ReleasePaths { get; set; } = new()
    {
        {ArtifactsScanner.PerformXDirectoryName, "BPE"},
        {ArtifactsScanner.VinXDirectoryName, "BPE"},
        {ArtifactsScanner.CustomerExtensionsDirectoryName, string.Empty}, // relative to product installation!
        {SettingsScanner.SettingsDirectoryName, "Settings"} // relative to product installation!
    };
}