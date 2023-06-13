using System.Collections.Generic;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Store.Model;

using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Store;

public class JobCatalogue : IJobCatalogue
{
    private readonly IPluginLoader _pluginLoader;
    private readonly ILogger<JobCatalogue> _logger;

    public JobCatalogue(IPluginLoader pluginLoader, ILogger<JobCatalogue> logger)
    {
        _pluginLoader = pluginLoader;
        _logger = logger;
        Catalogue = new List<IJobMetadata>();
        LoadCatalogue();
    }

    public List<IJobMetadata> Catalogue { get; }

    public void Reload()
    {
        Catalogue.Clear();
        LoadCatalogue();
    }

    private void LoadCatalogue()
    {
        foreach (var jobType in _pluginLoader.GetImplementations<IJob>("IAG*.dll"))
        {
            try
            {
                _logger.LogTrace($"loading {jobType.FullName}");

                var jobInfoAttribute = JobInfoAttribute.GetJobInfo(jobType);

                var metadata = new JobMetadata
                {
                    TemplateId = jobInfoAttribute.TemplateId,
                    JobType = jobType,
                    // ReSharper disable once PossibleNullReferenceException (will be available since implementing IJob...)
                    ConfigType = jobType.GetProperty(nameof(IJob.Config)).PropertyType,
                    // ReSharper disable once PossibleNullReferenceException (will be available since implementing IJob...)
                    ParameterType = jobType.GetProperty(nameof(IJob.Parameter)).PropertyType,
                    PluginName = jobInfoAttribute.Name,
                    AutoActivate = jobInfoAttribute.AutoActivate
                };
                Catalogue.Add(metadata);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, $"PE job loading error ({jobType.FullName})");
            }
        }
    }
}