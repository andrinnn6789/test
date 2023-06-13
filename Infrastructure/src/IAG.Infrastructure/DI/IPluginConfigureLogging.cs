namespace IAG.Infrastructure.DI
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public interface IPluginConfigureLogging
    {
        void ConfigPlugin(ILoggingBuilder logging, IHostingEnvironment env);
    }
}