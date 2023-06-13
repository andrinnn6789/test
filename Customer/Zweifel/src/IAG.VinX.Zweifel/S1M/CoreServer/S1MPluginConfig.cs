using IAG.Infrastructure.CoreServer.Plugin;

namespace IAG.VinX.Zweifel.S1M.CoreServer;

public class S1MPluginConfig : PluginConfig<S1MPlugin>
{
    public string S1MMediaFolderPath { get; set; } = "$$s1MMediaFolderPath$";
}