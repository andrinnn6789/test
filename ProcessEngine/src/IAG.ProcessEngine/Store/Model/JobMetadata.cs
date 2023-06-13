using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.ProcessEngine.Store.Model;

[ExcludeFromCodeCoverage]
public class JobMetadata : IJobMetadata
{
    public Guid TemplateId { get; set; }

    public Type JobType { get; set; }

    public Type ConfigType { get; set; }

    public Type ParameterType { get; set; }

    public string PluginName { get; set; }

    public string Description { get; set; }
        
    public bool AutoActivate { get; set; }
}