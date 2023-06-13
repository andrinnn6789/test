using System;

namespace IAG.ProcessEngine.Store.Model;

public interface IJobMetadata
{
    /// <summary>
    ///     Id of the job-class, used to link with the different configs and logs
    /// </summary>
    Guid TemplateId { get; }

    Type JobType { get; }

    Type ConfigType { get; }

    Type ParameterType { get; }

    /// <summary>
    ///     Name of the plugin, from JobInfoAttribute
    /// </summary>
    string PluginName { get; }

    /// <summary>
    ///     Description of the plugin from the translations via PluginName
    /// </summary>
    string Description { get; set; }

    /// <summary>
    ///     Whether a config should be created at startup if missing (from JobInfoAttribute)
    /// </summary>
    public bool AutoActivate { get; set; }
}