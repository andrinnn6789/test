using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Configuration.Macro;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.ProcessEngine.DataLayer.Settings.Context;
using IAG.ProcessEngine.DataLayer.Settings.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using FollowUpJob = IAG.Infrastructure.ProcessEngine.Configuration.FollowUpJob;


namespace IAG.ProcessEngine.Store;

public class JobConfigStoreDb : IJobConfigStore
{
    private readonly SettingsDbContext _context;
    private readonly IJobCatalogue _jobCatalogue;
    private readonly ILogger _logger;
    private readonly IMacroReplacer _macroReplacer;

    public JobConfigStoreDb(SettingsDbContext context, IJobCatalogue jobCatalogue, IConfigurationRoot configurationRoot, ILogger<JobConfigStoreDb> logger)
    {
        _context = context;
        _jobCatalogue = jobCatalogue;
        _logger = logger;
        _macroReplacer = new MacroReplacer(new MacroValueSource(context, configurationRoot));  // Cannot be injected since ConfigCommonStoreDbContext is unknown!
    }

    public void Delete(IJobConfig config)
    {
        var configEntry = _context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id);
        if (configEntry == null)
        {
            throw new NotFoundException(config.Id.ToString());
        }
            
        _context.ConfigEntries.Remove(configEntry);
        _context.SaveChanges();
    }

    public void Insert(IJobConfig config)
    {
        if (_context.ConfigEntries.AsNoTracking().Any(t => t.Id == config.Id))
        {
            throw new DuplicateKeyException(config.Id.ToString());
        }

        var configEntry = new JobConfig
        {
            Id = config.Id,
            TemplateId = config.TemplateId,
            Name = config.Name,
            FollowUpJobs = new List<DataLayer.Settings.Model.FollowUpJob>()
        };
        UpdateJobConfig(configEntry, config);

        _context.ConfigEntries.Add(configEntry);
        _context.SaveChanges();
    }

    public IEnumerable<IJobConfig> GetAll()
    {
        return GetAll(true);
    }

    public IEnumerable<IJobConfig> GetAllUnprocessed()
    {
        return GetAll(false);
    }

    public IJobConfig Read(Guid id)
    {
        return GetSingle(id, true);
    }

    public IJobConfig ReadUnprocessed(Guid id)
    {
        return GetSingle(id, false);
    }

    public void Update(IJobConfig config)
    {
        var configEntry = _context.ConfigEntries.Include(t => t.FollowUpJobs).FirstOrDefault(t => t.Id == config.Id);
        if (configEntry == null)
        {
            throw new NotFoundException(config.Id.ToString());
        }
            
        UpdateJobConfig(configEntry, config);

        _context.ConfigEntries.Update(configEntry);
        _context.SaveChanges();
    }

    public IEnumerable<IFollowUpJob> GetFollowUpJobs(Guid id)
    {
        return _context.FollowUpJobs.Where(x => x.MasterId == id).Select(x => new FollowUpJob
        {
            FollowUpJobConfigId = x.FollowUpId,
            ExecutionCondition = x.ExecutionCondition,
            Description = x.Description
        });
    }

    public IJobConfig GetOrCreateJobConfig(string jobConfigName)
    {
        var allConfigs = GetAll().ToList();
        var jobConfigs = allConfigs.Where(m => m.Name.Contains(jobConfigName, StringComparison.OrdinalIgnoreCase)).ToList();
        switch (jobConfigs.Count)
        {
            case 1:
                return jobConfigs[0];
        }

        var jobConfig = allConfigs.FirstOrDefault(m => m.Name.ToUpper() == jobConfigName.ToUpper());
        if (jobConfig != null)
        {
            return jobConfig;
        }

        var jobMetas = _jobCatalogue.Catalogue.Where(m => m.PluginName.Contains(jobConfigName, StringComparison.OrdinalIgnoreCase)).ToList();
        switch (jobMetas.Count)
        {
            case 1:
                jobConfig = ActivatorWithCheck.CreateInstance<IJobConfig>(jobMetas[0].ConfigType);
                jobConfig.Id = jobMetas[0].TemplateId;
                Insert(jobConfig);

                return jobConfig;
            default:
                throw new NotFoundException(jobConfigName);
        }
    }

    public void EnsureConfig(Guid templateId, Type configType)
    {
        if (_context.ConfigEntries.AsNoTracking().FirstOrDefault(config => config.TemplateId == templateId) != null)
        {
            return;
        }

        var jobConfig = ActivatorWithCheck.CreateInstance<IJobConfig>(configType);
        jobConfig.Id = templateId;
        Insert(jobConfig);
    }

    private IEnumerable<IJobConfig> GetAll(bool processed)
    {
        foreach (var configEntry in _context.ConfigEntries.AsNoTracking().ToList())
        {
            IJobConfig jobConfig;
            try
            {
                jobConfig = ConvertJobConfig(configEntry, processed);
            }
            catch (NotFoundException)
            {
                _logger.LogDebug($"config not found: {configEntry.Name} / {configEntry.TemplateId}");
                continue;
            }

            yield return jobConfig;
        }
    }


    private IJobConfig GetSingle(Guid id, bool processed)
    {
        var configEntry = _context.ConfigEntries.Include(t => t.FollowUpJobs).FirstOrDefault(t => t.Id == id);
        if (configEntry == null)
        {
            throw new NotFoundException(id.ToString());
        }

        var config = ConvertJobConfig(configEntry, processed);
        config.Id = id;

        return config;
    }

    private void UpdateJobConfig(JobConfig configEntry, IJobConfig config)
    {
        configEntry.Description = config.Description;
        var tempFollowUps = config.FollowUpJobs?? new List<FollowUpJob>();
        config.FollowUpJobs = null; // don't persist in data..
        configEntry.Data = JsonConvert.SerializeObject(config);
        config.FollowUpJobs = tempFollowUps;

        var removedFollowUpJobs = configEntry.FollowUpJobs.ToDictionary(x => x.Id, x => x);
        foreach (var followUpJob in config.FollowUpJobs)
        {
            var followUpEntry = configEntry.FollowUpJobs.FirstOrDefault(x => x.Id == followUpJob.Id);
            if (followUpEntry == null)
            {
                followUpEntry = new DataLayer.Settings.Model.FollowUpJob()
                {
                    Id = followUpJob.Id,
                    MasterId = config.Id,
                };
                configEntry.FollowUpJobs.Add(followUpEntry);
            }

            followUpEntry.FollowUpId = followUpJob.FollowUpJobConfigId;
            followUpEntry.ExecutionCondition = followUpJob.ExecutionCondition;
            followUpEntry.Description = followUpJob.Description;

            removedFollowUpJobs.Remove(followUpJob.Id);
        }
        foreach (var followUpJob in removedFollowUpJobs.Values)
        {
            configEntry.FollowUpJobs.Remove(followUpJob);
        }
    }

    private IJobConfig ConvertJobConfig(JobConfig configEntry, bool processed)
    {
        var meta = _jobCatalogue.Catalogue.FirstOrDefault(m => m.TemplateId == configEntry.TemplateId);
        if (meta == null)
        {
            throw new NotFoundException(configEntry.TemplateId.ToString());
        }

        var configData = processed ? _macroReplacer.ReplaceMacros(configEntry.Data): configEntry.Data;

        var config = (IJobConfig)JsonConvert.DeserializeObject(configData, meta.ConfigType);
        config!.SerializedData = configData;

        if (configEntry.FollowUpJobs != null)
        {
            config.FollowUpJobs = new List<FollowUpJob>();
            foreach (var followUpJob in configEntry.FollowUpJobs)
            {
                config.FollowUpJobs.Add(new FollowUpJob()
                {
                    Id = followUpJob.Id,
                    FollowUpJobConfigId = followUpJob.FollowUpId,
                    ExecutionCondition = followUpJob.ExecutionCondition,
                    Description = followUpJob.Description
                });
            }
        }

        return config;
    }
}